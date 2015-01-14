using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BookCollector.Model;
using BookCollector.Screens.Import;
using BookCollector.Services;
using BookCollector.Services.Browsing;
using BookCollector.Utilities;
using Caliburn.Micro;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Apis.Audible
{
    [Export(typeof(IImportController))]
    public class AudibleImportController : IImportController
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Uri audible_uri = new Uri(@"http://www.audible.com");
        private static readonly Uri amazon_uri = new Uri(@"http://www.amazon.com");

        private readonly AudibleApi api;
        private readonly ApplicationSettings application_settings;
        private readonly IEventAggregator event_aggregator;
        private readonly OffscreenBrowser offscreen_browser;
        private readonly Browser browser;
        private readonly IProgress<string> progress;

        public string ApiName { get { return api.Name; } }

        [ImportingConstructor]
        public AudibleImportController(AudibleApi api, ApplicationSettings application_settings, IEventAggregator event_aggregator, OffscreenBrowser offscreen_browser, Browser browser)
        {
            this.api = api;
            this.application_settings = application_settings;
            this.event_aggregator = event_aggregator;
            this.offscreen_browser = offscreen_browser;
            this.browser = browser;

            progress = new Progress<string>(str =>
            {
                logger.Trace(str);
                event_aggregator.PublishOnUIThread(ImportMessage.Information(str));
            });
        }

        public async void Start(ProfileDescription profile)
        {
            progress.Report("Initializing");
            await offscreen_browser.Ready;

            progress.Report("Loading main page");
            await offscreen_browser.Load(audible_uri.ToString(), Predicate);

            await Authenticate(profile);
            var result = await GetBooksAsync();

            event_aggregator.PublishOnUIThread(ImportMessage.Results(result));
        }

        private async Task Authenticate(ProfileDescription profile)
        {
            var credentials = application_settings.GetCredentials<AudibleCredentials>(profile.Id, api.Name) ?? new AudibleCredentials();
            var customer_id = await GetCustomerId();

            // Sign out if necessary
            if (!string.IsNullOrWhiteSpace(customer_id) && customer_id != credentials.CustomerId)
            {
                progress.Report("Signing out");
                var sign_out_url = await GetSignOutUrl();
                await offscreen_browser.Load(sign_out_url, Predicate);
                customer_id = string.Empty;
            }

            // Sign in if necessary
            if (string.IsNullOrWhiteSpace(customer_id))
            {
                progress.Report("Signing in");
                var sign_in_url = await GetSignInUrl();
                var sign_in_page_loaded = false;
                var sign_in_done = false;
                var tcs = browser.Show();
                await browser.Load(sign_in_url, s =>
                {
                    if (sign_in_page_loaded && !tcs.Task.IsCompleted)
                    {
                        sign_in_done = true;
                        tcs.SetResult(true);
                    }
                }, s =>
                {
                    if (Predicate(s))
                        sign_in_page_loaded = true;
                }, s => Predicate(s) && sign_in_done);
                await tcs.Task;

                progress.Report("Authentication done");

                // Save customer id if necessary
                if (string.IsNullOrWhiteSpace(credentials.CustomerId))
                {
                    await offscreen_browser.Load(audible_uri.ToString(), Predicate);
                    credentials.CustomerId= await GetCustomerId();
                    application_settings.AddCredentials(profile.Id, api.Name, credentials);
                }
            }
        }

        public async Task<List<ImportedBook>> GetBooksAsync()
        {
            progress.Report("Loading library page");
            var library_page_url = await GetLibraryPageUrl();
            await offscreen_browser.Load(library_page_url);

            progress.Report("Setting time filter");
            var time_filter_result = await offscreen_browser.Evaluate("document.getElementsByName('timeFilter')[0].value = 'all'");
            if (!time_filter_result.Success)
                throw new Exception();
            await offscreen_browser.Execute("document.getElementById('myLibraryForm').submit()", Predicate);

            progress.Report("Settings items filter");
            var items_filter_result = await offscreen_browser.Evaluate("document.getElementsByName('itemsPerPage')[0].value = document.getElementById('totalItems').value");
            if (!items_filter_result.Success)
                throw new Exception();
            await offscreen_browser.Execute("document.getElementById('myLibraryForm').submit()", Predicate);

            var books = await ParseBooks();
            return books.Select(Convert).ToList();
        }

        private ImportedBook Convert(AudibleBook book)
        {
            return new ImportedBook
            {
                Book = new Book
                {
                    Title = book.Title,
                    Description = book.Description,
                    Asin = book.Asin,
                    Authors = book.Authors,
                    Narrators = book.Narrators,
                    ImportSource = api.Name
                },
                ImageLinks = new List<ImageLink>
                {
                    new ImageLink(book.ImageUrl, "Image")
                }
            };
        }

        private async Task<List<AudibleBook>> ParseBooks()
        {
            progress.Report("Parsing books");
            var books = new List<AudibleBook>();
            var doc = await offscreen_browser.GetSourceAsDocument();
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

        private static bool Predicate(string url)
        {
            logger.Trace("Url: " + url);

            var uri = new Uri(url);
            return uri.Host == audible_uri.Host || uri.Host == amazon_uri.Host;
        }

        private async Task<string> GetCustomerId()
        {
            var text = await offscreen_browser.GetSource();
            var match = Regex.Match(text, @"'CustomerID'[,\s]*'(\S*)'");
            return match.Groups.Count == 2 ? match.Groups[1].Value : null;
        }

        private async Task<string> GetSignInUrl()
        {
            var doc = await offscreen_browser.GetSourceAsDocument();
            var link = doc.ChildLinkById("anon_header_v2_signin");
            var uri = new Uri(audible_uri, link);
            return uri.ToString();
        }

        private async Task<string> GetSignOutUrl()
        {
            var link = await offscreen_browser.Evaluate("document.getElementById('barker-sign-out').getAttribute('href')");
            var uri = new Uri(audible_uri, (string)link.Result);
            return uri.ToString();
        }

        private async Task<string> GetLibraryPageUrl()
        {
            var doc = await offscreen_browser.GetSourceAsDocument();
            var link = doc.ChildLinkById("library-menu");
            var uri = new Uri(audible_uri, link);
            return uri.ToString();
        }
    }
}
