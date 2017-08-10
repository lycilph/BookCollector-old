using System;
using Core.Shell;
using MaterialDesignThemes.Wpf;
using NLog;
using ReactiveUI;
using IScreen = Core.Shell.IScreen;

namespace BookCollector.Screens.Shell
{
    public class ShellViewModel : ShellBase, IShellViewModel
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public IWindowCommand SettingsCommand { get; private set; }
        public IWindowCommand CollectionCommand { get; private set; }

        private bool _IsFullscreen;
        public bool IsFullscreen
        {
            get { return _IsFullscreen; }
            set { this.RaiseAndSetIfChanged(ref _IsFullscreen, value); }
        }

        private bool _IsMenuOpen;
        public bool IsMenuOpen
        {
            get { return _IsMenuOpen; }
            set { this.RaiseAndSetIfChanged(ref _IsMenuOpen, value); }
        }

        private IScreen _MainContent;
        public IScreen MainContent
        {
            get { return _MainContent; }
            set { this.RaiseAndSetIfChanged(ref _MainContent, value); }
        }

        private IScreen _HeaderContent;
        public IScreen HeaderContent
        {
            get { return _HeaderContent; }
            set { this.RaiseAndSetIfChanged(ref _HeaderContent, value); }
        }

        private IScreen _MenuContent;
        public IScreen MenuContent
        {
            get { return _MenuContent; }
            set { this.RaiseAndSetIfChanged(ref _MenuContent, value); }
        }

        public ShellViewModel(ISnackbarMessageQueue message_queue)
        {
            MessageQueue = message_queue;

            Initialize();
        }

        private void Initialize()
        {
            var settings_icon = new PackIcon() { Kind = PackIconKind.Settings };
            SettingsCommand = new WindowCommand(settings_icon, () => MessageBus.Current.SendMessage(ApplicationMessage.ShowSettingsScreen));
            LeftShellCommands.Add(SettingsCommand);

            CollectionCommand = new WindowCommand("[Collection]", () => MessageBus.Current.SendMessage(ApplicationMessage.ShowCollectionsScreen));
            RightShellCommands.Add(CollectionCommand);

            // Handle activation/deactivation of menu content
            this.WhenAnyValue(x => x.IsMenuOpen)
                .Subscribe(open =>
                {
                    if (open)
                        MenuContent?.Activate();
                    else
                        MenuContent?.Deactivate();
                });
        }

        protected override void OnViewLoaded(object view)
        {
            logger.Trace("Shell is loaded");

            MessageBus.Current.SendMessage(ApplicationMessage.ShellLoaded);
        }

        public void Show(IScreen screen, ShellScreenPosition position)
        {
            switch (position)
            {
                case ShellScreenPosition.MainContent:
                    ShowMainContent(screen);
                    ShowHeaderContent(null);
                    ShowMenuContent(null);
                    break;
                case ShellScreenPosition.HeaderContent:
                    ShowHeaderContent(screen);
                    break;
                case ShellScreenPosition.MenuContent:
                    ShowMenuContent(screen);
                    break;
            }
        }

        private void ShowMainContent(IScreen screen)
        {
            if (MainContent == screen)
                return;

            MainContent?.Deactivate();
            MainContent = screen;
            MainContent?.Activate();

            IsMenuOpen = false;
        }

        private void ShowHeaderContent(IScreen screen)
        {
            if (HeaderContent == screen)
                return;

            HeaderContent?.Deactivate();
            HeaderContent = screen;
            HeaderContent?.Activate();
        }

        private void ShowMenuContent(IScreen screen)
        {
            if (MenuContent == screen)
                return;

            // Activation/deactivation is handled through the IsMenuOpen property
            MenuContent = screen;
        }
    }
}
