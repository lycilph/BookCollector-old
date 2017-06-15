using BookCollector.Data;

namespace BookCollector.Services
{
    public interface IDataService
    {
        bool SettingsExists();
        Settings LoadSettings();
        void SaveSettings(Settings settings);
    }
}