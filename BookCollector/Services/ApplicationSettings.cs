﻿using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using BookCollector.Services.Goodreads;
using Newtonsoft.Json;
using NLog;
using ReactiveUI;

namespace BookCollector.Services
{
    [Export(typeof(ApplicationSettings))]
    public class ApplicationSettings : ReactiveObject
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private const string key = "827A1C31-9CFA-478C-92B9-350126EC8BD3";

        private GoodreadsSettings  _GoodreadsSettings = new GoodreadsSettings();
        public GoodreadsSettings GoodreadsSettings
        {
            get { return _GoodreadsSettings; }
            set { this.RaiseAndSetIfChanged(ref _GoodreadsSettings, value); }
        }

        public EventHandler Loaded = (sender, args) => { };
        
        private static string GetFilename()
        {
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(folder, "BookCollector.settings");
        }

        private static string GetDefaultFilename()
        {
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(folder, "BookCollector.defaultsettings");
        }

        public void Load()
        {
            logger.Trace("Loading");

            var filename = GetFilename();
            if (!File.Exists(filename))
            {
                logger.Trace("No settings found - loading default");

                filename = GetDefaultFilename();
                if (!File.Exists(filename))
                    throw new Exception("Missing settings file");
            }

            var json = File.ReadAllText(filename);
            var data_template = new { GoodreadsSettings };
            var data = JsonConvert.DeserializeAnonymousType(json, data_template);

            GoodreadsSettings = data.GoodreadsSettings.Decrypt(key);

            Loaded(this, new EventArgs());
        }

        public void Save()
        {
            logger.Trace("Saving");

            var encrypted_goodreads_settings = GoodreadsSettings.Encrypt(key);

            var data = new { GoodreadsSettings = encrypted_goodreads_settings };
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(GetFilename(), json);
        }
    }
}
