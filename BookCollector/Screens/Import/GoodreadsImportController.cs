using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using BookCollector.Apis.Goodreads;
using BookCollector.Model;
using BookCollector.Services.Browsing;
using Caliburn.Micro;
using NLog;
using RestSharp.Contrib;
using LogManager = NLog.LogManager;

namespace BookCollector.Screens.Import
{
    [Export(typeof(IImportController))]
    public class GoodreadsImportController : IImportController, IHandle<BrowsingMessage>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Uri callback_uri = new Uri(@"custom://www.bookcollector.com");

        private readonly GoodreadsApi api;
        private readonly IEventAggregator event_aggregator;
        private readonly IProgress<string> progress;
        private GoodreadsAuthorizationResponse authorization_response;
        private TaskCompletionSource<bool> tcs;

        public string Name { get { return "Goodreads"; } }

        [ImportingConstructor]
        public GoodreadsImportController(GoodreadsApi api, IEventAggregator event_aggregator)
        {
            this.api = api;
            this.event_aggregator = event_aggregator;

            progress = new Progress<string>(str => event_aggregator.PublishOnUIThread(ImportMessage.Information(str)));
        }

        public async void Start()
        {
            event_aggregator.Subscribe(this);

            progress.Report("Authenticating");
            if (api.IsAuthenticated)
            {
                await Finish();
            }
            else
            {
                progress.Report("Requesting authorization token");
                authorization_response = await api.RequestAuthorizationTokenAsync(callback_uri.ToString());
                logger.Trace("Response url: " + authorization_response.Url);

                tcs = BrowserController.ShowAndNavigate(authorization_response.Url);
            }
        }

        private async Task Finish()
        {
            var result = await GetBooksAsync();

            event_aggregator.Unsubscribe(this);
            event_aggregator.PublishOnUIThread(ImportMessage.Results(result));
        }

        public Task<List<ImportedBook>> GetBooksAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                progress.Report("Getting books");
                var response = api.GetBooks(1, 50, "all");
                var result = response.Books.Select(Convert).ToList();

                progress.Report(string.Format("Downloaded {0} books", result.Count));

                if (response.End >= response.Total)
                    return result;

                var pages = (int)Math.Ceiling((double)response.Total / response.End);
                for (var i = 2; i <= pages; i++)
                {
                    response = api.GetBooks(i, 50, "all");
                    var books = response.Books.Select(Convert).ToList();
                    result.AddRange(books);

                    progress.Report(string.Format("Downloaded {0} books", result.Count));
                }

                progress.Report("Download completed");

                return result;
            });
        }

        private ImportedBook Convert(GoodreadsBook book)
        {
            var image_links = new List<ImageLink>();

            if (!book.ImageUrl.ToLowerInvariant().Contains("nophoto"))
                image_links.Add(new ImageLink(book.ImageUrl, "Image"));
            if (!book.SmallImageUrl.ToLowerInvariant().Contains("nophoto"))
                image_links.Add(new ImageLink(book.SmallImageUrl, "SmallImage"));

            return new ImportedBook
            {
                Book = new Book
                {
                    Title = book.Title,
                    Description = book.Description,
                    Authors = book.Authors.Select(a => a.Name).ToList(),
                    ISBN10 = book.Isbn,
                    ISBN13 = book.Isbn13,
                    ImportSource = Name
                },
                ImageLinks = image_links
            };
        }

        private async void HandleLoadEnd(Uri uri)
        {
            if (uri.Host != callback_uri.Host || uri.Scheme != callback_uri.Scheme)
                return;

            var query_string = HttpUtility.ParseQueryString(uri.Query);
            if (query_string["authorize"] != "1")
                return;

            tcs.SetResult(true);
            progress.Report("Requesting access token");
            var access_response = await Task.Factory.StartNew(() => api.RequestAccessToken(authorization_response.OAuthToken, authorization_response.OAuthTokenSecret));
            api.Settings.OAuthToken = access_response.OAuthToken;
            api.Settings.OAuthTokenSecret = access_response.OAuthTokenSecret;

            progress.Report("Requesting user id");
            var user_response = await Task.Factory.StartNew(() => api.GetUserId());
            api.Settings.UserId = user_response;

            progress.Report("Authorization done!");

            await Finish();
        }

        public void Handle(BrowsingMessage message)
        {
            logger.Trace("{0}: {1}", Enum.GetName(typeof(BrowsingMessage.MessageKind), message.Kind), message.Uri);

            switch (message.Kind)
            {
                case BrowsingMessage.MessageKind.LoadStart:
                    break;
                case BrowsingMessage.MessageKind.LoadEnd:
                    HandleLoadEnd(message.Uri);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
