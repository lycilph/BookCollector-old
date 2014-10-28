using System.ComponentModel.Composition;
using BookCollector.Services.Goodreads;
using NLog;
using ReactiveUI;

namespace BookCollector.Services
{
    [Export(typeof(ApplicationSettings))]
    public class ApplicationSettings : ReactiveObject
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private GoodreadsSettings  _GoodreadsSettings;
        public GoodreadsSettings GoodreadsSettings
        {
            get { return _GoodreadsSettings; }
            set { this.RaiseAndSetIfChanged(ref _GoodreadsSettings, value); }
        }

        public void Load()
        {
            logger.Trace("Loading");

        }

        public void Save()
        {
            logger.Trace("Saving");
            
        }
    }
}
