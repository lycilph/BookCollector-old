using System.Collections.Generic;
using BookCollector.Data;

namespace BookCollector.Services.Search
{
    public interface ISearchEngine
    {
        void Index(List<Book> books);
        List<SearchResult> Search(string query);
    }
}