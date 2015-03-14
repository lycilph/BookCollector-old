using System.Collections.Generic;
using System.ComponentModel.Composition;
using BookCollector.Screens;
using Caliburn.Micro;
using NLog;
using Panda.ApplicationCore.Shell;
using ReactiveUI;
using LogManager = NLog.LogManager;

namespace BookCollector.Shell
{
    [Export(typeof(IShell))]
    [Export(typeof(IBookCollectorShell))]
    public sealed class ShellViewModel : ConductorShellBase<IBookCollectorScreen>, IBookCollectorShell
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator event_aggregator;
        private readonly Stack<IBookCollectorScreen> screens = new Stack<IBookCollectorScreen>();

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
        }

        protected override void ChangeActiveItem(IBookCollectorScreen new_item, bool close_previous)
        {
            base.ChangeActiveItem(new_item, close_previous);

            logger.Trace("Changed to screen: " + new_item.DisplayName);
            event_aggregator.PublishOnUIThread(ShellMessage.ActiveItemChanged);
        }

        public void Back()
        {
            screens.Pop();
            ActivateItem(screens.Peek());
        }

        public void Show(IBookCollectorScreen screen)
        {
            screens.Push(screen);
            ActivateItem(screen);
        }
    }
}
