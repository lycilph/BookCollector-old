using System.Collections.Generic;
using System.Threading.Tasks;
using BookCollector.Model;

namespace BookCollector.Services.Import
{
    public interface IImporter
    {
        List<Book> GetBooks();
        Task<List<Book>> GetBooksAsync();
    }
}
