using System;
using System.Collections.Generic;
using System.Linq;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.Shell;

namespace BookCollector.Domain
{
    public class ApplicationController : IApplicationController, IHandle<ApplicationMessage>
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IApplicationModel application_model;
        private IShellFacade shell;
        private Dictionary<string, IScreen> screens;

        public ApplicationController(IEventAggregator event_aggregator, IApplicationModel application_model, IShellFacade shell_facade, IScreen[] all_screens)
        {
            this.application_model = application_model;
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
            //SetupShell();
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
            }
        }

        private void SetupShell()
        {
            throw new NotImplementedException();

            /*
             * Add window commands
             * Add settings flyout
             */
        }

        private void ShellLoaded()
        {
            log.Info("Shell loaded");

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
            shell.ShowMainContent(screen);
        }
    }
}
