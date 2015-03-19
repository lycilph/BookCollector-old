using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookCollector.Amazon;
using BookCollector.Api.Amazon;
using BookCollector.Data;
using Newtonsoft.Json;
using Panda.Utilities.Extensions;
using ReactiveUI;

namespace BookCollector.Api.SearchProvider
{
    public class AmazonSearchProvider : ISearchProvider
    {
        private readonly AmazonSettings settings;
        private readonly AWSECommerceServicePortTypeClient client;

        public IProgress<List<Book>> Results { get; private set; }

        public AmazonSearchProvider(IProgress<List<Book>> results)
        {
            Results = results;
            var json = ResourceExtensions.GetResource("Amazon");
            settings = JsonConvert.DeserializeObject<AmazonSettings>(json);
            client = new AWSECommerceServicePortTypeClient("AWSECommerceServicePort");
            client.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(settings.AccessKeyId, settings.SecretKey));
        }

        public async Task Search(string text)
        {
            var item_search = new ItemSearch
            {
                Request = new[]
                {
                    new ItemSearchRequest
                    {
                        SearchIndex = "Books",
                        Keywords = text,
                        ResponseGroup = new[] { "ItemAttributes", "Images" }
                    },
                },
                AWSAccessKeyId = settings.AccessKeyId,
                AssociateTag = settings.AssociateTag
            };
            var response = await client.ItemSearchAsync(item_search);
            var books = response.ItemSearchResponse
                                .Items[0]
                                .Item
                                .Select(item => new Book
                                {
                                    Title = item.ItemAttributes.Title,
                                    Authors = new ReactiveList<string> { item.ItemAttributes.Author.First() },
                                    Source = "Amazon"
                                })
                                .ToList();
            Results.Report(books);
        }
    }
}
