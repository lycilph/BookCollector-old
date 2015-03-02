using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookSearch.Data;

namespace BookSearch.Api.SearchProvider
{
    public interface ISearchProvider
    {
        IProgress<List<Book>> Results { get; }

        Task Search(string text);
    }
}