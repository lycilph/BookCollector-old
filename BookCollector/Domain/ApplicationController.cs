using System;
using System.Collections.Generic;
using System.Linq;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.Services.Search;
using BookCollector.Shell;
using MaterialDesignThemes.Wpf;

namespace BookCollector.Domain
{
    public class ApplicationController : IApplicationController, IHandle<ApplicationMessage>
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IApplicationModel application_model;
        private IShellFacade shell;
        private Dictionary<string, IScreen> screens;
        private IWindowCommand collection_command;
        private IWindowCommand settings_command;
        private ISearchEngine search_engine;

        public ApplicationController(IEventAggregator event_aggregator,
                                     IApplicationModel application_model, 
                                     IShellFacade shell_facade, 
                                     IScreen[] all_screens, 
                                     ISearchEngine search_engine)
        {
            this.application_model = application_model;
            this.search_engine = search_engine;
            shell = shell_facade;
            screens = all_screens.ToDictionary(s => s.DisplayName);

            event_aggregator.Subscribe(this);
        }

        public void Initialize()
        {
            log.Info("Initializing");

            // Load application model
            application_model.Load();

            // Setup shell and launch it
            SetupShell();
            SetCollectionCommandText();
            shell.Show();
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
                case ApplicationMessage.MessageKind.NavigateTo:
                    NavigateTo(message.Text);
                    break;
                case ApplicationMessage.MessageKind.CollectionChanged:
                    CollectionChanged();
                    break;
                default:
                    log.Info($"No action for message: {message.Kind}");
                    break;
            }
        }

        private void SetupShell()
        {
            log.Info("Setting up shell");

            var settings_view_model = screens[Constants.SettingsScreenDisplayName] as FlyoutBase;
            shell.AddFlyout(settings_view_model);

            collection_command = new WindowCommand(string.Empty, () => NavigateTo(Constants.CollectionsScreenDisplayName));
            shell.AddCommand(collection_command, ShellFacade.CommandPosition.Right);

            var settings_icon = new PackIcon() { Kind = PackIconKind.Settings };
            settings_command = new WindowCommand(settings_icon, () => settings_view_model.Toggle());
            shell.AddCommand(settings_command, ShellFacade.CommandPosition.Left);
        }

        private void ShellLoaded()
        {
            log.Info("Shell loaded");

            if (application_model.Settings.LoadCollectionOnStartup && application_model.CurrentCollection != null)
                NavigateTo(Constants.BooksScreenDisplayName);
            else
                NavigateTo(Constants.CollectionsScreenDisplayName);
        }

        private void ShellClosing()
        {
            log.Info("Shell closing");

            application_model.Save();
        }

        private void NavigateTo(string screen_name)
        {
            log.Info($"Navigating to {screen_name}");

            var screen = screens[screen_name];
            IScreen navigation = null;
            IScreen search = null;
            var show_collection_command = true;
            var is_fullscreen = false;
            switch (screen_name)
            {
                case Constants.CollectionsScreenDisplayName:
                    show_collection_command = false;
                    break;
                case Constants.BooksScreenDisplayName:
                    navigation = screens[Constants.NavigationScreenDisplayName];
                    search = screens[Constants.SearchScreenDisplayName];
                    break;
            }

            collection_command.IsVisible = show_collection_command;

            shell.ShowMainContent(screen, is_fullscreen);
            shell.ShowMenuContent(navigation);
            shell.ShowHeaderContent(search);
        }

        private void CollectionChanged()
        {
            log.Info("Collection changed");

            if (collection_command != null)
                SetCollectionCommandText();

            // Reindex search engine
            if (application_model.CurrentCollection != null)
                search_engine.Index(application_model.CurrentCollection.Books);
        }

        private void SetCollectionCommandText()
        {
            collection_command.DisplayName = application_model.CurrentCollection?.Description.Name ?? "[No Name]";
        }
    }
}
