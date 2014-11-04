using System.ComponentModel.Composition;
using BookCollector.Shell;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;

namespace BookCollector.Main
{
    [Export("Main", typeof(IScreen))]
    public class MainViewModel : ReactiveScreen
    {
        private readonly IEventAggregator event_aggregator;
        private readonly IScreen import_view_model;

        [ImportingConstructor]
        public MainViewModel([Import("Import")] IScreen import_view_model, IEventAggregator event_aggregator)
        {
            this.import_view_model = import_view_model;
            this.event_aggregator = event_aggregator;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Import();
        }

        public void Import()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.ShowMessage(import_view_model));
        }
    }
}
