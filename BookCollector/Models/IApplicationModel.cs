namespace BookCollector.Models
{
    public interface IApplicationModel
    {
        ISettingsModel SettingsModel { get; }
        ICollectionModel CollectionModel { get; }

        void Load();
        void Save();
    }
}