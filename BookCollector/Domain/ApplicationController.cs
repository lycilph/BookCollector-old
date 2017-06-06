using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Shell;

namespace BookCollector.Domain
{
    public class ApplicationController : IApplicationController, IHandle<ApplicationMessage>
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IShellViewModel shell_view_model;

        public ApplicationController(IEventAggregator event_aggregator, IShellViewModel shell_view_model)
        {
            this.shell_view_model = shell_view_model;

            event_aggregator.Subscribe(this);
        }

        public void Initialize()
        {
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
            }
        }

        private void ShellLoaded()
        {
            log.Info("Showing first screen");
        }

        private void ShellClosing()
        {
            log.Info("Closing application");
        }
    }
}
