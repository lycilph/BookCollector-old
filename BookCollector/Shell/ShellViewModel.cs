using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using BookCollector.Application.Messages;
using BookCollector.Framework.Logging;
using BookCollector.Framework.MessageBus;
using BookCollector.Screens;
using ReactiveUI;

namespace BookCollector.Shell
{
    public class ShellViewModel : ReactiveObject, IViewAware, IHandle<NavigationMessage>, IHandle<StatusMessage>
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IEventAggregator event_aggregator;
        private List<IShellScreen> shell_screens;

        private WindowCommand tools_window_command;

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

            IsEnabled = true;
            UpdateTitle();

            event_aggregator.Subscribe(this);

            tools_window_command = new WindowCommand("Tools", () => ShellContent.Tools.Toggle());
            RightShellCommands.Add(tools_window_command);

            this.WhenAnyValue(x => x.ShellContent)
                .Where(screen => screen != null)
                .Subscribe(screen => 
                {
                    tools_window_command.IsVisible = screen.Tools != null;

                    ShellFlyouts.Clear();
                    if (screen.Tools != null)
                        ShellFlyouts.Add(screen.Tools);
                });
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

        public void Handle(StatusMessage message)
        {
            log.Info("Handling status message " + message.Kind);

            switch (message.Kind)
            {
                case StatusMessage.MessageKind.CollectionChanged:
                    UpdateTitle(message.Name);
                    break;
                default:
                    throw new InvalidOperationException("Unknown status message kind " + message.Kind);
            }
        }

        private void UpdateTitle(string text = "")
        {
            DisplayName = ScreenNames.ShellName + (string.IsNullOrWhiteSpace(text) ? "" : " - " + text);
        }
    }
}
