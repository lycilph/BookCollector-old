﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using BookCollector.Utilities;
using Newtonsoft.Json;
using NLog;
using ReactiveUI;

namespace BookCollector.Services.Settings
{
    [Export(typeof(ApplicationSettings))]
    [JsonObject(MemberSerialization.OptOut)]
    public sealed class ApplicationSettings : ReactiveObject
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private const string filename = "Settings.txt";
        private const string data = "Data";
        private const string image = "Images";

        private readonly string settings_path;

        [JsonProperty]
        private Dictionary<string, bool> api_enabled = new Dictionary<string, bool>();
        [JsonProperty]
        private Dictionary<string, string> api_credentials = new Dictionary<string, string>(); 

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

        public ApplicationSettings()
        {
            var base_dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (base_dir == null)
                throw new InvalidOperationException();

            this.WhenAnyValue(x => x.DataDir, x => x.ImageDir)
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
            if (!File.Exists(settings_path))
                return;

            logger.Trace("Loading (path = {0})", settings_path);
            var settings = JsonExtensions.DeserializeFromFile<ApplicationSettings>(settings_path);

            DataDir = settings.DataDir;
            ImageDir = settings.ImageDir;
            RememberLastCollection = settings.RememberLastCollection;
            api_enabled = settings.api_enabled;
            api_credentials = settings.api_credentials;
        }

        public void Save()
        {
            logger.Trace("Saving (path = {0})", settings_path);
            JsonExtensions.SerializeToFile(settings_path, this);
        }

        public T GetSettings<T>(string api_name)
        {
            var settings_filename = Assembly.GetExecutingAssembly().GetManifestResourceNames().First(n => n.Contains(api_name));
            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(settings_filename))
            using (var sr = new StreamReader(s))
            {
                var json = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        public T GetCredentials<T>(string profile_id) where T : class
        {
            return api_credentials.ContainsKey(profile_id) ? JsonConvert.DeserializeObject<T>(api_credentials[profile_id]) : null;
        }

        public void AddCredentials<T>(string profile_id, T credentials)
        {
            var json = JsonConvert.SerializeObject(credentials, Formatting.Indented);
            api_credentials[profile_id] = json;
        }

        public bool IsApiEnabled(string name)
        {
            if (!api_enabled.ContainsKey(name))
                api_enabled.Add(name, true);
            return api_enabled[name];
        }
    }
}
