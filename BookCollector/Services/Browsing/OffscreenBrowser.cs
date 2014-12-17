using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using CefSharp;
using CefSharp.OffScreen;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Services.Browsing
{
    public class OffscreenBrowser
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ChromiumWebBrowser offscreen_browser;
        private readonly IEventAggregator event_aggregator;

        public OffscreenBrowser()
        {
            event_aggregator = IoC.Get<IEventAggregator>();

            offscreen_browser = new ChromiumWebBrowser();
            offscreen_browser.FrameLoadStart += OffscreenBrowserOnFrameLoadStart;
            offscreen_browser.FrameLoadEnd += OffscreenBrowserOnFrameLoadEnd;
        }

        private void OffscreenBrowserOnFrameLoadEnd(object sender, FrameLoadEndEventArgs args)
        {
            logger.Trace("Frame load end (current thread = {0}, url = {1})", Thread.CurrentThread.ManagedThreadId, args.Url);
            event_aggregator.PublishOnUIThread(BrowsingMessage.LoadEnd(new Uri(args.Url)));
        }

        private void OffscreenBrowserOnFrameLoadStart(object sender, FrameLoadStartEventArgs args)
        {
            logger.Trace("Frame load start (current thread = {0}, url = {1})", Thread.CurrentThread.ManagedThreadId, args.Url);
            event_aggregator.PublishOnUIThread(BrowsingMessage.LoadStart(new Uri(args.Url)));
        }

        public void Load(string url)
        {
            offscreen_browser.Load(url);
        }

        public Task<JavascriptResponse> Evaluate(string script)
        {
            return offscreen_browser.EvaluateScriptAsync(script);
        }

        public void Execute(string script)
        {
            offscreen_browser.ExecuteScriptAsync(script);
        }

        public Task<string> GetSource()
        {
            return offscreen_browser.GetSourceAsync();
        }
    }
}
