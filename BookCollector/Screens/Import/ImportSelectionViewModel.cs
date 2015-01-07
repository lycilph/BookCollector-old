using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Apis;
using BookCollector.Shell;
using BookCollector.Utilities;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    [Export(typeof(ImportSelectionViewModel))]
    public class ImportSelectionViewModel : ReactiveScreen
    {
        private readonly IEventAggregator event_aggregator;
        private readonly ApiController api_controller;

        private ReactiveList<ImportControllerViewModel> _ImportControllers;
        public ReactiveList<ImportControllerViewModel> ImportControllers
        {
            get { return _ImportControllers; }
            set { this.RaiseAndSetIfChanged(ref _ImportControllers, value); }
        }

        [ImportingConstructor]
        public ImportSelectionViewModel(IEventAggregator event_aggregator, ApiController api_controller)
        {
            this.event_aggregator = event_aggregator;
            this.api_controller = api_controller;
        }

        protected override void OnActivate()
        {
            event_aggregator.PublishOnUIThread(ShellMessage.Text("Select where to import from"));

            ImportControllers = api_controller.GetImportControllers()
                                              .Select(i => new ImportControllerViewModel(i))
                                              .ToReactiveList();
        }
    }
}
