using System;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Shell;

namespace BookCollector.Domain
{
    public class ApplicationController : IApplicationController, IHandle<ApplicationMessage>
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IShellViewModel shell_view_model;
        private IShellView shell_view;

        public ApplicationController(IShellViewModel shell_view_model, IShellView shell_view)
        {
            this.shell_view_model = shell_view_model;
            this.shell_view = shell_view;
        }

        public void Initialize()
        {
            log.Info("Initializing");

            // Load settings model
            // Load application model

            // Set current theme

            // Setup shell

            // Launch shell
            shell_view.Show();
        }

        public void Handle(ApplicationMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
