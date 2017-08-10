using System.Windows;
using MaterialDesignThemes.Wpf;
using NLog;
using ReactiveUI;

namespace Core.Shell
{
    public class ShellBase : ScreenBase, IShell, IViewAware
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

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

        void IViewAware.AttachView(object view)
        {
            logger.Trace($"Attaching view {view.GetType().Name}");

            var fe = view as FrameworkElement;
            if (fe == null)
            {
                logger.Warn($"View must derived from FrameworkElement");
                return;
            }

            // Attach loaded event handler
            RoutedEventHandler loaded = null;
            loaded = (s, e) => 
            {
                fe.Loaded -= loaded;
                OnViewLoaded(view);
            };
            fe.Loaded += loaded;

            // Attach unloaded event handler
            RoutedEventHandler unloaded = null;
            unloaded = (s, e) =>
            {
                fe.Unloaded -= unloaded;
                OnViewUnloaded(view);
            };
            fe.Unloaded += unloaded;
        }

        protected virtual void OnViewLoaded(object view) { }

        protected virtual void OnViewUnloaded(object view) { }
    }
}
