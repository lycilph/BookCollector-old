using System.Collections.Generic;
using System.ComponentModel.Composition;
using BookCollector.Services;
using BookCollector.Shell;
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

        private readonly IEventAggregator event_aggregator;
        private readonly ImportSelectionStepViewModel selection_step;
        private readonly ImportAuthenticationStepViewModel authentication_step;
        private readonly ImportResultsStepViewModel results_step;

        public IReactiveDerivedList<ImportStepViewModel> Steps { get; set; }
        
        [ImportingConstructor]
        public ImportViewModel(IEventAggregator event_aggregator)
        {
            this.event_aggregator = event_aggregator;

            selection_step = new ImportSelectionStepViewModel(this);
            authentication_step = new ImportAuthenticationStepViewModel(this);
            results_step = new ImportResultsStepViewModel(this);

            Items.AddRange(new List<IScreen> {selection_step, authentication_step, results_step});

            Steps = Items.CreateDerivedCollection(i => new ImportStepViewModel(i));
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(selection_step);
        }

        public void Import(IApi api)
        {
            logger.Trace("Importing (api = {0})", api.Name);

            if (api.IsAuthenticated)
            {
                results_step.Setup(api);
                ActivateItem(results_step);                
            }
            else
            {
                authentication_step.Setup(api);
                ActivateItem(authentication_step);
            }
        }

        public void Done()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.BackMessage());
        }
    }
}
