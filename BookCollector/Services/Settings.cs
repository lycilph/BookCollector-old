using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using NLog;
using Panda.Utilities.Extensions;

namespace BookCollector.Services
{
    [Export(typeof(Settings))]
    public class Settings
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private const string Filename = "settings.txt";
        private const string DefaultDataFolder = "Data";

        public static string ApplicationFolder
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }

        private string _DataFolder;
        public string DataFolder
        {
            get
            {
                Directory.CreateDirectory(_DataFolder);
                return _DataFolder;
            }
            set { _DataFolder = value; }
        }

        public bool LoadLastCollection { get; set; }

        public Settings()
        {
            DataFolder = Path.Combine(ApplicationFolder, DefaultDataFolder);
            LoadLastCollection = true;
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
