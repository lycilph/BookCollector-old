using System.ComponentModel.Composition;
using BookCollector.Services;
using BookCollector.Services.Goodreads;
using BookCollector.Shell;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Import
{
    [Export("Import", typeof(IScreen))]
    public class ImportViewModel : ReactiveScreen
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator event_aggregator;
        private readonly ApplicationSettings settings;
        private readonly GoodreadsApi goodreads_api;

        [ImportingConstructor]
        public ImportViewModel(ApplicationSettings settings, GoodreadsApi goodreads_api, IEventAggregator event_aggregator)
        {
            this.settings = settings;
            this.goodreads_api = goodreads_api;
            this.event_aggregator = event_aggregator;
        }

        protected override void OnViewLoaded(object view)
        {
            logger.Trace("View loaded");

            base.OnViewLoaded(view);

            ImportFromGoodreads();
        }

        public void ImportFromGoodreads()
        {
            logger.Trace("ImportFromGoodreads");

            IScreen vm = new GoodreadsImportViewModel(goodreads_api);
            if (!settings.GoodreadsSettings.IsAuthenticated)
                vm = new GoodreadsAuthenticateViewModel(goodreads_api, vm);
            event_aggregator.PublishOnCurrentThread(ShellMessage.ShowMessage(vm));
        }
    }
}
