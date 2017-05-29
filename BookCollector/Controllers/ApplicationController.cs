using BookCollector.Framework.Logging;
using BookCollector.Framework.MessageBus;
using BookCollector.Framework.Messages;
using BookCollector.Models;
using BookCollector.Screens;

namespace BookCollector.Controllers
{
    public class ApplicationController : IHandle<ApplicationMessage>
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IEventAggregator event_aggregator;
        private IBookCollectorModel book_collector_model;

        public ApplicationController(IEventAggregator event_aggregator, IBookCollectorModel book_collector_model)
        {
            this.event_aggregator = event_aggregator;
            this.book_collector_model = book_collector_model;
        }

        public void Initialize()
        {
            log.Info("Initializing application controller");

            event_aggregator.Subscribe(this);

            // Load settings
        }

        public void Handle(ApplicationMessage message)
        {
            log.Info("Handling message " + message.Kind);

            switch (message.Kind)
            {
                case ApplicationMessage.MessageKind.ShellLoaded:
                    log.Info("Showing start screen");
                    event_aggregator.Publish(new NavigationMessage(ScreenNames.StartScreenName));
                    break;
                case ApplicationMessage.MessageKind.ShellClosing:
                    log.Info("Closing application");
                    // Save settings
                    // Save current collection
                    break;
            }
        }
    }
}
