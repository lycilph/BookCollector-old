using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using BookCollector.Apis.Audible;
using BookCollector.Model;
using BookCollector.Services.Browsing;
using BookCollector.Services.Repository;
using BookCollector.Utilities;
using Caliburn.Micro;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Screens.Import
{
    [Export(typeof(IImportController))]
    public class AudibleImportController : IImportController, IHandle<BrowsingMessage>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Uri audible_uri = new Uri(@"http://www.audible.com");
        private static readonly Uri amazon_uri = new Uri(@"http://www.amazon.com");

        private enum State
        {
            LoadingMainPage,
            LoadingSignInPage,
            SigningIn,
            LoadingLibraryPage,
            SettingTimeFilter,
            SettingItemsFilter
        }

        private readonly IEventAggregator event_aggregator;
        private readonly IProgress<string> progress;
        private TaskCompletionSource<bool> tcs;
        private State current_state;

        public string Name { get { return "Audible"; } }

        [ImportingConstructor]
        public AudibleImportController(IEventAggregator event_aggregator)
        {
            this.event_aggregator = event_aggregator;

            progress = new Progress<string>(str => event_aggregator.PublishOnUIThread(ImportMessage.Information(str)));
        }

        public void Start()
        {
            event_aggregator.Subscribe(this);
   
            progress.Report("Starting offscreen webbrowser");
            current_state = State.LoadingMainPage;
            BrowserController.NavigateOffscreen(audible_uri.ToString());
        }

        private async void LoadEnd(Uri uri)
        {
            if (uri.Host != audible_uri.Host && uri.Host != amazon_uri.Host)
                return;

            logger.Trace("Current state: " + Enum.GetName(typeof(State), current_state));

            switch (current_state)
            {
                case State.LoadingMainPage:
                    HandleMainPage();
                    break;
                case State.LoadingSignInPage:
                    current_state = State.SigningIn;
                    break;
                case State.SigningIn:
                    tcs.SetResult(true);
                    LoadLibraryPage();
                    break;
                case State.LoadingLibraryPage:
                    progress.Report("Library page loaded");
                    await SetTimeFilter();
                    break;
                case State.SettingTimeFilter:
                    await SetItemsFilter();
                    break;
                case State.SettingItemsFilter:
                    var books = await ParseBooks();
                    Finish(books);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task<List<AudibleBook>> ParseBooks()
        {
            progress.Report("Parsing books");

            var books = new List<AudibleBook>();
            var doc = await BrowserController.GetOffscreenSource();
            var content = doc.DocumentNode.SelectSingleNode("//div[@class='adbl-lib-content']");
            foreach (var node in content.SelectNodes(".//tr"))
            {
                if (!node.HasChildNodes) continue;

                var inputs = node.SelectNodes(".//input");
                if (inputs == null) continue;

                var asin_node = inputs.SingleNodeWithAttributeNameAndValue("name", "asin");
                if (asin_node == null) continue;

                var parent_asin_node = inputs.SingleNodeWithAttributeNameAndValue("name", "parentasin");
                if (parent_asin_node == null) continue;

                var title_node = node.SelectSingleNode(".//a[@name='tdTitle']");
                if (title_node == null) continue;

                var description_node = node.SelectSingleNode(".//p");
                if (description_node == null) continue;

                var list = node.SelectNodes(".//strong");
                if (list == null) continue;

                var product_cover_node = node.SelectSingleNode(".//td[@name='productCover']");
                var image_node = (product_cover_node != null ? product_cover_node.SelectSingleNode(".//img") : null);

                var parent_asin = parent_asin_node.Attributes["value"].Value;
                var asin = asin_node.Attributes["value"].Value;

                if (string.IsNullOrWhiteSpace(parent_asin))
                {
                    var authors = list[0].InnerText;
                    var authors_list = authors.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(a => a.Trim())
                        .ToList();

                    var narrators = list[1].InnerText;
                    var narrators_list = narrators.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(n => n.Trim())
                        .ToList();

                    var book = new AudibleBook
                    {
                        Title = title_node.InnerText,
                        Asin = asin,
                        Authors = authors_list,
                        Narrators = narrators_list,
                        Description = description_node.InnerText.Trim(),
                        ImageUrl = (image_node == null ? "" : image_node.Attributes["src"].Value)
                    };
                    books.Add(book);
                }
                else
                {
                    var parent = books.SingleOrDefault(b => b.Asin == parent_asin);
                    if (parent != null)
                        parent.PartsAsin.Add(asin);
                }
            }

            return books;
        }

        private void Finish(IReadOnlyCollection<AudibleBook> books)
        {
            logger.Trace("Found {0} books", books.Count);

            var imported_books = books.Select(b => new ImportedBook
            {
                Book = new Book
                {
                    Title = b.Title,
                    Description = b.Description,
                    Asin = b.Asin,
                    Authors = b.Authors,
                    Narrators = b.Narrators,
                    ImportSource = Name
                },
                ImageLinks = new List<ImageLink>
                {
                    new ImageLink(b.ImageUrl, "Image")
                }
            }).ToList();
            event_aggregator.PublishOnUIThread(ImportMessage.Results(imported_books));
            
        }

        private async Task SetItemsFilter()
        {
            progress.Report("Setting items filter");

            var result = await BrowserController.EvaluateOffscreen("document.getElementsByName('itemsPerPage')[0].value = document.getElementById('totalItems').value");
            if (!result.Success)
                throw new Exception();

            current_state = State.SettingItemsFilter;
            BrowserController.ExecuteOffscreen("document.getElementById('myLibraryForm').submit()");
        }

        private async Task SetTimeFilter()
        {
            progress.Report("Setting time filter");

            var result = await BrowserController.EvaluateOffscreen("document.getElementsByName('timeFilter')[0].value = 'all'");
            if (!result.Success)
                throw new Exception();

            current_state = State.SettingTimeFilter;
            BrowserController.ExecuteOffscreen("document.getElementById('myLibraryForm').submit()");
        }

        private async void LoadLibraryPage()
        {
            progress.Report("Finding library page link");

            var doc = await BrowserController.GetOffscreenSource();
            var link = doc.ChildLinkById("library-menu");
            var library_uri = new Uri(audible_uri, link);

            logger.Trace("Library page url: " + library_uri);

            current_state = State.LoadingLibraryPage;
            BrowserController.NavigateOffscreen(library_uri.ToString());
        }

        private async void HandleMainPage()
        {
            progress.Report("Main page loaded");

            if (await IsSignedIn())
                LoadLibraryPage();
            else
                HandleSignIn();
        }

        private async void HandleSignIn()
        {
            logger.Trace("Signing in");

            var doc = await BrowserController.GetOffscreenSource();
            var link = doc.ChildLinkById("anon_header_v2_signin");
            var sign_in_uri = new Uri(audible_uri, link);

            logger.Trace("login page url: " + sign_in_uri);

            current_state = State.LoadingSignInPage;
            tcs = BrowserController.ShowAndNavigate(sign_in_uri.ToString());
        }

        private static async Task<bool> IsSignedIn()
        {
            logger.Trace("Authenticating");

            var result = await BrowserController.EvaluateOffscreen("document.getElementById('anon_header_v2_signin') == null");
            return Convert.ToBoolean(result.Result);
        }

        public void Handle(BrowsingMessage message)
        {
            logger.Trace("{0}: {1}", Enum.GetName(typeof(BrowsingMessage.MessageKind), message.Kind), message.Uri);

            switch (message.Kind)
            {
                case BrowsingMessage.MessageKind.LoadStart:
                    break;
                case BrowsingMessage.MessageKind.LoadEnd:
                    LoadEnd(message.Uri);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
