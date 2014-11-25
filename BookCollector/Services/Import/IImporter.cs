using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookCollector.Services.Import
{
    public interface IImporter
    {
        List<ImportedBook> GetBooks();
        Task<List<ImportedBook>> GetBooksAsync();
    }
}
