using System.Collections.Generic;
using System.Linq;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.Screens;
using BookCollector.Screens.Main;
using BookCollector.Screens.Settings;
using BookCollector.Shell;

namespace BookCollector.Domain
{
    public class ApplicationController : IApplicationController, IHandle<ApplicationMessage>
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IShellViewModel shell_view_model;
        private IMainViewModel main_view_model;
        private SettingsViewModel settings_view_model;
        private Dictionary<string, IScreen> screens;
        private IWindowCommand collection_command;

        public ApplicationController(IEventAggregator event_aggregator,
                                     IShellViewModel shell_view_model, 
                                     IMainViewModel main_view_model,
                                     SettingsViewModel settings_view_model,
                                     IScreen[] screens)
        {
            this.shell_view_model = shell_view_model;
            this.main_view_model = main_view_model;
            this.settings_view_model = settings_view_model;
            this.screens = screens.ToDictionary(x => x.DisplayName);

            event_aggregator.Subscribe(this);
        }

        public void Initialize()
        {
            // Load settings
            // Load application model

            // Setup shell view model
            shell_view_model.ShellFlyouts.Add(settings_view_model);

            collection_command = new WindowCommand("[No Name]", () => NavigateTo(ScreenNames.CollectionsName));
            shell_view_model.RightShellCommands.Add(collection_command);

            var settings_command = new WindowCommand("Settings", () => settings_view_model.Toggle());
            shell_view_model.LeftShellCommands.Add(settings_command);

            // Create and show shell
            var view = new ShellView() { DataContext = shell_view_model };
            view.Show();
        }

        public void Handle(ApplicationMessage message)
        {
            log.Info("Handling message " + message.Kind);

            switch (message.Kind)
            {
                case ApplicationMessage.MessageKind.ShellLoaded:
                    ShellLoaded();
                    break;
                case ApplicationMessage.MessageKind.ShellClosing:
                    ShellClosing();
                    break;
                case ApplicationMessage.MessageKind.ToggleMainMenu:
                    main_view_model.ToggleMenu();
                    break;
                case ApplicationMessage.MessageKind.NavigateTo:
                    NavigateTo(message.ScreenName);
                    break;
            }
        }

        private void ShellLoaded()
        {
            log.Info("Showing first screen");

            NavigateTo(ScreenNames.BooksName);
        }

        private void ShellClosing()
        {
            log.Info("Closing application");

            // Save settings
            // Save application model
        }

        private void NavigateTo(string screen_name)
        {
            log.Info($"Navigating to {screen_name}");

            var screen = screens[screen_name];

            if (screen is IMainScreen)
            {
                var main_screen = screen as IMainScreen;

                // Make sure the main screen is already visible
                shell_view_model.Show(main_view_model);

                collection_command.IsVisible = main_screen.ShowCollectionCommand;
                main_view_model.Show(main_screen);
            }
            else
            {
                collection_command.IsVisible = false;
                shell_view_model.Show(screen);
            }
        }
    }
}
