using System.Collections.Generic;
using System.Linq;
using BookCollector.Data;
using BookCollector.Framework.Extensions;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Mapping;
using BookCollector.Framework.Messaging;
using BookCollector.Services;

namespace BookCollector.Domain
{
    public class ApplicationModel : IApplicationModel
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IEventAggregator event_aggregator;
        private IDataService data_service;
        private IThemeService theme_service;

        public Settings Settings { get; private set; }

        private Collection _CurrentCollection;
        public Collection CurrentCollection
        {
            get { return _CurrentCollection; }
            private set
            {
                _CurrentCollection = value;
                event_aggregator.Publish(ApplicationMessage.CollectionChanged());
            }
        }

        public ApplicationModel(IEventAggregator event_aggregator, IDataService data_service, IThemeService theme_service)
        {
            this.event_aggregator = event_aggregator;
            this.data_service = data_service;
            this.theme_service = theme_service;
        }

        public void Load()
        {
            log.Info("Loading");

            // Handle settings
            if (data_service.SettingsExists())
            {
                Settings = data_service.LoadSettings();
                SetCurrentTheme();
            }
            else
            {
                Settings = new Settings();
                GetCurrentTheme();
            }

            // Handle current collection
            if (Settings.LoadCollectionOnStartup && !string.IsNullOrEmpty(Settings.LastCollectionFilename) && data_service.CollectionExists(Settings.LastCollectionFilename))
                LoadCurrentCollection(Settings.LastCollectionFilename);
        }

        public void Save()
        {
            log.Info("Saving");

            data_service.SaveSettings(Settings);

            if (CurrentCollection != null)
                data_service.SaveCollection(CurrentCollection);
        }

        public void AddToCurrentCollection(List<Book> books)
        {
            log.Info($"Adding {books.Count} books to current collection");

            // Add books
            CurrentCollection.Books.AddRange(books);
            // Add new shelves
            var new_shelves = books.SelectMany(b => b.Shelves)
                                   .Distinct()
                                   .Except(CurrentCollection.Shelves)
                                   .ToList();
            CurrentCollection.Shelves.AddRange(new_shelves);
            // Save collection
            data_service.SaveCollection(CurrentCollection);
            // This should also trigger a CollectionChanged event
            event_aggregator.Publish(ApplicationMessage.CollectionChanged());
        }

        public void AddToCurrentCollection(Shelf shelf)
        {
            log.Info($"Adding shelf {shelf.Name} to current collection");

            // Add shelf
            CurrentCollection.Shelves.Add(shelf);
            // Save collection
            data_service.SaveCollection(CurrentCollection);
        }

        public void RemoveFromCurrentCollection(Shelf shelf)
        {
            log.Info($"Removing shelf {shelf.Name} from current collection");

            // Remove shelf
            CurrentCollection.Shelves.Remove(shelf);
            // Remove shelf from all books
            CurrentCollection.Books.Where(b => b.Shelves.Contains(shelf))
                                   .Apply(b => b.Shelves.Remove(shelf));
            // Save collection
            data_service.SaveCollection(CurrentCollection);
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
            Settings.LastCollectionFilename = CurrentCollection.Description.Filename;
        }

        public void SaveCurrentCollection()
        {
            data_service.SaveCollection(CurrentCollection);
        }

        public void AddCollection(Description description)
        {
            log.Info($"Adding collection {description.Filename ?? "[Empty filename]"}");

            data_service.SaveCollection(new Collection()
            {
                Description = description,
                Shelves = new List<Shelf>() { new Shelf(Constants.AllShelfName, Constants.AllShelfDescription, true) }
            });
        }

        public void UpdateCollection(Description description)
        {
            log.Info($"Updating collection {description.Filename}");

            // Load, update and save the collection
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
            if (CurrentCollection != null && CurrentCollection.Description.Filename == description.Filename)
                CurrentCollection = null;
        }

        public Description CreateCollectionDescription()
        {
            log.Info($"Creating new collection description");

            // The 1 accounts for the "all" shelf that is always added to a collection (see the AddCollection method)
            return new Description { ShelfCount = 1 };
        }

        public List<Description> GetAllCollectionDescriptions()
        {
            log.Info("Loading all collection descriptions");

            return data_service.GetAllCollectionDescriptions();
        }

        private void GetCurrentTheme()
        {
            Settings.PrimaryColor = theme_service.GetCurrentPrimaryColor();
            Settings.AccentColor = theme_service.GetCurrentAccentColor();
        }

        private void SetCurrentTheme()
        {
            theme_service.Set(Settings.PrimaryColor, Settings.AccentColor);
        }
    }
}
