using BookCollector.Framework.Logging;

namespace BookCollector.Models
{
    public class ApplicationModel : IApplicationModel
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private ICollectionModel collection_model;
        private ISettingsModel settings_model;

        public ISettingsModel SettingsModel { get { return settings_model; } }

        public ICollectionModel CollectionModel { get { return collection_model; } }

        public ApplicationModel(ICollectionModel collection_model, ISettingsModel settings_model)
        {
            this.collection_model = collection_model;
            this.settings_model = settings_model;
        }

        public void Load()
        {
            log.Info("Loading");

            settings_model.Load();

            // Handle current collection
            //if (Settings.LoadCollectionOnStartup && !string.IsNullOrEmpty(Settings.LastCollectionFilename) && data_service.CollectionExists(Settings.LastCollectionFilename))
            //    LoadCurrentCollection(Settings.LastCollectionFilename);
        }

        public void Save()
        {
            log.Info("Saving");

            settings_model.Save();
        }
    }
}
