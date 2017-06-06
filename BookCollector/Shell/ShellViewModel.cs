using BookCollector.Domain;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.Shell
{
    public class ShellViewModel : ReactiveObject, IShellViewModel, IViewAware
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IEventAggregator event_aggregator;

        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
        }

        private bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { this.RaiseAndSetIfChanged(ref _IsEnabled, value); }
        }

        private ReactiveList<IWindowCommand> _LeftShellCommands = new ReactiveList<IWindowCommand>();
        public ReactiveList<IWindowCommand> LeftShellCommands
        {
            get { return _LeftShellCommands; }
            set { this.RaiseAndSetIfChanged(ref _LeftShellCommands, value); }
        }

        private ReactiveList<IWindowCommand> _RightShellCommands = new ReactiveList<IWindowCommand>();
        public ReactiveList<IWindowCommand> RightShellCommands
        {
            get { return _RightShellCommands; }
            set { this.RaiseAndSetIfChanged(ref _RightShellCommands, value); }
        }

        private ReactiveList<IFlyout> _ShellFlyouts = new ReactiveList<IFlyout>();
        public ReactiveList<IFlyout> ShellFlyouts
        {
            get { return _ShellFlyouts; }
            set { this.RaiseAndSetIfChanged(ref _ShellFlyouts, value); }
        }

        public ShellViewModel(IEventAggregator event_aggregator)
        {
            this.event_aggregator = event_aggregator;

            DisplayName = "Book Collector";
            IsEnabled = true;
        }

        public void OnViewLoaded()
        {
            log.Info("ShellViewModel - view loaded");
            event_aggregator.Publish(new ApplicationMessage(ApplicationMessage.MessageKind.ShellLoaded));
        }

        public void OnViewClosing()
        {
            log.Info("ShellViewModel - view closing");
            event_aggregator.Publish(new ApplicationMessage(ApplicationMessage.MessageKind.ShellClosing));
        }
    }
}
