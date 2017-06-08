namespace BookCollector.Models
{
    public interface ISettingsModel
    {
        string LastCollectionFilename { get; set; }
        bool LoadCollectionOnStart { get; set; }
        string PrimaryColorName { get; set; }
        string AccentColorName { get; set; }

        void Load();
        void Save();
    }
}