using System.IO;
using System.Reflection;
using BookCollector.Data;
using BookCollector.Framework.Extensions;

namespace BookCollector.Services
{
    public class DataService : IDataService
    {
        private const string settings_filename = "ApplicationSettings.json";
        private const string collection_extension = ".bcdb"; // bcdb = Book Collector DataBase
        private const string collection_search_pattern = "*" + collection_extension;

        public bool SettingsExists()
        {
            return File.Exists(GetSettingsPath());
        }

        public Settings LoadSettings()
        {
            if (!SettingsExists())
                return null;

            var settings = JsonExtensions.ReadFromFile<Settings>(GetSettingsPath());
            settings.IsDirty = false;
            return settings;
        }

        public void SaveSettings(Settings settings)
        {
            if (settings.IsDirty || !SettingsExists())
            {
                JsonExtensions.WriteToFile<Settings>(GetSettingsPath(), settings);
                settings.IsDirty = false;
            }
        }

        private string GetDataDirectory()
        {
            var app_path = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(app_path);
        }

        private string GetSettingsPath()
        {
            return Path.Combine(GetDataDirectory(), settings_filename);
        }
    }
}
