using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookCollector.Api.Goodreads;
using BookCollector.Data;
using BookCollector.Utilities;
using ReactiveUI;
using RestSharp;

namespace BookCollector.Api.SearchProvider
{
    public class GoodreadsSearchProvider : ISearchProvider
    {
        private readonly GoodreadsSettings settings;
        private readonly RestClient client;

        public IProgress<List<Book>> Results { get; private set; }

        public GoodreadsSearchProvider(IProgress<List<Book>> results)
        {
            Results = results;
            settings = ResourceHelper.GetAndDeserialize<GoodreadsSettings>("Goodreads");
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
                                    Authors = new ReactiveList<string> { work.Author.Name },
                                    Source = "Goodreads"
                                })
                                .ToList();
            Results.Report(books);
        }
    }
}