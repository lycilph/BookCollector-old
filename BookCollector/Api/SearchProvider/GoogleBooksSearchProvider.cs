using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
    [Export(typeof(ISearchProvider))]
    public class GoogleBooksSearchProvider : ISearchProvider
    {
        private readonly BooksService service;

        public string Image { get { return "Images/Google-Play-Books-icon.png"; } }

        public GoogleBooksSearchProvider()
        {
            var json = ResourceExtensions.GetResource("GoogleBooks");
            var settings = JsonConvert.DeserializeObject<GoodreadsSettings>(json);
            service = new BooksService(new BaseClientService.Initializer
            {
                ApplicationName = "BookCollector",
                ApiKey = settings.ApiKey
            });
        }

        public async Task<List<Book>> Search(string text)
        {
            var request = service.Volumes.List(text);
            var volumes = await request.ExecuteAsync();
            return volumes.Items
                          .Select(volume => new Book
                          {
                              Title = volume.VolumeInfo.Title,
                              Authors = new ReactiveList<string> { volume.VolumeInfo.Authors != null && volume.VolumeInfo.Authors.Any() ? 
                                                                   volume.VolumeInfo.Authors.First() : 
                                                                   string.Empty },
                              Source = "Google Books"
                          })
                          .ToList();
        }
    }
}
