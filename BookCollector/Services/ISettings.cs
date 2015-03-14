namespace BookCollector.Services
{
    public interface ISettings
    {
        string DataFolder { get; set; }
        bool LoadLastCollection { get; set; }
        string LastCollectionId { get; set; }
        string LastUserId { get; set; }
        string GetPathFor(string filename);
        void Load();
        void Save();
    }
}