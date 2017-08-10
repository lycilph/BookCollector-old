using Core.Utility;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Settings : DirtyTrackingBase
    {
        private bool _LoadCollectionOnStartup;
        public bool LoadCollectionOnStartup
        {
            get { return _LoadCollectionOnStartup; }
            set { this.RaiseAndSetIfChanged(ref _LoadCollectionOnStartup, value); }
        }

        private string _LastCollectionFilename;
        public string LastCollectionFilename
        {
            get { return _LastCollectionFilename; }
            set { this.RaiseAndSetIfChanged(ref _LastCollectionFilename, value); }
        }

        private string _PrimaryColor;
        public string PrimaryColor
        {
            get { return _PrimaryColor; }
            set { this.RaiseAndSetIfChanged(ref _PrimaryColor, value); }
        }

        private string _AccentColor;
        public string AccentColor
        {
            get { return _AccentColor; }
            set { this.RaiseAndSetIfChanged(ref _AccentColor, value); }
        }

        private int _ShelfMappingThreshold;
        public int ShelfMappingThreshold
        {
            get { return _ShelfMappingThreshold; }
            set { this.RaiseAndSetIfChanged(ref _ShelfMappingThreshold, value); }
        }
    }
}
