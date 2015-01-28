using System.ComponentModel.Composition;

namespace BookCollector.Services.Browsing
{
    [Export(typeof(BrowserService))]
    public class BrowserService
    {
        private readonly Browser browser;
        private readonly OffscreenBrowser offscreen_browser;

        public Browser Browser { get { return browser; } }

        public OffscreenBrowser OffscreenBrowser { get { return offscreen_browser; } }

        [ImportingConstructor]
        public BrowserService(Browser browser, OffscreenBrowser offscreen_browser)
        {
            this.browser = browser;
            this.offscreen_browser = offscreen_browser;
        }
    }
}
