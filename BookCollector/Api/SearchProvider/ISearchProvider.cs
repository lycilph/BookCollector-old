using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookCollector.Data;

namespace BookCollector.Api.SearchProvider
{
    public interface ISearchProvider
    {
        IProgress<List<Book>> Results { get; }

        Task Search(string text);
    }
}