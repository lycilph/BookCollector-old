﻿using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Models;
using BookCollector.Services;
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

        public ApplicationController(IEventAggregator event_aggregator, IApplicationModel application_model, IShellFacade shell, INavigationService navigation_service)
        {
            this.event_aggregator = event_aggregator;
            this.application_model = application_model;
            this.shell = shell;
            this.navigation_service = navigation_service;

            event_aggregator.Subscribe(this);
        }

        public void Initialize()
        {
            log.Info("Initializing");

            // Load application model
            application_model.Load();

            // Initialize and launch shell
            shell.Initialize();
            shell.ShowShell();
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
                default:
                    log.Warn($"No action for message: {message.Kind}");
                    break;
            }
        }

        private void ShellLoaded()
        {
            log.Info("Shell loaded, navigating to start screen");

            navigation_service.NavigateTo(Constants.ImportScreenDisplayName);
        }

        private void ShellClosing()
        {
            log.Info("Shell closing, saving state");

            application_model.Save();
        }
    }
}
