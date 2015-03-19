using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookCollector.Api.Goodreads;
using BookCollector.Data;
using Google.Apis.Books.v1;
using Google.Apis.Services;
using Newtonsoft.Json;
using Panda.Utilities.Extensions;
using ReactiveUI;

namespace BookCollector.Api.SearchProvider
{
    public class GoogleBooksSearchProvider : ISearchProvider
    {
        private readonly BooksService service;

        public IProgress<List<Book>> Results { get; private set; }

        public GoogleBooksSearchProvider(IProgress<List<Book>> results)
        {
            Results = results;
            var json = ResourceExtensions.GetResource("GoogleBooks");
            var settings = JsonConvert.DeserializeObject<GoodreadsSettings>(json);
            service = new BooksService(new BaseClientService.Initializer
            {
                ApplicationName = "BookCollector",
                ApiKey = settings.ApiKey
            });
        }

        public async Task Search(string text)
        {
            var request = service.Volumes.List(text);
            var volumes = await request.ExecuteAsync();
            var books = volumes.Items
                               .Select(volume => new Book
                               {
                                   Title = volume.VolumeInfo.Title,
                                   Authors = new ReactiveList<string> { volume.VolumeInfo.Authors != null && volume.VolumeInfo.Authors.Any() ? 
                                                                        volume.VolumeInfo.Authors.First() : 
                                                                        string.Empty },
                                   Source = "Google Books"
                               })
                               .ToList();
            Results.Report(books);
        }
    }
}
