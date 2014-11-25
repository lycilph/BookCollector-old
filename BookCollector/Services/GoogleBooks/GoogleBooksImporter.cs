using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookCollector.Model;
using BookCollector.Services.Import;

namespace BookCollector.Services.GoogleBooks
{
    public class GoogleBooksImporter : IImporter
    {
        private readonly GoogleBooksApi api;
        private readonly IProgress<ImportProgressStatus> progress;

        public GoogleBooksImporter(GoogleBooksApi api, IProgress<ImportProgressStatus> progress)
        {
            this.api = api;
            this.progress = progress;
        }

        public List<ImportedBook> GetBooks()
        {
            var google_books = api.GetBooks();
            var books = google_books.Select(Convert).ToList();

            var message = string.Format("{0} books found", books.Count);
            progress.Report(new ImportProgressStatus(message, books));

            return books;
        }

        private static ImportedBook Convert(GoogleBook book)
        {
            var isbn10 = book.VolumeInfo.IndustryIdentifiers.FirstOrDefault(i => i.Type == "ISBN_10") ?? new GoogleBooksIndustryIdentifiers();
            var isbn13 = book.VolumeInfo.IndustryIdentifiers.FirstOrDefault(i => i.Type == "ISBN_13") ?? new GoogleBooksIndustryIdentifiers();

            return new ImportedBook
            {
                Book = new Book
                {
                    Title = book.VolumeInfo.Title,
                    Description = book.VolumeInfo.Description,
                    Authors = book.VolumeInfo.Authors,
                    ISBN10 = isbn10.Identifier,
                    ISBN13 = isbn13.Identifier
                },
                ImageLinks = new ImageLinks
                {
                    ImageLink = book.VolumeInfo.ImageLinks.Thumbnail,
                    SmallImageLink = book.VolumeInfo.ImageLinks.SmallThumbnail
                }
            };
        }

        public Task<List<ImportedBook>> GetBooksAsync()
        {
            return Task.Factory.StartNew(() => GetBooks());
        }
    }
}
