using System;
using System.Collections.Generic;
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

        public List<Book> GetBooks()
        {
            throw new NotImplementedException();
        }

        public Task<List<Book>> GetBooksAsync()
        {
            throw new NotImplementedException();
        }
    }
}
