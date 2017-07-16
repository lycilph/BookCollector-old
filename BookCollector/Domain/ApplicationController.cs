using System.Linq;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Models;
using BookCollector.Services;
using BookCollector.Services.Search;
using BookCollector.Shell;

namespace BookCollector.Domain
{
    public class ApplicationController : IApplicationController, IHandle<ShellMessages>, IHandle<ApplicationMessage>
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IEventAggregator event_aggregator;
        private IApplicationModel application_model;
        private IShellFacade shell;
        private INavigationService navigation_service;
        private ISearchEngine search_engine;

        public ApplicationController(IEventAggregator event_aggregator, IApplicationModel application_model, IShellFacade shell, INavigationService navigation_service, ISearchEngine search_engine)
        {
            this.event_aggregator = event_aggregator;
            this.application_model = application_model;
            this.shell = shell;
            this.navigation_service = navigation_service;
            this.search_engine = search_engine;

            event_aggregator.Subscribe(this);
        }

        public void Initialize()
        {
            log.Info("Initializing");

            // Load application model
            application_model.Load();

            // Initialize the navigation service
            navigation_service.Initialize();

            // Initialize and launch shell
            shell.Initialize();
        }

        public void Handle(ShellMessages message)
        {
            log.Info("Handling shell message " + message.Kind);

            switch (message.Kind)
            {
                case ShellMessages.MessageKind.ShellLoaded:
                    ShellLoaded();
                    break;
                case ShellMessages.MessageKind.ShellClosing:
                    ShellClosing();
                    break;
                default:
                    log.Warn($"No action for message: {message.Kind}");
                    break;
            }
        }

        public void Handle(ApplicationMessage message)
        {
            log.Info("Handling shell message " + message.Kind);

            switch (message.Kind)
            {
                case ApplicationMessage.MessageKind.NavigateTo:
                    navigation_service.NavigateTo(message.Text);
                    break;
                case ApplicationMessage.MessageKind.CollectionChanged:
                    UpdateSearchIndex();
                    UpdateCollectionCommandText();
                    UpdateLastCollectionFilename();
                    break;
                default:
                    log.Warn($"No action for message: {message.Kind}");
                    break;
            }
        }

        private void ShellLoaded()
        {
            log.Info("Shell loaded, navigating to start screen");

            if (application_model.CollectionModel.CurrentCollection == null)
                navigation_service.NavigateTo(Constants.CollectionsScreenDisplayName);
            else
                navigation_service.NavigateTo(Constants.BooksScreenDisplayName);
        }

        private void ShellClosing()
        {
            log.Info("Shell closing, saving state");

            application_model.Save();
        }

        private void UpdateSearchIndex()
        {
            log.Info("Updating search index");

            // Reindex search engine
            if (application_model.CollectionModel.CurrentCollection != null)
                search_engine.Index(application_model.CollectionModel.CurrentCollection.DefaultShelf.Books.ToList());
        }

        public void UpdateCollectionCommandText()
        {
            log.Info("Updating collection command text");

            var collection = application_model.CollectionModel.CurrentCollection;
            var text = (collection == null ? "[NA]" : collection.Description.Name);

            shell.SetCollectionCommandText(text);
        }

        private void UpdateLastCollectionFilename()
        {
            log.Info("Updating last collection filename");

            var collection = application_model.CollectionModel.CurrentCollection;
            var settings = application_model.SettingsModel.Settings;

            settings.LastCollectionFilename = (collection == null ? string.Empty : collection.Description.Filename);
        }
    }
}
