using System;
using System.Collections.Generic;
using System.Linq;
using BookCollector.Data;
using BookCollector.Domain;
using BookCollector.Framework.Extensions;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using ReactiveUI;

namespace BookCollector.Models
{
    public class CollectionModel : ReactiveObject, ICollectionModel
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IEventAggregator event_aggregator;

        private Collection _CurrentCollection;
        public Collection CurrentCollection
        {
            get { return _CurrentCollection; }
            set { this.RaiseAndSetIfChanged(ref _CurrentCollection, value); }
        }

        public CollectionModel(IEventAggregator event_aggregator)
        {
            this.event_aggregator = event_aggregator;
        }

        public void Load()
        {
        }

        public void Save()
        {
        }

        public Collection CreateDefaultCollection()
        {
            var collection_description = new Description(Constants.DefaultCollectionName, Constants.DefaultCollectionDescription);
            var default_shelf = new Shelf(Constants.DefaultShelfName, Constants.DefaultShelfDescription, true);
            var collection = new Collection(collection_description, default_shelf);
            return collection;
        }

        public List<SimilarityInformation> CalculateBookSimilarities(List<Book> c1, List<Book> c2)
        {
            List<SimilarityInformation> result = new List<SimilarityInformation>();
            foreach (var book in c1)
            {
                var duplicates = c2.Where(b => b.Title.Equals(book.Title) ||
                                               b.Authors.SequenceEqual(book.Authors) ||
                                               b.ISBN10.Equals(book.ISBN10) ||
                                               b.ISBN13.Equals(book.ISBN13));
                var similarities = duplicates.Select(b =>
                {
                    var similarity_information = new SimilarityInformation(book);
                    if (b.Title.Equals(book.Title))
                        similarity_information.Add(25, "Title", "T");
                    if (b.Authors.SequenceEqual(b.Authors))
                        similarity_information.Add(25, "Authors", "A");
                    if (!string.IsNullOrWhiteSpace(b.ISBN10) && b.ISBN10.Equals(book.ISBN10))
                        similarity_information.Add(25, "ISBN10", "10");
                    if (!string.IsNullOrWhiteSpace(b.ISBN13) && b.ISBN13.Equals(book.ISBN13))
                        similarity_information.Add(25, "ISBN13", "13");
                    similarity_information.Cleanup();
                    return similarity_information;
                })
                .OrderByDescending(s => s.Score);

                if (similarities.Any())
                    result.Add(similarities.First());
                else
                    result.Add(new SimilarityInformation(book));
            }
            return result;
        }

        public void Import(List<Book> books)
        {
            log.Info($"Adding {books.Count} books to current collection");

            // Flatten list into (book, shelf) pairs
            var pairs = books.SelectMany(b => b.Shelves.Select(s => new { book = b, shelf = s }));
            // Group into (shelf, list of books)
            var shelf_groups = pairs.GroupBy(p => p.shelf, p => p.book).ToList();

            // Clear shelves in list to import
            books.Apply(b => b.Shelves.Clear());
            shelf_groups.Apply(g => g.Key.Books.Clear());

            // Add books to shelves in the current collection
            foreach (var group in shelf_groups)
            {
                // If a shelf with this name already exists, use this
                var shelf = CurrentCollection.Shelves.SingleOrDefault(s => s.Name.Equals(group.Key.Name, StringComparison.InvariantCultureIgnoreCase));
                if (shelf == null)
                {
                    // This is a new shelf, so add it to the current collection
                    shelf = group.Key;
                    CurrentCollection.Add(shelf);
                }
                // Add all books to shelf
                group.Apply(b => shelf.Add(b));
            }

            // Add books to default shelf
            books.Apply(b => CurrentCollection.DefaultShelf.Add(b));

            // Fire collection changed message which: reindex' search engine, updates collection command
            event_aggregator.Publish(ApplicationMessage.CollectionChanged());
        }
    }
}