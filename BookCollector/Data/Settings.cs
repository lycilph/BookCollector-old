using BookCollector.Framework.MVVM;
using Newtonsoft.Json;

namespace BookCollector.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Settings : DirtyTrackingBase
    {
        public bool LoadCollectionOnStartup { get; set; }
        public string LastCollectionFilename { get; set; }
        public string PrimaryColor { get; set; }
        public string AccentColor { get; set; }
    }
}
