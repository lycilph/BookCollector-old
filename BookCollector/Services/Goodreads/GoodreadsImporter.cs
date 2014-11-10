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

        public List<Book> GetBooks()
        {
            var response = api.GetBooks(1, 50, "all");
            var result = response.Books.Select(b => b.ToBook()).ToList();

            Report(result, result.Count);

            if (response.End < response.Total)
            {
                var pages = (int)Math.Ceiling((double)response.Total / response.End);
                for (var i = 2; i <= pages; i++)
                {
                    response = api.GetBooks(i, 50, "all");
                    var books = response.Books.Select(b => b.ToBook()).ToList();
                    result.AddRange(books);

                    Report(books, result.Count);
                }
            }

            return result;
        }

        public Task<List<Book>> GetBooksAsync()
        {
            return Task.Factory.StartNew(() => GetBooks());
        }

        private void Report(IEnumerable<Book> result, int count)
        {
            var message = string.Format("{0} books found", count);
            progress.Report(new ImportProgressStatus(message, result));
        }
    }
}
