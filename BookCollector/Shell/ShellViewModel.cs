using System;
using System.Collections.Generic;
using System.Linq;
using BookCollector.Framework.Logging;
using BookCollector.Framework.MessageBus;
using BookCollector.Framework.Messages;
using BookCollector.Screens;
using ReactiveUI;

namespace BookCollector.Shell
{
    public class ShellViewModel : ReactiveObject, IViewAware, IHandle<NavigationMessage>
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IEventAggregator event_aggregator;
        private List<IShellScreen> shell_screens;

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

        private IShellScreen _ShellContent;
        public IShellScreen ShellContent
        {
            get { return _ShellContent; }
            set { this.RaiseAndSetIfChanged(ref _ShellContent, value); }
        }

        public ShellViewModel(IEventAggregator event_aggregator, IShellScreen[] screens)
        {
            log.Info("ShellViewModel created");

            this.event_aggregator = event_aggregator;
            shell_screens = screens.ToList();

            DisplayName = ScreenNames.ShellName;
            IsEnabled = true;

            event_aggregator.Subscribe(this);

            //var t = new ToolsViewModel();
            //ShellFlyouts.Add(t);
            //RightShellCommands.Add(new WindowCommand("Tools", () => t.Toggle()));
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

        public void Handle(NavigationMessage message)
        {
            log.Info("Navigating to " + message.ScreenName);

            var screen = shell_screens.SingleOrDefault(x => x.DisplayName == message.ScreenName);
            if (screen == null)
                throw new ArgumentException($"Screen {message.ScreenName} not found");

            screen.Activate();
            ShellContent = screen;
        }
    }
}
