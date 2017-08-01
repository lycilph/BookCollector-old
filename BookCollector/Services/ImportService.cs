using System;
using System.Collections.Generic;
using System.Linq;
using BookCollector.Data;
using BookCollector.Data.Import;
using Core.Extensions;
using Core.Mapping;

namespace BookCollector.Services
{
    public class ImportService : IImportService
    {
        private ICollectionsService collections_service;

        public ImportService(ICollectionsService collections_service)
        {
            this.collections_service = collections_service;
        }

        public void Import(List<ImportedBook> books_to_import)
        {
            // Map to "normal" book class
            var books = books_to_import.Select(b => Mapper.Map<Book>(b)).ToList();

            // Add a history entry
            books.Apply(b => b.History.Add($"Imported on the {DateTime.Now.ToShortDateString()}"));

            // Add to default shelf
            collections_service.Current.DefaultShelf.Add(books);

            // Add to shelves
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
