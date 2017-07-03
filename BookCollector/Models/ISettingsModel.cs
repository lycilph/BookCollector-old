using BookCollector.Data;

namespace BookCollector.Models
{
    public interface ISettingsModel
    {
        Settings Settings { get; }

        void Load();
        void Save();
    }
}