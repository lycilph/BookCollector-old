using System;
using System.Collections.Generic;
using System.Linq;
using BookCollector.Data;
using BookCollector.Data.Import;
using Core.Extensions;
using Core.Mapping;
using Core.Utility;

namespace BookCollector.Services
{
    public class ImportService : IImportService
    {
        //private const int shelf_mapping_threshold = 3;

        private ICollectionsService collections_service;
        private ISettingsService settings_service;

        public ImportService(ICollectionsService collections_service, ISettingsService settings_service)
        {
            this.collections_service = collections_service;
            this.settings_service = settings_service;
        }

        public void Import(List<ImportedBook> books_to_import, Dictionary<string, Shelf> shelf_mapping)
        {
            // Make a Book to ImportedBook mapping
            var book_mapping = books_to_import.ToDictionary(b => Mapper.Map<Book>(b));
            var books = book_mapping.Keys.ToList();

            // Add a history entry
            var time_stamp = DateTime.Now.ToShortDateString();
            books.Apply(b => b.History.Add($"Imported on the {time_stamp}"));

            // Add to default shelf
            collections_service.Current.DefaultShelf.Add(books);

            // Add books to mapped shelves
            foreach (var book in books)
            {
                foreach (var import_shelf in book_mapping[book].Shelves)
                {
                    // Map shelf in ImportedBooks (which is a string) to a shelf in the current collection
                    var shelf = shelf_mapping[import_shelf];

                    if (!shelf.IsDefault)
                        shelf.Add(book);
                }
            }
        }

        public Shelf Map(string imported_shelf)
        {
            var collection = collections_service.Current;
            var edit_distances = collection.Shelves.Select(s => new { Shelf = s, EditDistance = StringMetrics.EditDistance(imported_shelf, s.Name) })
                                                   .OrderBy(p => p.EditDistance);
            var closest = edit_distances.First();
            var shelf_mapping_threshold = settings_service.Settings.ShelfMappingThreshold;
            if (closest.EditDistance < shelf_mapping_threshold)
                return closest.Shelf;
            else
                return collection.DefaultShelf;
        }

        public void GetSimilarity(ImportedBook imported_book)
        {
            var books_in_collection = collections_service.Current.Books;
            var possible_duplicates = books_in_collection.Where(b => IsSimilar(b, imported_book));
            var similarities = possible_duplicates.Select(b => CalculateSimilarity(b, imported_book));

            imported_book.Similarity = similarities.OrderByDescending(s => s.Score)
                                                   .DefaultIfEmpty(new SimilarityInformation(null))
                                                   .First();
        }

        private bool IsSimilar(Book book, ImportedBook imported_book)
        {
            return book.Title.Equals(imported_book.Title, StringComparison.InvariantCultureIgnoreCase) ||
                   (!string.IsNullOrWhiteSpace(book.ISBN10) && book.ISBN10.Equals(imported_book.ISBN10, StringComparison.InvariantCultureIgnoreCase)) ||
                   (!string.IsNullOrWhiteSpace(book.ISBN13) && book.ISBN13.Equals(imported_book.ISBN13, StringComparison.InvariantCultureIgnoreCase)) ||
                   book.Authors.SequenceEqual(imported_book.Authors, StringComparer.InvariantCultureIgnoreCase);
        }

        private SimilarityInformation CalculateSimilarity(Book book, ImportedBook imported_book)
        {
            var similarity_information = new SimilarityInformation(book);

            if (book.Title.Equals(imported_book.Title, StringComparison.InvariantCultureIgnoreCase))
                similarity_information.Add(25, "Title", "T");

            if (book.Authors.SequenceEqual(imported_book.Authors, StringComparer.InvariantCultureIgnoreCase))
                similarity_information.Add(25, "Authors", "A");

            if (book.ISBN10.Equals(imported_book.ISBN10, StringComparison.InvariantCultureIgnoreCase))
                similarity_information.Add(25, "ISBN10", "10");

            if (book.ISBN13.Equals(imported_book.ISBN13, StringComparison.InvariantCultureIgnoreCase))
                similarity_information.Add(25, "ISBN13", "13");

            similarity_information.Cleanup();
            return similarity_information;
        }
    }
}
