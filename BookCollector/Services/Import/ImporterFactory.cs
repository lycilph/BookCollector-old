using System;
using BookCollector.Services.Goodreads;
using BookCollector.Services.GoogleBooks;

namespace BookCollector.Services.Import
{
    public static class ImporterFactory
    {
        public static IImporter Create(IApi api, IProgress<ImportProgressStatus> progress)
        {
            if (api is GoodreadsApi)
                return new GoodreadsImporter(api as GoodreadsApi, progress);

            if (api is GoogleBooksApi)
                return new GoogleBooksImporter(api as GoogleBooksApi, progress);

            throw new Exception("Unknown api");
        }
    }
}
