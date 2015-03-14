using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using NLog;
using Panda.Utilities.Extensions;

namespace BookCollector.Services
{
    [Export(typeof(ISettings))]
    public class Settings : ISettings
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private const string Filename = "settings.txt";
        private const string DefaultDataFolder = "Data";

        public static string ApplicationFolder
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }

        public string DataFolder { get; set; }
        public bool LoadLastCollection { get; set; }
        public string LastUserId { get; set; }
        public string LastCollectionId { get; set; }

        public Settings()
        {
            DataFolder = Path.Combine(ApplicationFolder, DefaultDataFolder);
            LoadLastCollection = true;
        }

        public string GetPathFor(string filename)
        {
            if (!Directory.Exists(DataFolder))
                Directory.CreateDirectory(DataFolder);
            return Path.Combine(DataFolder, filename);
        }

        public void Load()
        {
            logger.Trace("Loading settings");

            var path = Path.Combine(ApplicationFolder, Filename);
            if (!File.Exists(path)) return;

            var loaded_settings = JsonExtensions.ReadFromFile<Settings>(path);
            loaded_settings.CopyPropertiesTo(this);
        }

        public void Save()
        {
            logger.Trace("Saving settings");

            var path = Path.Combine(ApplicationFolder, Filename);
            JsonExtensions.WriteToFile(path, this);
        }
    }
}
