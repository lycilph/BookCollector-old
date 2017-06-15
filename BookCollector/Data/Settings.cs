using BookCollector.Framework.MVVM;

namespace BookCollector.Data
{
    public class Settings : DirtyTrackingBase
    {
        public bool LoadCollectionOnStartup { get; set; }
        public string PrimaryColor { get; set; }
        public string AccentColor { get; set; }
    }
}
