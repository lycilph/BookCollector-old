using BookCollector.Data;
using BookCollector.Framework.Logging;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using BookCollector.Controllers;

namespace BookCollector.Models
{
    public class BookCollectorModel : ReactiveObject, IBookCollectorModel
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IDataController data_controller;
        private Settings settings;

        private Collection _CurrentCollection;
        public Collection CurrentCollection
        {
            get { return _CurrentCollection; }
            set { this.RaiseAndSetIfChanged(ref _CurrentCollection, value); }
        }

        public BookCollectorModel(IDataController data_controller, Settings settings)
        {
            this.data_controller = data_controller;
            this.settings = settings;

            this.WhenAnyValue(x => x.CurrentCollection)
                .Skip(1) // Skip the initial value (will be loaded from file)
                .Subscribe(x => this.settings.LastCollectionFilename = x?.Description.Filename );
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
