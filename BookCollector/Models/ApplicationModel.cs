using System;
using System.Collections.Generic;
using System.Linq;
using BookCollector.Data;
using BookCollector.Domain;
using BookCollector.Framework.Extensions;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Mapping;
using BookCollector.Framework.Messaging;
using ReactiveUI;

namespace BookCollector.Models
{
    public class ApplicationModel : ReactiveObject, IApplicationModel
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IDataService data_service;

        private Collection _CurrentCollection;
        public Collection CurrentCollection
        {
            get { return _CurrentCollection; }
            set { this.RaiseAndSetIfChanged(ref _CurrentCollection, value); }
        }

        private Shelf _CurrentShelf;
        public Shelf CurrentShelf
        {
            get { return _CurrentShelf; }
            set { this.RaiseAndSetIfChanged(ref _CurrentShelf, value); }
        }

        public ApplicationModel(IEventAggregator event_aggregator, IDataService data_service)
        {
            this.data_service = data_service;

            this.WhenAnyValue(x => x.CurrentCollection)
                .Subscribe(collection => event_aggregator.Publish(ApplicationMessage.CollectionChangedMessage(collection?.Description.Filename)));
        }

        public void AddToCurrentCollection(List<Book> books)
        {
            log.Info($"Adding {books.Count} to current collection");

            // Add books
            CurrentCollection.Books.AddRange(books);
            // Add shelves
            CurrentCollection.Shelves = CurrentCollection.Books.SelectMany(b => b.Shelves).Distinct().ToReactiveList();
        }

        public void LoadCurrentCollection(string path)
        {
            log.Info($"Loading current collection {path}");

            if (!data_service.CollectionExists(path))
            {
                log.Warn($"No collection found for {path}");
                return;
            }

            CurrentCollection = data_service.LoadCollection(path);
            CurrentShelf = CurrentCollection.Shelves.Single(s => s.Name == ShelfNames.AllShelf);
        }

        public void SaveCurrentCollection()
        {
            if (CurrentCollection == null)
                return;

            log.Info($"Loading current collection {CurrentCollection.Description.Filename}");

            data_service.SaveCollection(CurrentCollection);
        }

        public void AddCollection(Description description)
        {
            log.Info($"Adding collection {description.Filename}");

            data_service.SaveCollection(new Collection()
            {
                Description = description,
                Shelves = new ReactiveList<Shelf>()
                {
                    new Shelf(ShelfNames.AllShelf, ShelfNames.AllShelfDescription, true)
                }
            });
        }

        public void UpdateCollection(Description description)
        {
            log.Info($"Updating collection {description.Filename}");

            // Load, update and save back the collection
            var collection = data_service.LoadCollection(description.Filename);
            Mapper.Map(description, collection.Description);
            data_service.SaveCollection(collection);

            // If this was the current collection, reload this
            if (CurrentCollection != null && CurrentCollection.Description.Filename == description.Filename)
                LoadCurrentCollection(description.Filename);
        }

        public void DeleteCollection(Description description)
        {
            log.Info($"Deleting collection {description.Filename}");

            // Delete the file
            data_service.DeleteCollection(description.Filename);

            // If this was the current collect, handle it
            if (CurrentCollection != null && CurrentCollection.Matches(description))
                CurrentCollection = null;
        }

        public List<Description> GetAllCollectionDescriptions()
        {
            log.Info("Loading all collection descriptions");

            return data_service.GetAllCollectionDescriptions();
        }
    }
}
