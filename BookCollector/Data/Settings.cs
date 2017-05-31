using AutoMapper;
using BookCollector.Controllers;
using BookCollector.Framework.Logging;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Settings : ReactiveObject
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IDataController data_controller;

        private string _LastCollectionFilename = string.Empty;
        public string LastCollectionFilename
        {
            get { return _LastCollectionFilename; }
            set { this.RaiseAndSetIfChanged(ref _LastCollectionFilename, value); }
        }

        private bool _LoadCollectionOnStart = true;
        public bool LoadCollectionOnStart
        {
            get { return _LoadCollectionOnStart; }
            set { this.RaiseAndSetIfChanged(ref _LoadCollectionOnStart, value); }
        }

        public Settings(IDataController data_controller)
        {
            this.data_controller = data_controller;
        }

        public void Load()
        {
            log.Info("Loading settings");

            if (!data_controller.HasSettings())
            {
                log.Warn("No settings found");
                return;
            }

            var loaded_settings = data_controller.LoadSettings();
            Mapper.Map(loaded_settings, this);
        }

        public void Save()
        {
            log.Info("Saving settings");

            data_controller.SaveSettings(this);
        }
    }
}
