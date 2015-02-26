using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BookCollector.Model;
using BookCollector.Screens.Import;
using BookCollector.Services;
using BookCollector.Services.Browsing;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Apis.GoogleBooks
{
    [Export(typeof(IImportController))]
    public class GoogleBooksImportController : IImportController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Uri redirect_uri = new Uri(@"http://localhost:9327");

        private readonly GoogleBooksApi api;
        private readonly ApplicationSettings application_settings;
        private readonly IImportProcessController import_process_controller;
        private readonly Browser browser;
        private readonly IProgress<string> progress;

        public string ApiName { get { return api.Name; } }

        [ImportingConstructor]
        public GoogleBooksImportController(GoogleBooksApi api, ApplicationSettings application_settings, IImportProcessController import_process_controller, Browser browser)
        {
            this.api = api;
            this.application_settings = application_settings;
            this.import_process_controller = import_process_controller;
            this.browser = browser;

            progress = new Progress<string>(import_process_controller.UpdateProgress);
        }

        public async void Start(ProfileDescription profile)
        {
            var credentials = await Authenticate(profile);
            var result = await GetBooksAsync(credentials);

            import_process_controller.ShowResults(result);
        }

        private async Task<GoogleBooksCredentials> Authenticate(ProfileDescription profile)
        {
            var credentials = application_settings.GetCredentials<GoogleBooksCredentials>(profile.Id, api.Name);
            if (credentials != null)
                return credentials;

            var authorization_response_task = Task.Factory.StartNew(() => Listen());

            progress.Report("Requesting authorization url");
            var uri = await Task.Factory.StartNew(() => api.RequestAuthorizationUrl(redirect_uri.ToString()));
            logger.Trace("Response uri: " + uri);

            var tcs = browser.Show();
            await browser.Load(uri.ToString());
            var code = await authorization_response_task;
            tcs.SetResult(MessageDialogResult.Affirmative);

            progress.Report("Requesting access token");
            var response = await Task.Factory.StartNew(() => api.RequestAccessToken(code, redirect_uri.ToString()));
            credentials = new GoogleBooksCredentials(response);

            progress.Report("Authorization done!");
            application_settings.AddCredentials(profile.Id, api.Name, credentials);

            return credentials;
        }

        public Task<List<ImportedBook>> GetBooksAsync(GoogleBooksCredentials credentials)
        {
            return Task.Factory.StartNew(() =>
            {
                progress.Report("Getting books");
                var google_books = api.GetBooks(credentials);

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
                    ImportSource = api.Name,
                    ImageSource = api.Name
                },
                ImageLinks = new List<ImageLink>
                {
                    new ImageLink(book.VolumeInfo.ImageLinks.Thumbnail, "Image"),
                    new ImageLink(book.VolumeInfo.ImageLinks.SmallThumbnail, "SmallImage")
                }
            };
        }

        private static string Listen()
        {
            var listener = new TcpListener(IPAddress.Loopback, 9327);
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
