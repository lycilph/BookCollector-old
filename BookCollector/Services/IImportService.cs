using System.Collections.Generic;
using BookCollector.Data;
using BookCollector.Data.Import;

namespace BookCollector.Services
{
    public interface IImportService
    {
        void Import(List<ImportedBook> books_to_import, Dictionary<string, Shelf> shelf_mapping);
        Shelf Map(string imported_shelf);
        void GetSimilarity(ImportedBook imported_book);
    }
}