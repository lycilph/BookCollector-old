using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookSearch.Api.GoogleBooks;
using BookSearch.Data;
using BookSearch.Utils;
using Google.Apis.Books.v1;
using Google.Apis.Services;

namespace BookSearch.Api.SearchProvider
{
    public class GoogleBooksSearchProvider : ISearchProvider
    {
        private readonly BooksService service;

        public IProgress<List<Book>> Results { get; private set; }

        public GoogleBooksSearchProvider(IProgress<List<Book>> results)
        {
            Results = results;
            var settings = SettingsHelper.Get<GoogleBooksSettings>("GoogleBooks");
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
                                   Author = (volume.VolumeInfo.Authors != null && volume.VolumeInfo.Authors.Any() ? volume.VolumeInfo.Authors.First() : string.Empty),
                                   Source = "Google Books"
                               })
                               .ToList();
            Results.Report(books);
        }
    }
}
