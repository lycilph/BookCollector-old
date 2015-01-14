using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using BookCollector.Model;
using BookCollector.Screens.Import;
using BookCollector.Services;
using BookCollector.Services.Browsing;
using Caliburn.Micro;
using NLog;
using RestSharp.Contrib;
using LogManager = NLog.LogManager;

namespace BookCollector.Apis.GoodReads
{
    [Export(typeof(IImportController))]
    public class GoodReadsImportController : IImportController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Uri callback_uri = new Uri(@"custom://www.bookcollector.com");

        private readonly ApplicationSettings application_settings;
        private readonly GoodReadsApi api;
        private readonly IEventAggregator event_aggregator;
        private readonly Browser browser;
        private readonly IProgress<string> progress;

        public string ApiName { get { return api.Name; } }

        [ImportingConstructor]
        public GoodReadsImportController(GoodReadsApi api, ApplicationSettings application_settings, IEventAggregator event_aggregator, Browser browser)
        {
            this.api = api;
            this.application_settings = application_settings;
            this.event_aggregator = event_aggregator;
            this.browser = browser;

            progress = new Progress<string>(str => event_aggregator.PublishOnUIThread(ImportMessage.Information(str)));
        }

        public async void Start(ProfileDescription profile)
        {
            var credentials = await Authenticate(profile);
            var result = await GetBooksAsync(credentials);

            event_aggregator.PublishOnUIThread(ImportMessage.Results(result));
        }

        private async Task<GoodReadsCredentials> Authenticate(ProfileDescription profile)
        {
            var credentials = application_settings.GetCredentials<GoodReadsCredentials>(profile.Id, api.Name);
            if (credentials != null)
                return credentials;

            progress.Report("Requesting authorization token");
            var authorization_response = await Task.Factory.StartNew(() => api.RequestAuthorizationToken(callback_uri.ToString()));
            logger.Trace("Response url: " + authorization_response.Url);

            var tcs = browser.Show();
            await browser.Load(authorization_response.Url, s => Predicate(s, tcs));
            await tcs.Task;

            progress.Report("Requesting access token");
            var access_response = await Task.Factory.StartNew(() => api.RequestAccessToken(authorization_response));
            credentials = new GoodReadsCredentials(access_response);

            progress.Report("Requesting user id");
            credentials.UserId = await Task.Factory.StartNew(() => api.GetUserId(credentials));

            progress.Report("Authorization done!");
            application_settings.AddCredentials(profile.Id, api.Name, credentials);

            return credentials;
        }

        private bool Predicate(string s, TaskCompletionSource<bool> tcs)
        {
            var uri = new Uri(s);
            if (uri.Host != callback_uri.Host || uri.Scheme != callback_uri.Scheme)
                return false;

            var query_string = HttpUtility.ParseQueryString(uri.Query);
            if (query_string["authorize"] != "1")
                return false;

            tcs.SetResult(true);
            return true;
        }

        public Task<List<ImportedBook>> GetBooksAsync(GoodReadsCredentials credentials)
        {
            return Task.Factory.StartNew(() =>
            {
                progress.Report("Getting books");
                var response = api.GetBooks(credentials, 1, 50, "all");
                var result = response.Books.Select(Convert).ToList();

                progress.Report(string.Format("Downloaded {0} books", result.Count));

                if (response.End >= response.Total)
                    return result;

                var pages = (int)Math.Ceiling((double)response.Total / response.End);
                for (var i = 2; i <= pages; i++)
                {
                    response = api.GetBooks(credentials, i, 50, "all");
                    var books = response.Books.Select(Convert).ToList();
                    result.AddRange(books);

                    progress.Report(string.Format("Downloaded {0} books", result.Count));
                }

                progress.Report("Download completed");

                return result;
            });
        }

        private ImportedBook Convert(GoodReadsBook book)
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
                    ImportSource = ApiName
                },
                ImageLinks = image_links
            };
        }

        //private void HandleLoadEnd(Uri uri)
        //{
        //    if (uri.Host != callback_uri.Host || uri.Scheme != callback_uri.Scheme)
        //        return;

        //    var query_string = HttpUtility.ParseQueryString(uri.Query);
        //    if (query_string["authorize"] != "1")
        //        return;

        //    tcs.SetResult(true);
        //}

        //public void Handle(BrowsingMessage message)
        //{
        //    logger.Trace("{0}: {1}", Enum.GetName(typeof(BrowsingMessage.MessageKind), message.Kind), message.Uri);

        //    switch (message.Kind)
        //    {
        //        case BrowsingMessage.MessageKind.LoadStart:
        //            break;
        //        case BrowsingMessage.MessageKind.LoadEnd:
        //            HandleLoadEnd(message.Uri);
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //}
    }
}
