using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using HtmlAgilityPack;
using mshtml;

namespace WebBrowserTest
{
    public partial class MainWindow
    {
        private const string main_page_url = @"http://www.audible.com";
        private const string host_name = @"www.audible.com";

        public ObservableCollection<AudibleBook> Books { get; set; }

        private enum State
        {
            LoadingMainPage,
            LoggingIn,
            LoadingLibraryPage,
            SettingTimeFilter,
            SettingItemsFilter
        }

        private State current_state;

        public MainWindow()
        {
            InitializeComponent();
            Books = new ObservableCollection<AudibleBook>();

            Browser.LoadCompleted += BrowserOnLoadCompleted;
            DataContext = this;
        }

        private void BrowserOnLoadCompleted(object sender, NavigationEventArgs navigation_event_args)
        {
            System.Diagnostics.Debug.WriteLine("Load completed [{0}]: {1}", current_state, navigation_event_args.Uri);

            if (current_state == State.LoadingMainPage)
            {
                if (IsLoggedIn())
                    LoadLibraryPage();
                else
                    LoadLoginPage();
            }
            else if (current_state == State.LoggingIn)
            {
                if (navigation_event_args.Uri.Host == host_name)
                    LoadLibraryPage();
            }
            else if (current_state == State.LoadingLibraryPage)
            {
                SetTimeFilter();
            }
            else if (current_state == State.SettingTimeFilter)
            {
                SetItemsFilter();
            }
            else if (current_state == State.SettingItemsFilter)
            {
                ParseBooks();
                MainTabControl.SelectedIndex = 1;
                Browser.Visibility = Visibility.Visible;
            }
        }

        private void SetTimeFilter()
        {
            System.Diagnostics.Debug.WriteLine("Setting time filter");
            current_state = State.SettingTimeFilter;

            var document = (IHTMLDocument3)Browser.Document;
            var form = document.getElementById("myLibraryForm") as HTMLFormElement;
            if (form == null) return;

            foreach (var element in form)
            {
                var input = element as HTMLInputElement;
                if (input != null && input.name.ToLowerInvariant() == "timefilter")
                    input.value = "all";
            }
            form.submit();
        }

        private void SetItemsFilter()
        {
            System.Diagnostics.Debug.WriteLine("Setting items filter");
            current_state = State.SettingItemsFilter;

            var document = (IHTMLDocument3)Browser.Document;
            var form = document.getElementById("myLibraryForm") as HTMLFormElement;
            if (form == null) return;

            var total_items = document.getElementById("totalItems") as HTMLInputElement;
            if (total_items == null) return;

            foreach (var element in form)
            {
                var input = element as HTMLInputElement;
                if (input != null && input.name.ToLowerInvariant() == "itemsperpage")
                    input.value = total_items.value;
            }
            form.submit();
        }

        private void ParseBooks()
        {
            dynamic browser_doc = Browser.Document;
            var text = browser_doc.documentElement.InnerHtml;

            var doc = new HtmlDocument();
            doc.LoadHtml(text);

            var content = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'adbl-lib-content')]");
            foreach (var node in content.SelectNodes(".//tr"))
            {
                if (!node.HasChildNodes) continue;

                var inputs = node.SelectNodes(".//input");
                if (inputs == null) continue;

                var asin_node = inputs.SingleOrDefault(i => i.HasAttributes && i.Attributes.Contains("name") && i.Attributes["name"].Value.ToLowerInvariant() == "asin");
                if (asin_node == null) continue;

                var parent_asin_node = inputs.SingleOrDefault(i => i.HasAttributes && i.Attributes.Contains("name") && i.Attributes["name"].Value.ToLowerInvariant() == "parentasin");
                if (parent_asin_node == null) continue;

                var title_node = node.SelectNodes(".//a[@name]").SingleOrDefault(n => n.Attributes["name"].Value.ToLowerInvariant() == "tdtitle");
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
                    Books.Add(book);
                }
                else
                {
                    var parent = Books.SingleOrDefault(b => b.Asin == parent_asin);
                    if (parent != null)
                        parent.PartsAsin.Add(asin);
                }
            }
        }

        private void LoadLoginPage()
        {
            System.Diagnostics.Debug.WriteLine("Loading login page");

            current_state = State.LoggingIn;
            var url = GetLogInLink();
            Browser.Navigate(url);
        }

        private void LoadLibraryPage()
        {
            System.Diagnostics.Debug.WriteLine("Loading library page");

            current_state = State.LoadingLibraryPage;
            var url = GetLibraryLink();
            Browser.Navigate(url);
        }

        private void LoadMainPage()
        {
            System.Diagnostics.Debug.WriteLine("Loading main page");

            current_state = State.LoadingMainPage;
            Browser.Navigate(main_page_url);
        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            Browser.Visibility = Visibility.Hidden;
            LoadMainPage();
        }

        private string GetLibraryLink()
        {
            var document = (IHTMLDocument3)Browser.Document;
            var list_element = document.getElementById("library-menu");
            var item_element = list_element.children[0];
            var link = item_element.children[0] as HTMLAnchorElement;
            return link == null ? null : link.href;
        }

        private string GetLogInLink()
        {
            var document = (IHTMLDocument3)Browser.Document;
            var signin_element = document.getElementById("anon_header_v2_signin");
            var link = signin_element.children[0] as HTMLAnchorElement;
            return link == null ? null : link.href;
        }

        private bool IsLoggedIn()
        {
            var document = (IHTMLDocument3)Browser.Document;
            var signin_element = document.getElementById("anon_header_v2_signin");

            return signin_element == null;
        }
    }
}