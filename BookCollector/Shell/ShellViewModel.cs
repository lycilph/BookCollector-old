using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using NLog;
using Panda.ApplicationCore.Shell;
using ReactiveUI;
using IScreen = Caliburn.Micro.IScreen;
using LogManager = NLog.LogManager;

namespace BookCollector.Shell
{
    [Export(typeof(IShell))]
    [Export(typeof(IBookCollectorShell))]
    public sealed class ShellViewModel : ConductorShellBase<IScreen>, IBookCollectorShell
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator event_aggregator;
        private readonly Stack<IScreen> screens = new Stack<IScreen>();

        private bool _IsEnabled = true;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { this.RaiseAndSetIfChanged(ref _IsEnabled, value); }
        }

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator event_aggregator)
        {
            this.event_aggregator = event_aggregator;

            DisplayName = "Book Collector";
            RightShellCommands.Add(new WindowCommand("Notifications", () => {}));
        }

        protected override void ChangeActiveItem(IScreen new_item, bool close_previous)
        {
            logger.Trace("Changing to screen: " + new_item.DisplayName);
            event_aggregator.PublishOnUIThread(ShellMessage.ActiveItemChanged);
            base.ChangeActiveItem(new_item, close_previous);
        }

        public void Back()
        {
            screens.Pop();
            ActivateItem(screens.Peek());
        }

        public void Show(IScreen screen)
        {
            screens.Push(screen);
            ActivateItem(screen);
        }
    }
}
