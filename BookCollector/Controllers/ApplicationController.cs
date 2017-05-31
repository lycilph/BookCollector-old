using BookCollector.Application.Messages;
using BookCollector.Data;
using BookCollector.Framework.Logging;
using BookCollector.Framework.MessageBus;
using BookCollector.Models;
using BookCollector.Screens;

namespace BookCollector.Controllers
{
    public class ApplicationController : IHandle<ApplicationMessage>, IApplicationController
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IEventAggregator event_aggregator;
        private IBookCollectorModel book_collector_model;
        private Settings settings;

        public ApplicationController(IEventAggregator event_aggregator, IBookCollectorModel book_collector_model, Settings settings)
        {
            this.event_aggregator = event_aggregator;
            this.book_collector_model = book_collector_model;
            this.settings = settings;

            event_aggregator.Subscribe(this);
        }

        public void Initialize()
        {
            log.Info("Initializing application controller");

            settings.Load();
            book_collector_model.LoadAndSetCurrentCollection(settings.LastCollectionFilename);
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

                    settings.Save();
                    // Save current collection
                    break;
            }
        }
    }
}
