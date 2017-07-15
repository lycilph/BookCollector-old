using BookCollector.Framework.Logging;
using BookCollector.Services;

namespace BookCollector.Models
{
    public class ApplicationModel : IApplicationModel
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IDataService data_service;
        private ICollectionModel collection_model;
        private ISettingsModel settings_model;

        public ISettingsModel SettingsModel { get { return settings_model; } }

        public ICollectionModel CollectionModel { get { return collection_model; } }

        public ApplicationModel(IDataService data_service, ICollectionModel collection_model, ISettingsModel settings_model)
        {
            this.data_service = data_service;
            this.collection_model = collection_model;
            this.settings_model = settings_model;
        }

        public void Load()
        {
            log.Info("Loading");

            settings_model.Load();

            var settings = settings_model.Settings;
            if (settings.LoadCollectionOnStartup && !string.IsNullOrEmpty(settings.LastCollectionFilename) && data_service.CollectionExists(settings.LastCollectionFilename))
                collection_model.LoadCurrentCollection(settings.LastCollectionFilename);
        }

        public void Save()
        {
            log.Info("Saving");

            settings_model.Save();
            collection_model.SaveCurrentCollection();
        }
    }
}
