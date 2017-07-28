using System.IO;
using System.Reflection;
using BookCollector.Data;
using Core.Extensions;

namespace BookCollector.Services
{
    public class SettingsRepository : ISettingsRepository
    {
        private const string settings_filename = "ApplicationSettings.json";

        public Settings LoadOrCreate()
        {
            if (Exists())
                return Load();

            return Create();
        }

        public void Save(Settings settings)
        {
            if (settings.IsDirty || !Exists())
            {
                JsonExtensions.WriteToFile<Settings>(GetSettingsPath(), settings);
                settings.IsDirty = false;
            }
        }

        private Settings Create()
        {
            return new Settings
            {
                LoadCollectionOnStartup = true,
                LastCollectionFilename = string.Empty,
                PrimaryColor = "Green",
                AccentColor = "Teal"
            };
        }

        private Settings Load()
        {
            var settings = JsonExtensions.ReadFromFile<Settings>(GetSettingsPath());
            settings.IsDirty = false;
            return settings;
        }

        private bool Exists()
        {
            return File.Exists(GetSettingsPath());
        }

        private string GetSettingsPath()
        {
            var app_path = Assembly.GetExecutingAssembly().Location;
            var app_directory = Path.GetDirectoryName(app_path);
            return Path.Combine(app_directory, settings_filename);
        }
    }
}
