using System.Threading.Tasks;
using Framework.Core.Dialogs;

namespace BookCollector.Services.Browsing
{
    public static class BrowserController
    {
        private static readonly BrowserViewModel BrowserViewModel = new BrowserViewModel();

        public static TaskCompletionSource<bool> ShowOverlayBrowser()
        {
            var tcs = new TaskCompletionSource<bool>();
            BrowserViewModel.TaskCompletionSource = tcs;
            DialogController.ShowContent(BrowserViewModel);
            return tcs;
        }

        public static void Navigate(string url)
        {
            BrowserViewModel.Url = url;
        }

        public static TaskCompletionSource<bool> ShowAndNavigate(string url)
        {
            var tcs = ShowOverlayBrowser();
            Navigate(url);
            return tcs;
        }
    }
}
