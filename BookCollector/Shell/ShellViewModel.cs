using BookCollector.Domain;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using MaterialDesignThemes.Wpf;
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

        private bool _IsFullscreen;
        public bool IsFullscreen
        {
            get { return _IsFullscreen; }
            set { this.RaiseAndSetIfChanged(ref _IsFullscreen, value); }
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

        private ISnackbarMessageQueue _MessageQueue;
        public ISnackbarMessageQueue MessageQueue
        {
            get { return _MessageQueue; }
            set { this.RaiseAndSetIfChanged(ref _MessageQueue, value); }
        }

        private IScreen _MainContent;
        public IScreen MainContent
        {
            get { return _MainContent; }
            set { this.RaiseAndSetIfChanged(ref _MainContent, value); }
        }

        private IScreen _MenuContent;
        public IScreen MenuContent
        {
            get { return _MenuContent; }
            set { this.RaiseAndSetIfChanged(ref _MenuContent, value); }
        }

        private IScreen _HeaderContent;
        public IScreen HeaderContent
        {
            get { return _HeaderContent; }
            set { this.RaiseAndSetIfChanged(ref _HeaderContent, value); }
        }

        public ShellViewModel(IEventAggregator event_aggregator, ISnackbarMessageQueue message_queue)
        {
            this.event_aggregator = event_aggregator;
            MessageQueue = message_queue;

            DisplayName = Constants.ShellDisplayName;
            IsEnabled = true;
            IsFullscreen = false;
        }

        public void ShowMainContent(IScreen content, bool is_fullscreen)
        {
            if (MainContent == content)
                return;

            // Deactivate old content
            MainContent?.Deactivate();
            // Activate new content
            content?.Activate();

            // Configure main content
            IsFullscreen = is_fullscreen;

            // Show new content
            MainContent = content;
        }

        public void ShowMenuContent(IScreen content)
        {
            throw new System.NotImplementedException();

            //if (MenuContent == content)
            //    return;

            //// Deactivate old content
            //MenuContent?.Deactivate();
            //// Activate new content
            //content?.Activate();

            //// Show new content
            //MenuContent = content;
        }

        public void ShowHeaderContent(IScreen content)
        {
            throw new System.NotImplementedException();

            //if (HeaderContent == content)
            //    return;

            //// Deactivate old content
            //HeaderContent?.Deactivate();
            //// Activate new content
            //content?.Activate();

            //// Show new content
            //HeaderContent = content;
        }
    }
}
