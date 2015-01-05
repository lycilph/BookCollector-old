using System;
using System.IO;
using System.Reactive.Linq;
using System.Reflection;
using BookCollector.Utilities;
using Newtonsoft.Json;
using NLog;
using ReactiveUI;

namespace BookCollector.Services.Settings
{
    [JsonObject(MemberSerialization.OptOut)]
    public sealed class ApplicationSettings : ReactiveObject
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly ApplicationSettings instance = new ApplicationSettings();

        private const string filename = "Settings.txt";
        private const string data = "Data";
        private const string image = "Images";

        private readonly string settings_path;

        public static ApplicationSettings Instance
        {
            get { return instance; }
        }

        private string _DataDir;
        public string DataDir
        {
            get { return _DataDir; }
            set { this.RaiseAndSetIfChanged(ref _DataDir, value); }
        }

        private string _ImageDir;
        public string ImageDir
        {
            get { return _ImageDir; }
            set { this.RaiseAndSetIfChanged(ref _ImageDir, value); }
        }

        private bool _RememberLastCollection = true;
        public bool RememberLastCollection
        {
            get { return _RememberLastCollection; }
            set { this.RaiseAndSetIfChanged(ref _RememberLastCollection, value); }
        }

        private ApplicationSettings()
        {
            var base_dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (base_dir == null)
                throw new InvalidOperationException();

            var data_dir_observable = this.WhenAnyValue(x => x.DataDir);
            var image_dir_observable = this.WhenAnyValue(x => x.ImageDir);
            Observable.Merge(data_dir_observable, image_dir_observable)
                      .Subscribe(_ => EnsureDirectoriesExists());

            settings_path = Path.Combine(base_dir, filename);
            DataDir = Path.Combine(base_dir, data);
            ImageDir = Path.Combine(base_dir, image);
        }

        private void EnsureDirectoriesExists()
        {
            if (!string.IsNullOrWhiteSpace(DataDir) && !Directory.Exists(DataDir))
                Directory.CreateDirectory(DataDir);

            if (!string.IsNullOrWhiteSpace(ImageDir) && !Directory.Exists(ImageDir))
                Directory.CreateDirectory(ImageDir);
        }

        public void Load()
        {
            logger.Trace("Loading (path = {0})", settings_path);

            if (!File.Exists(settings_path))
                return;

            var settings = JsonExtensions.DeserializeFromFile<ApplicationSettings>(settings_path);

            DataDir = settings.DataDir;
            ImageDir = settings.ImageDir;
            RememberLastCollection = settings.RememberLastCollection;

        }

        public void Save()
        {
            logger.Trace("Saving (path = {0})", settings_path);

            JsonExtensions.SerializeToFile(settings_path, this);
        }




        // OLD STUFF

        //private const string key = "827A1C31-9CFA-478C-92B9-350126EC8BD3";

        //private GoodreadsSettings _GoodreadsSettings = new GoodreadsSettings();
        //public GoodreadsSettings GoodreadsSettings
        //{
        //    get { return _GoodreadsSettings; }
        //    set { this.RaiseAndSetIfChanged(ref _GoodreadsSettings, value); }
        //}

        //private GoogleBooksSettings _GoogleBooksSettings = new GoogleBooksSettings();
        //public GoogleBooksSettings GoogleBooksSettings
        //{
        //    get { return _GoogleBooksSettings; }
        //    set { this.RaiseAndSetIfChanged(ref _GoogleBooksSettings, value); }
        //}

        //private static string GetFilename()
        //{
        //    var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //    if (folder == null) 
        //        throw new InvalidOperationException();

        //    return Path.Combine(folder, "BookCollector.settings");
        //}

        //private static string GetDefaultFilename()
        //{
        //    var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //    if (folder == null)
        //        throw new InvalidOperationException();

        //    return Path.Combine(folder, "BookCollector.defaultsettings");
        //}

        //public void Load()
        //{
        //    logger.Trace("Loading");

        //    var filename = GetFilename();
        //    if (!File.Exists(filename))
        //    {
        //        logger.Trace("No settings found - loading default");

        //        filename = GetDefaultFilename();
        //        if (!File.Exists(filename))
        //            throw new Exception("Missing settings file");
        //    }

        //    var json = File.ReadAllText(filename);
        //    var data_template = new { GoodreadsSettings, GoogleBooksSettings };
        //    var data = JsonConvert.DeserializeAnonymousType(json, data_template);

        //    GoodreadsSettings = data.GoodreadsSettings.Decrypt(key);
        //    GoogleBooksSettings = data.GoogleBooksSettings.Decrypt(key);
        //}

        //public void Save()
        //{
        //    logger.Trace("Saving");

        //    var data = new
        //    {
        //        GoodreadsSettings = GoodreadsSettings.Encrypt(key),
        //        GoogleBooksSettings = GoogleBooksSettings.Encrypt(key)
        //    };
        //    var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        //    File.WriteAllText(GetFilename(), json);
        //}
    }
}
