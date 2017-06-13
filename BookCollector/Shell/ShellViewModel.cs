using BookCollector.Domain;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using ReactiveUI;
using IScreen = BookCollector.Framework.MVVM.IScreen;

namespace BookCollector.Shell
{
    public class ShellViewModel : ScreenBase, IShellViewModel
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IEventAggregator event_aggregator;

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

        private IScreen _ShellContent;
        public IScreen ShellContent
        {
            get { return _ShellContent; }
            set { this.RaiseAndSetIfChanged(ref _ShellContent, value); }
        }

        public ShellViewModel(IEventAggregator event_aggregator)
        {
            this.event_aggregator = event_aggregator;

            DisplayName = Constants.ShellDisplayName;
            IsEnabled = true;
        }

        public void Show(IScreen content)
        {
            if (ShellContent == content)
                return;

            // Deactivate old content
            ShellContent?.Deactivate();
            // Activate new content
            content?.Activate();

            // Show new content
            ShellContent = content;
        }

        //public void OnViewLoaded()
        //{
        //    log.Info("ShellViewModel - view loaded");
        //    event_aggregator.Publish(ApplicationMessage.ShellLoadedMessage());
        //}

        //public void OnViewClosing()
        //{
        //    log.Info("ShellViewModel - view closing");
        //    event_aggregator.Publish(ApplicationMessage.ShellClosingMessage());
        //}
    }
}
