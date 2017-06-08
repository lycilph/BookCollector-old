namespace BookCollector.Models
{
    public interface ISettingsModel
    {
        string LastCollectionFilename { get; set; }
        bool LoadCollectionOnStart { get; set; }

        void Load();
        void Save();
    }
}