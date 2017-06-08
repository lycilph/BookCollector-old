using System.Collections.Generic;
using System.Linq;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.Models;
using BookCollector.Screens;
using BookCollector.Screens.Main;
using BookCollector.Screens.Settings;
using BookCollector.Shell;
using MaterialDesignThemes.Wpf;

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
        private IWindowCommand settings_command;
        private IApplicationModel application_model;
        private ISettingsModel settings_model;

        public ApplicationController(IEventAggregator event_aggregator,
                                     IShellViewModel shell_view_model, 
                                     IMainViewModel main_view_model,
                                     SettingsViewModel settings_view_model,
                                     IScreen[] screens,
                                     IApplicationModel application_model,
                                     ISettingsModel settings_model)
        {
            this.shell_view_model = shell_view_model;
            this.main_view_model = main_view_model;
            this.settings_view_model = settings_view_model;
            this.screens = screens.ToDictionary(x => x.DisplayName);
            this.application_model = application_model;
            this.settings_model = settings_model;

            collection_command = new WindowCommand("[No Name]", () => NavigateTo(ScreenNames.CollectionsName));

            var settings_icon = new PackIcon() { Kind = PackIconKind.Settings };
            settings_command = new WindowCommand(settings_icon, () => settings_view_model.Toggle());

            event_aggregator.Subscribe(this);
        }

        public void Initialize()
        {
            // Load settings
            settings_model.Load();

            // Load application model
            if (settings_model.LoadCollectionOnStart && !string.IsNullOrEmpty(settings_model.LastCollectionFilename))
                application_model.LoadCurrentCollection(settings_model.LastCollectionFilename);

            // Setup shell
            shell_view_model.ShellFlyouts.Add(settings_view_model);
            shell_view_model.RightShellCommands.Add(collection_command);
            shell_view_model.LeftShellCommands.Add(settings_command);

            // Create and show shell view
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
                    NavigateTo(message.Text);
                    break;
                case ApplicationMessage.MessageKind.CollectionChanged:
                    UpdateCollectionName();
                    break;
            }
        }
        
        private void ShellLoaded()
        {
            log.Info("Showing first screen");

            if (settings_model.LoadCollectionOnStart && application_model.CurrentCollection != null)
                NavigateTo(ScreenNames.BooksName);
            else
                NavigateTo(ScreenNames.CollectionsName);
        }

        private void ShellClosing()
        {
            log.Info("Closing application");

            // Save settings
            settings_model.Save();

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

        private void UpdateCollectionName()
        {
            collection_command.DisplayName = (application_model.CurrentCollection != null ? application_model.CurrentCollection.Description.Name : "[No Name]");
        }
    }
}
