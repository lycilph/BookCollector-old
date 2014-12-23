using System.ComponentModel.Composition;
using BookCollector.Services.Collections;
using System;
using BookCollector.Utilities;
using NLog;
using ReactiveUI;

namespace BookCollector.Services.Books
{
    [Export(typeof(CollectionController))]
    public class CollectionController : IPersistable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private CollectionDescription current;

        private readonly BookRepository book_repository;

        [ImportingConstructor]
        public CollectionController(CollectionsController collections_controller, BookRepository book_repository)
        {
            this.book_repository = book_repository;

            collections_controller.WhenAnyValue(x => x.Current)
                                  .Subscribe(OnCollectionChanged);
        }

        private void OnCollectionChanged(CollectionDescription collection)
        {
            if (current != null)
                Save(current);

            if (collection != null)
                Load(collection);
        }

        private void Load(CollectionDescription collection)
        {
            logger.Trace("Loading collection " + collection);
            current = collection;
            book_repository.Load();
        }

        private void Save(CollectionDescription collection)
        {
            logger.Trace("Saving collection " + collection);
            // Save books
        }

        public void Load()
        {
            // Not needed
        }

        public void Save()
        {
            Save(current);
        }
    }
}
