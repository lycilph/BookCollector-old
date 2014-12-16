using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BookCollector.Model;
using BookCollector.Services.Browsing;
using BookCollector.Services.GoogleBooks;
using BookCollector.Services.Import;
using Caliburn.Micro;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Import
{
    [Export(typeof(IImportController))]
    public class GoogleBooksImportController : IImportController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Uri redirect_uri = new Uri(@"http://localhost:9327");

        private readonly GoogleBooksApi api;
        private readonly ImportInformationViewModel information;
        private readonly IEventAggregator event_aggregator;
        private readonly IProgress<string> progress;
        private TaskCompletionSource<bool> tcs;

        public string Name { get { return "Google Books"; } }

        [ImportingConstructor]
        public GoogleBooksImportController(ImportInformationViewModel information, GoogleBooksApi api, IEventAggregator event_aggregator)
        {
            this.information = information;
            this.api = api;
            this.event_aggregator = event_aggregator;

            progress = new Progress<string>(information.Write);
        }

        public void Start()
        {
            information.Write("Authenticating");
            if (api.IsAuthenticated)
            {
                Finish();
            }
            else
            {
                Authenticate();
            }
        }

        private async void Finish()
        {
            var result = await GetBooksAsync();
            event_aggregator.PublishOnUIThread(ImportMessage.Results(result));
        }

        public Task<List<ImportedBook>> GetBooksAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                progress.Report("Getting books");
                var google_books = api.GetBooks();

                progress.Report(string.Format("Downloaded {0} books", google_books.Count));
                return google_books.Select(Convert).ToList();
            });
        }

        private ImportedBook Convert(GoogleBook book)
        {
            var isbn10 = book.VolumeInfo.IndustryIdentifiers.FirstOrDefault(i => i.Type == "ISBN_10") ?? new GoogleBooksIndustryIdentifiers();
            var isbn13 = book.VolumeInfo.IndustryIdentifiers.FirstOrDefault(i => i.Type == "ISBN_13") ?? new GoogleBooksIndustryIdentifiers();

            return new ImportedBook
            {
                Book = new Book
                {
                    Title = book.VolumeInfo.Title,
                    Description = book.VolumeInfo.Description,
                    Authors = book.VolumeInfo.Authors,
                    ISBN10 = isbn10.Identifier,
                    ISBN13 = isbn13.Identifier,
                    ImportSource = Name
                },
                ImageLinks = new ImageLinks
                {
                    ImageLink = book.VolumeInfo.ImageLinks.Thumbnail,
                    SmallImageLink = book.VolumeInfo.ImageLinks.SmallThumbnail
                }
            };
        }

        public async void Authenticate()
        {
            var task = Task.Factory.StartNew(() => Listen())
                                   .ContinueWith(parent => RequestToken(parent.Result), TaskScheduler.FromCurrentSynchronizationContext());

            var uri = await Task.Factory.StartNew(() =>
            {
                progress.Report("Requesting authorization url");
                return api.RequestAuthorizationUrl(redirect_uri.ToString());
            });
            tcs = BrowserController.ShowAndNavigate(uri.ToString());

            await task;
        }

        private async void RequestToken(string code)
        {
            tcs.SetResult(true);

            progress.Report("Requesting access token");
            var response = await Task.Factory.StartNew(() => api.RequestAccessToken(code, redirect_uri.ToString()));
            api.Settings.AccessToken = response.AccessToken;
            api.Settings.RefreshToken = response.RefreshToken;
            api.Settings.ExpiresIn = DateTime.Now.AddSeconds(response.ExpiresIn);
            progress.Report("Authorization done!");

            Finish();
        }

        private static string Listen()
        {
            var addr = IPAddress.Loopback;
            var listener = new TcpListener(addr, 9327);
            listener.Start();

            var tcp_client = listener.AcceptTcpClient();

            var data = new byte[tcp_client.ReceiveBufferSize];
            string str;
            using (var ns = tcp_client.GetStream())
            {
                var read_count = ns.Read(data, 0, tcp_client.ReceiveBufferSize);
                str = Encoding.UTF8.GetString(data, 0, read_count);
            }

            listener.Stop();

            var elements = str.Split(' ');
            const string code_prefix = "/?code=";
            var code_element = elements.Single(element => element.StartsWith(code_prefix));
            var code = code_element.Substring(code_prefix.Length);
            return code;
        }
    }
}
