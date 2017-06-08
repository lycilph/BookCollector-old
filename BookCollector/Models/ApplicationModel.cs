using System;
using System.Collections.Generic;
using BookCollector.Domain;
using BookCollector.Framework.Logging;
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

        public ApplicationModel(IEventAggregator event_aggregator, IDataService data_service)
        {
            this.data_service = data_service;

            this.WhenAnyValue(x => x.CurrentCollection)
                .Subscribe(collection => event_aggregator.Publish(ApplicationMessage.CollectionChangedMessage(collection?.Description.Filename)));
        }

        public List<Description> GetAllCollectionDescriptions()
        {
            log.Info("Loading all collection descriptions");

            return data_service.GetAllCollectionDescriptions();
        }

        public void AddToCurrentCollection(List<Book> books)
        {
            log.Info($"Adding {books.Count} to current collection");

            //CurrentCollection.Books.AddRange(books);
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
        }

        public void SaveCurrentCollection()
        {
            //log.Info($"Loading current collection {CurrentCollection.Description.Filename}");

            //data_controller.SaveCollection(CurrentCollection);
        }
    }
}
