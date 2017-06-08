using BookCollector.Domain;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Mapping;
using BookCollector.Framework.Messaging;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    public class SettingsModel : ReactiveObject, ISettingsModel, IHandle<ApplicationMessage>
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IDataService data_service;

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

        public SettingsModel() { }
        public SettingsModel(IEventAggregator event_aggregator, IDataService data_service)
        {
            this.data_service = data_service;

            event_aggregator.Subscribe(this);
        }

        public void Load()
        {
            log.Info("Loading settings");

            if (!data_service.SettingsExists())
            {
                log.Warn("No settings found");
                return;
            }

            var loaded_settings = data_service.LoadSettings();
            Mapper.Map(loaded_settings, this);
        }

        public void Save()
        {
            log.Info("Saving settings");

            data_service.SaveSettings(this);
        }

        public void Handle(ApplicationMessage message)
        {
            if (message.Kind == ApplicationMessage.MessageKind.CollectionChanged)
                LastCollectionFilename = message.Text;
        }
    }
}
