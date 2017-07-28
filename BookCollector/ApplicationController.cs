using System;
using BookCollector.Data;
using BookCollector.Screens.Books;
using BookCollector.Screens.Collections;
using BookCollector.Screens.Import;
using BookCollector.Screens.Settings;
using BookCollector.Screens.Web;
using BookCollector.Services;
using MaterialDesignThemes.Wpf;
using NLog;
using ReactiveUI;

namespace BookCollector
{
    public class ApplicationController : IApplicationController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private INavigationService navigation_service;
        private ISnackbarMessageQueue message_queue;
        private ISettingsRepository settings_repository;

        public Settings Settings { get; private set; }

        public ApplicationController(INavigationService navigation_service, ISnackbarMessageQueue message_queue, ISettingsRepository settings_repository)
        {
            this.navigation_service = navigation_service;
            this.message_queue = message_queue;
            this.settings_repository = settings_repository;
        }

        public void Initialize()
        {
            logger.Trace("Initializing");

            MessageBus.Current.Listen<ApplicationMessage>()
                              .Subscribe(HandleApplicationMessage);

            Settings = settings_repository.LoadOrCreate();
        }

        public void Exit()
        {
            logger.Trace("Exiting");

            settings_repository.Save(Settings);
        }

        private void HandleApplicationMessage(ApplicationMessage message)
        {
            logger.Trace($"Got an application message {message}");

            switch (message)
            {
                case ApplicationMessage.ShellLoaded:
                    NavigateToBooksScreen();
                    break;
                case ApplicationMessage.ShowBooksScreen:
                    NavigateToBooksScreen();
                    break;
                case ApplicationMessage.ShowImportScreen:
                    NavigateToImportScreen();
                    break;
                case ApplicationMessage.ShowSettingsScreen:
                    NavigateToSettingsScreen();
                    break;
                case ApplicationMessage.ShowCollectionsScreen:
                    NavigateToCollectionsScreen();
                    break;
                case ApplicationMessage.ShowWebScreen:
                    NavigateToWebScreen();
                    break;
                default:
                    logger.Warn($"No handler for the message {message}");
                    break;
            }
        }

        private void NavigateToBooksScreen()
        {
            navigation_service.NavigateTo(typeof(IBooksScreen)); // Main content
            navigation_service.NavigateTo(typeof(ISearchScreen)); // Header content
            navigation_service.NavigateTo(typeof(INavigationScreen)); // Menu content

            message_queue.Enqueue("Welcome");
        }

        private void NavigateToCollectionsScreen()
        {
            navigation_service.NavigateTo(typeof(ICollectionsScreen));
        }

        private void NavigateToImportScreen()
        {
            navigation_service.NavigateTo(typeof(IImportScreen));
        }

        private void NavigateToSettingsScreen()
        {
            navigation_service.NavigateTo(typeof(ISettingsScreen));
        }

        private void NavigateToWebScreen()
        {
            navigation_service.NavigateTo(typeof(IWebScreen));
        }
    }
}
