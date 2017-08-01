using System.Collections.Generic;
using BookCollector.Data.Import;

namespace BookCollector.Services
{
    public interface IImportService
    {
        void GetSimilarity(ImportedBook imported_book);
        void Import(List<ImportedBook> books_to_import);
    }
}