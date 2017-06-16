using System.Collections.Generic;
using BookCollector.Data;
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

            return new Description
            {
                ShelfCount = 1 // This accounts for the "all" shelf that is always added to a collection (see the AddCollection method)
            };
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
