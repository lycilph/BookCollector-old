using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookCollector.Model;
using BookCollector.Services.Import;

namespace BookCollector.Services.Goodreads
{
    public class GoodreadsImporter : IImporter
    {
        private readonly GoodreadsApi api;
        private readonly IProgress<ImportProgressStatus> progress;

        public GoodreadsImporter(GoodreadsApi api, IProgress<ImportProgressStatus> progress)
        {
            this.api = api;
            this.progress = progress;
        }

        public List<ImportedBook> GetBooks()
        {
            var response = api.GetBooks(1, 50, "all");
            var result = response.Books.Select(Convert).ToList();

            Report(result, result.Count);

            if (response.End >= response.Total) 
                return result;

            var pages = (int)Math.Ceiling((double)response.Total / response.End);
            for (var i = 2; i <= pages; i++)
            {
                response = api.GetBooks(i, 50, "all");
                var books = response.Books.Select(Convert).ToList();
                result.AddRange(books);

                Report(books, result.Count);
            }

            return result;
        }

        public Task<List<ImportedBook>> GetBooksAsync()
        {
            return Task.Factory.StartNew(() => GetBooks());
        }

        private static ImportedBook Convert(GoodreadsBook book)
        {
            return new ImportedBook
            {
                Book = new Book
                {
                    Title = book.Title,
                    Description = book.Description,
                    Authors = book.Authors.Select(a => a.Name).ToList(),
                    ISBN10 = book.Isbn,
                    ISBN13 = book.Isbn13
                },
                ImageLinks = new ImageLinks
                {
                    ImageLink = book.ImageUrl,
                    SmallImageLink = book.SmallImageUrl
                }
            };
        }

        private void Report(IEnumerable<ImportedBook> result, int count)
        {
            var message = string.Format("{0} books found", count);
            progress.Report(new ImportProgressStatus(message, result));
        }
    }
}
