using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookSearch.Api.Goodreads;
using BookSearch.Data;
using BookSearch.Utils;
using RestSharp;

namespace BookSearch.Api.SearchProvider
{
    public class GoodreadsSearchProvider : ISearchProvider
    {
        private readonly GoodreadsSettings settings;
        private readonly RestClient client;

        public IProgress<List<Book>> Results { get; private set; }

        public GoodreadsSearchProvider(IProgress<List<Book>> results)
        {
            Results = results;
            settings = SettingsHelper.Get<GoodreadsSettings>("Goodreads");
            client = new RestClient("https://www.goodreads.com");
        }

        public async Task Search(string text)
        {
            var request = new RestRequest("search/index.xml");
            request.AddQueryParameter("q", text);
            request.AddQueryParameter("key", settings.ApiKey);
            var response = await client.ExecuteTaskAsync<GoodreadsResponse>(request);
            var books = response.Data
                                .Results
                                .Select(work => new Book
                                {
                                    Title = work.Title,
                                    Author = work.Author.Name,
                                    Source = "Goodreads"
                                })
                                .ToList();
            Results.Report(books);
        }
    }
}