using System.Threading;
using Caliburn.Micro;
using CefSharp;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Services.Browsing
{
    public partial class BrowserView
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator event_aggregator;

        public BrowserView()
        {
            InitializeComponent();

            event_aggregator = IoC.Get<IEventAggregator>();
        }

        private void OnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            logger.Trace("Frame load end (current thread = {0}, url = {1})", Thread.CurrentThread.ManagedThreadId, e.Url);
            event_aggregator.PublishOnUIThread(BrowsingMessage.LoadEnd(e.Url));
        }

        private void OnFrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            logger.Trace("Frame load start (current thread = {0}, url = {1})", Thread.CurrentThread.ManagedThreadId, e.Url);
            event_aggregator.PublishOnUIThread(BrowsingMessage.LoadStart(e.Url));
        }
    }
}
