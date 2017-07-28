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
        private ISettingsService settings_service;

        public ApplicationController(INavigationService navigation_service, ISnackbarMessageQueue message_queue, ISettingsService settings_service)
        {
            this.navigation_service = navigation_service;
            this.message_queue = message_queue;
            this.settings_service = settings_service;
        }

        public void Initialize()
        {
            logger.Trace("Initializing");

            MessageBus.Current.Listen<ApplicationMessage>()
                              .Subscribe(HandleApplicationMessage);

            settings_service.Initialize();
        }

        public void Exit()
        {
            logger.Trace("Exiting");

            settings_service.Exit();
        }

        private void HandleApplicationMessage(ApplicationMessage message)
        {
            logger.Trace($"Got an application message {message}");

            switch (message)
            {
                case ApplicationMessage.ShellLoaded:
                    HandleShellLoaded();
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

        private void HandleShellLoaded()
        {
            NavigateToBooksScreen();

            message_queue.Enqueue("Welcome to Book Collector");
        }

        private void NavigateToBooksScreen()
        {
            navigation_service.NavigateTo(typeof(IBooksScreen)); // Main content
            navigation_service.NavigateTo(typeof(ISearchScreen)); // Header content
            navigation_service.NavigateTo(typeof(INavigationScreen)); // Menu content
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
