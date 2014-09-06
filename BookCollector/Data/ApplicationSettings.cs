using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using BookCollector.Utils;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Data
{
    [Export(typeof(ApplicationSettings))]
    public class ApplicationSettings : ReactiveObject
    {
        private const string DefaultFolder = "Data";
        private const string Filename = "Settings.txt";

        private string _DataFolder = DefaultFolder;
        [JsonProperty]
        public string DataFolder
        {
            get { return _DataFolder; }
            set { this.RaiseAndSetIfChanged(ref _DataFolder, value); }
        }

        private bool _KeepStartOpen = true;
        [JsonProperty]
        public bool KeepStartOpen
        {
            get { return _KeepStartOpen; }
            set { this.RaiseAndSetIfChanged(ref _KeepStartOpen, value); }
        }

        public string GetFilename(string filename)
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(dir, DataFolder, filename);
        }

        public void Load()
        {
            var path = GetFilename(Filename);
            if (!File.Exists(path))
                return;

            var json = File.ReadAllText(path);
            var settings = JsonConvert.DeserializeObject<ApplicationSettings>(json);

            Mapper.MapPublicProperties(settings, this);
        }

        public void Save()
        {
            var path = GetFilename(Filename);
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
