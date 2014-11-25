using System;
using System.Linq;
using BookCollector.Services.Authentication;
using BookCollector.Utilities;
using HtmlAgilityPack;
using NLog;

namespace BookCollector.Services.Audible
{
    public class AudibleAuthenticator : AuthenticatorBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly AudibleApi api;
        private readonly IAuthenticationHandler handler;

        private enum Mode { SignIn, Library, Ready }

        private Mode current_mode;

        public AudibleAuthenticator(AudibleApi api, IAuthenticationHandler handler)
        {
            this.api = api;
            this.handler = handler;
        }

        public override void Start()
        {
            current_mode = Mode.SignIn;
            handler.Navigate(api.BaseUrl);
        }

        public override void Navigating(Uri uri)
        {
            logger.Trace("Navigating to: " + uri);
        }

        public override void Navigated(Uri uri)
        {
            logger.Trace("Navigated to: " + uri);
        }

        public override void Loaded(string html)
        {
            logger.Trace("Loaded (Mode = {0})", current_mode);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var link = string.Empty;
            if (current_mode == Mode.SignIn)
            {
                link = GetSignInLink(doc);
                if (string.IsNullOrWhiteSpace(link))
                    current_mode = Mode.Library;                
            }

            switch (current_mode)
            {
                case Mode.SignIn:
                    handler.Navigate(link);
                    break;
                case Mode.Library:
                    link = GetLibraryLink(doc);
                    current_mode = Mode.Ready;
                    handler.Navigate(link);
                    break;
                case Mode.Ready:
                    HandleLibraryPage();
                    break;
            }
        }

        private void HandleLibraryPage()
        {
            var doc = handler.GetDocument();
            var element = doc.GetElementById("adbl_time_filter");

            foreach (var child in element.Children)
            {
                logger.Trace("Option - " + child.GetAttribute("value"));

                if (child.GetAttribute("value") == "all")
                    child.SetAttribute("selected", "selected");
            }

        }

        private string GetLibraryLink(HtmlDocument doc)
        {
            var node = doc.DocumentNode
                          .Descendants("a")
                          .WithAttributes("href", "title")
                          .FirstOrDefault(n => n.Attributes["title"].Value.ToLower() == "my books");

            if (node == null)
                return null;

            var link = HtmlEntity.DeEntitize(node.Attributes["href"].Value).Trim();
            var base_uri = new Uri(api.BaseUrl);
            var relative_uri = new Uri(link, UriKind.Relative);
            return new Uri(base_uri, relative_uri).ToString();
        }

        private string GetSignInLink(HtmlDocument doc)
        {
            var signin_node = doc.DocumentNode.SelectSingleNode("//div[@class='signin']");
            if (signin_node == null || signin_node.InnerText.Trim().ToLower() != "sign in")
                return null;

            var link_node = signin_node.SelectSingleNode("a");
            if (!link_node.HasAttributes || link_node.Attributes.All(a => a.Name != "href"))
                return null;

            var link = HtmlEntity.DeEntitize(link_node.Attributes["href"].Value).Trim();
            var base_uri = new Uri(api.BaseUrl);
            var relative_uri = new Uri(link, UriKind.Relative);
            return new Uri(base_uri, relative_uri).ToString();
        }
    }
}
