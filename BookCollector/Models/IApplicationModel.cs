namespace BookCollector.Models
{
    public interface IApplicationModel
    {
        ICollectionModel CollectionModel { get; }

        void Load();
        void Save();
    }
}