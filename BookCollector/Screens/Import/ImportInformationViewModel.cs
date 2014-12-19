using System.ComponentModel.Composition;
using BookCollector.Shell;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    [Export(typeof(ImportInformationViewModel))]
    public class ImportInformationViewModel : ReactiveScreen, IHandle<ImportMessage>
    {
        private readonly IEventAggregator event_aggregator;

        private ReactiveList<string> _Messages = new ReactiveList<string>();
        public ReactiveList<string> Messages
        {
            get { return _Messages; }
            set { this.RaiseAndSetIfChanged(ref _Messages, value); }
        }

        [ImportingConstructor]
        public ImportInformationViewModel(IEventAggregator event_aggregator)
        {
            this.event_aggregator = event_aggregator;
            event_aggregator.Subscribe(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Messages.Clear();

            event_aggregator.PublishOnUIThread(ShellMessage.BusyMessage(true));
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            event_aggregator.PublishOnUIThread(ShellMessage.BusyMessage(false));
        }

        public void Handle(ImportMessage message)
        {
            if (message.Kind == ImportMessage.MessageKind.Information)
                Messages.Add(message.Text);
        }
    }
}
