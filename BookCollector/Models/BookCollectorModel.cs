using BookCollector.Data;
using BookCollector.Framework.Logging;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using BookCollector.Controllers;
using BookCollector.Framework.MessageBus;
using BookCollector.Application.Messages;

namespace BookCollector.Models
{
    public class BookCollectorModel : ReactiveObject, IBookCollectorModel
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IEventAggregator event_aggregator;
        private IDataController data_controller;
        private Settings settings;

        private Collection _CurrentCollection;
        public Collection CurrentCollection
        {
            get { return _CurrentCollection; }
            set { this.RaiseAndSetIfChanged(ref _CurrentCollection, value); }
        }

        public BookCollectorModel(IEventAggregator event_aggregator, IDataController data_controller, Settings settings)
        {
            this.event_aggregator = event_aggregator;
            this.data_controller = data_controller;
            this.settings = settings;

            this.WhenAnyValue(x => x.CurrentCollection)
                .Skip(1) // Skip the initial value (will be loaded from file)
                .Subscribe(collection =>
                {
                    this.settings.LastCollectionFilename = collection?.Description.Filename;
                    this.event_aggregator.Publish(new StatusMessage(StatusMessage.MessageKind.CollectionChanged, collection.Description.Name));
                });
        }

        public void LoadAndSetCurrentCollection(string path)
        {
            log.Info($"Loading current collection {path}");

            if (!data_controller.CollectionExists(path))
            {
                log.Warn($"No collection found for {path}");
                return;
            }

            CurrentCollection = data_controller.LoadCollection(path);
        }
    }
}
