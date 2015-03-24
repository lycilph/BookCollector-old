using System.Collections.Generic;
using System.Threading.Tasks;
using BookCollector.Data;

namespace BookCollector.Api.SearchProvider
{
    public interface ISearchProvider
    {
        string Image { get; }

        Task<List<Book>> Search(string text);
    }
}