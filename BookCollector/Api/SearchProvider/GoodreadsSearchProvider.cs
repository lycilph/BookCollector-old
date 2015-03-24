using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using BookCollector.Api.Goodreads;
using BookCollector.Data;
using Newtonsoft.Json;
using Panda.Utilities.Extensions;
using ReactiveUI;
using RestSharp;

namespace BookCollector.Api.SearchProvider
{
    [Export(typeof(ISearchProvider))]
    public class GoodreadsSearchProvider : ISearchProvider
    {
        private readonly GoodreadsSettings settings;
        private readonly RestClient client;

        public string Image { get { return "Images/Goodreads-icon.png"; } }

        public GoodreadsSearchProvider()
        {
            var json = ResourceExtensions.GetResource("Goodreads");
            settings = JsonConvert.DeserializeObject<GoodreadsSettings>(json);
            client = new RestClient("https://www.goodreads.com");
        }

        public async Task<List<Book>> Search(string text)
        {
            var request = new RestRequest("search/index.xml");
            request.AddQueryParameter("q", text);
            request.AddQueryParameter("key", settings.ApiKey);
            var response = await client.ExecuteTaskAsync<GoodreadsResponse>(request);
            return response.Data
                           .Results
                           .Select(work => new Book
                           {
                               Title = work.Title,
                               Authors = new ReactiveList<string> { work.Author.Name },
                               Source = "Goodreads"
                           })
                           .ToList();
        }
    }
}