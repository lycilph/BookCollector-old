using System.Threading.Tasks;
using CefSharp;
using Framework.Core.Dialogs;
using HtmlAgilityPack;

namespace BookCollector.Services.Browsing
{
    public static class BrowserController
    {
        private static readonly BrowserViewModel browser_view_model = new BrowserViewModel();
        private static readonly OffscreenBrowser offscreen_browser = new OffscreenBrowser();

        public static TaskCompletionSource<bool> ShowOverlayBrowser()
        {
            var tcs = new TaskCompletionSource<bool>();
            browser_view_model.TaskCompletionSource = tcs;
            DialogController.ShowContent(browser_view_model);
            return tcs;
        }

        public static void Navigate(string url)
        {
            browser_view_model.Url = url;
        }

        public static TaskCompletionSource<bool> ShowAndNavigate(string url)
        {
            var tcs = ShowOverlayBrowser();
            Navigate(url);
            return tcs;
        }

        public static void NavigateOffscreen(string url)
        {
            offscreen_browser.Load(url);
        }

        public static Task<JavascriptResponse> EvaluateOffscreen(string script)
        {
            return offscreen_browser.Evaluate(script);
        }

        public static void ExecuteOffscreen(string script)
        {
            offscreen_browser.Execute(script);
        }

        public static async Task<string> GetOffscreenSourceAsText()
        {
            return await offscreen_browser.GetSource();
        }

        public static async Task<HtmlDocument> GetOffscreenSourceAsDocument()
        {
            var source = await offscreen_browser.GetSource();
            var doc = new HtmlDocument();
            doc.LoadHtml(source);
            return doc;
        }
    }
}
