using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Shell;
using BookCollector.Utilities;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;

namespace BookCollector.Import
{
    [Export(typeof(ImportSelectionViewModel))]
    public class ImportSelectionViewModel : ReactiveScreen
    {
        private readonly IEventAggregator event_aggregator;

        private ReactiveList<ImportControllerViewModel> _ImportControllers;
        public ReactiveList<ImportControllerViewModel> ImportControllers
        {
            get { return _ImportControllers; }
            set { this.RaiseAndSetIfChanged(ref _ImportControllers, value); }
        }

        [ImportingConstructor]
        public ImportSelectionViewModel([ImportMany] IEnumerable<IImportController> import_controllers, IEventAggregator event_aggregator)
        {
            this.event_aggregator = event_aggregator;
            ImportControllers = import_controllers.Select(i => new ImportControllerViewModel(i)).ToReactiveList();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            event_aggregator.PublishOnUIThread(ShellMessage.TextMessage("Select where to import from"));
        }
    }
}
