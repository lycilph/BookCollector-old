using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using BookCollector.Model;
using BookCollector.Services.Audible;
using BookCollector.Services.Import;
using Caliburn.Micro;
using CefSharp;
using CefSharp.OffScreen;
using HtmlAgilityPack;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Import
{
    [Export(typeof(IImportController))]
    public class AudibleImportController : IImportController
    {
        private enum State
        {
            LoadingMainPage,
            LoadingSignInPage,
            SigningIn,
            LoadingLibraryPage,
            SettingTimeFilter,
            SettingItemsFilter
        }

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator event_aggregator;
        private readonly IProgress<string> progress;
        private ChromiumWebBrowser wb;
        private State current_state;

        public string Name { get { return "Audible"; } }

        [ImportingConstructor]
        public AudibleImportController(IEventAggregator event_aggregator, ImportInformationViewModel information)
        {
            this.event_aggregator = event_aggregator;

            progress = new Progress<string>(information.Write);
        }

        public void Start()
        {
            progress.Report("Starting offscreen webbrowser");

            wb = new ChromiumWebBrowser();
            wb.FrameLoadEnd += OnFrameLoadEnd;
            wb.Load(@"http://www.audible.com");

            current_state = State.LoadingMainPage;
        }

        private async void OnFrameLoadEnd(object sender, FrameLoadEndEventArgs args)
        {
            logger.Trace("Loaded: " + args.Url);

            var uri = new Uri(args.Url);
            if (uri.Host != @"www.audible.com" && uri.Host != @"www.amazon.com")
                return;

            if (current_state == State.LoadingMainPage)
            {
                progress.Report("Loaded main page");

                if (await IsSignedIn())
                    LoadLibraryPage();
                else
                    await LoadSignInPage();
            }
            else if (current_state == State.LoadingSignInPage)
            {
                await SignIn();
            }
            else if (current_state == State.SigningIn)
            {
                LoadLibraryPage();
            }
            else if (current_state == State.LoadingLibraryPage)
            {
                await SetTimeFilter();
            }
            else if (current_state == State.SettingTimeFilter)
            {
                await SetItemsFilter();
            }
            else if (current_state == State.SettingItemsFilter)
            {
                var books = await ParseBooks();
                Finish(books);
            }
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
                }
            }).ToList();
            event_aggregator.PublishOnUIThread(ImportMessage.Results(imported_books));
        }

        private async Task<List<AudibleBook>> ParseBooks()
        {
            progress.Report("Parsing items");

            var source = await wb.GetSourceAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(source);

            var books = new List<AudibleBook>();
            var content = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'adbl-lib-content')]");
            foreach (var node in content.SelectNodes(".//tr"))
            {
                if (!node.HasChildNodes) continue;

                var inputs = node.SelectNodes(".//input");
                if (inputs == null) continue;

                var asin_node =
                    inputs.SingleOrDefault(
                        i =>
                            i.HasAttributes && i.Attributes.Contains("name") &&
                            i.Attributes["name"].Value.ToLowerInvariant() == "asin");
                if (asin_node == null) continue;

                var parent_asin_node =
                    inputs.SingleOrDefault(
                        i =>
                            i.HasAttributes && i.Attributes.Contains("name") &&
                            i.Attributes["name"].Value.ToLowerInvariant() == "parentasin");
                if (parent_asin_node == null) continue;

                var title_node =
                    node.SelectNodes(".//a[@name]")
                        .SingleOrDefault(n => n.Attributes["name"].Value.ToLowerInvariant() == "tdtitle");
                if (title_node == null) continue;

                var description_node = node.SelectSingleNode(".//p");
                if (description_node == null) continue;

                var list = node.SelectNodes(".//strong");
                if (list == null) continue;

                var parent_asin = parent_asin_node.Attributes["value"].Value;
                var asin = asin_node.Attributes["value"].Value;
                var title = title_node.InnerText;
                var description = description_node.InnerText;
                var authors = list[0].InnerText;
                var narrators = list[1].InnerText;

                if (string.IsNullOrWhiteSpace(parent_asin))
                {
                    var authors_list = authors.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(a => a.Trim())
                        .ToList();
                    var narrators_list = narrators.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(n => n.Trim())
                        .ToList();

                    var book = new AudibleBook
                    {
                        Title = title,
                        Asin = asin,
                        Authors = authors_list,
                        Narrators = narrators_list,
                        Description = description
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

        private async Task SetItemsFilter()
        {
            progress.Report("Settings items filter");

            var result = await wb.EvaluateScriptAsync("document.getElementsByName('itemsPerPage')[0].value = document.getElementById('totalItems').value");
            if (!result.Success)
                throw new Exception();

            current_state = State.SettingItemsFilter;
            wb.ExecuteScriptAsync("document.getElementById('myLibraryForm').submit()");
        }

        private async Task SetTimeFilter()
        {
            progress.Report("Loaded library page");

            var result = await wb.EvaluateScriptAsync("document.getElementsByName('timeFilter')[0].value = 'all'");
            if (!result.Success)
                throw new Exception();

            progress.Report("Setting time filter");

            current_state = State.SettingTimeFilter;
            wb.ExecuteScriptAsync("document.getElementById('myLibraryForm').submit()");
        }

        private async Task SignIn()
        {
            progress.Report("Loaded sign in page");

            var email_result = await wb.EvaluateScriptAsync("document.getElementById('ap_email').value = 'lycilph@gmail.com'");
            if (!email_result.Success)
                throw new Exception();

            var password_result = await wb.EvaluateScriptAsync("document.getElementById('ap_password').value = 'zgubbtCmsu7B'");
            if (!password_result.Success)
                throw new Exception();

            current_state = State.SigningIn;
            wb.ExecuteScriptAsync("document.getElementById('signInSubmit').click()");
        }

        private async Task LoadSignInPage()
        {
            var source = await wb.GetSourceAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(source);

            var node = doc.GetElementbyId("anon_header_v2_signin");
            var link_node = node.SelectSingleNode("a[@href]");
            var href = link_node.Attributes["href"].Value;

            var sign_in_uri = new Uri(new Uri(@"http://www.audible.com/"), href);
            logger.Trace("login page url: " + sign_in_uri);

            current_state = State.LoadingSignInPage;
            wb.Load(sign_in_uri.ToString());
        }

        private async void LoadLibraryPage()
        {
            progress.Report("Finding library page link");

            var source = await wb.GetSourceAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(source);

            var node = doc.GetElementbyId("library-menu");
            var link_node = node.SelectSingleNode(".//a[@href]");
            var href = link_node.Attributes["href"].Value;

            var library_uri = new Uri(new Uri(@"http://www.audible.com/"), href);
            logger.Trace("Library page url: " + library_uri);

            current_state = State.LoadingLibraryPage;
            wb.Load(library_uri.ToString());
        }

        private async Task<bool> IsSignedIn()
        {
            var result = await wb.EvaluateScriptAsync("document.getElementById('anon_header_v2_signin') == null");
            return Convert.ToBoolean(result.Result);
        }
    }
}
