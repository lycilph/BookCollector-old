using BookCollector.Data;

namespace BookCollector.Services
{
    public class CollectionsService : ICollectionsService
    {
        private ICollectionsRepository collections_repository;
        private ISettingsService settings_service;

        public Collection Current { get; set; }

        public CollectionsService(ICollectionsRepository collections_repository, ISettingsService settings_service)
        {
            this.collections_repository = collections_repository;
            this.settings_service = settings_service;
        }

        public void Initialize()
        {
            var settings = settings_service.Settings;
            if (settings.LoadCollectionOnStartup && 
                !string.IsNullOrWhiteSpace(settings.LastCollectionFilename) && 
                collections_repository.Exists(settings.LastCollectionFilename))
            {
                Current = collections_repository.Load(settings.LastCollectionFilename);
            }
            else
            {
                // DEBUG ONLY
                Current = new Collection(default_shelf_name: "All");
                Current.Description.Name = "Collection 1";
                Current.Description.Summary = "My Collection";
            }
        }

        public void Exit()
        {
            Save();
        }

        private void Save()
        {
            collections_repository.Save(Current);
            settings_service.Settings.LastCollectionFilename = Current.Description.Filename;
        }
    }
}
