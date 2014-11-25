using System.Collections.Generic;
using System.ComponentModel.Composition;
using BookCollector.Utilities;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using NLog;
using ReactiveUI;
using IScreen = Caliburn.Micro.IScreen;
using LogManager = NLog.LogManager;

namespace BookCollector.Import
{
    [Export("Import", typeof(IScreen))]
    public class ImportViewModel : ReactiveConductor<IScreen>.Collection.OneActive
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ImportSelectionViewModel selection;
        private readonly ImportInformationViewModel information;
        private readonly ImportResultsViewModel results;

        private ReactiveList<ImportStepViewModel> _Steps;
        public ReactiveList<ImportStepViewModel> Steps
        {
            get { return _Steps; }
            set { this.RaiseAndSetIfChanged(ref _Steps, value); }
        }
   
        [ImportingConstructor]
        public ImportViewModel(IEventAggregator event_aggregator, ImportSelectionViewModel selection, ImportInformationViewModel information, ImportResultsViewModel results)
        {
            this.selection = selection;
            this.information = information;
            this.results = results;

            Items.AddRange(new List<IScreen> {selection, information, results});

            event_aggregator.Subscribe(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(selection);
        }

        public void Select(ImportControllerViewModel view_model)
        {
            var controller = view_model.AssociatedObject;
            logger.Trace("Selected the {0} import controller", controller.Name);

            ActivateItem(information);

            Steps = controller.Steps.ToReactiveList();
            controller.Start();
        }
    }
}
