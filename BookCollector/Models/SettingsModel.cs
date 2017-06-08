using BookCollector.Domain;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Mapping;
using BookCollector.Framework.Messaging;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    public class SettingsModel : ReactiveObject, ISettingsModel, IHandle<ApplicationMessage>
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IDataService data_service;

        private string _LastCollectionFilename;
        public string LastCollectionFilename
        {
            get { return _LastCollectionFilename; }
            set { this.RaiseAndSetIfChanged(ref _LastCollectionFilename, value); }
        }

        private bool _LoadCollectionOnStart;
        public bool LoadCollectionOnStart
        {
            get { return _LoadCollectionOnStart; }
            set { this.RaiseAndSetIfChanged(ref _LoadCollectionOnStart, value); }
        }

        private string _PrimaryColorName;
        public string PrimaryColorName
        {
            get { return _PrimaryColorName; }
            set { this.RaiseAndSetIfChanged(ref _PrimaryColorName, value); }
        }

        private string _AccentColorName;
        public string AccentColorName
        {
            get { return _AccentColorName; }
            set { this.RaiseAndSetIfChanged(ref _AccentColorName, value); }
        }

        public SettingsModel() { }
        public SettingsModel(IEventAggregator event_aggregator, IDataService data_service)
        {
            this.data_service = data_service;

            event_aggregator.Subscribe(this);

            Reset();
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

        private void Reset()
        {
            LastCollectionFilename = string.Empty;
            LoadCollectionOnStart = true;

            var palette_helper = new PaletteHelper();
            var current_palette = palette_helper.QueryPalette();
            PrimaryColorName = current_palette.PrimarySwatch.Name;
            AccentColorName = current_palette.AccentSwatch.Name;
        }
    }
}
