using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using HtmlAgilityPack;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Services.Browsing
{
    [Export(typeof(OffscreenBrowser))]
    public class OffscreenBrowser
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private const string blank_url = "about:blank";

        private readonly ChromiumWebBrowser offscreen_browser;

        private readonly TaskCompletionSource<bool> ready_task_completion_source = new TaskCompletionSource<bool>();
        public Task Ready
        {
            get { return ready_task_completion_source.Task; }
        }

        public OffscreenBrowser()
        {
            offscreen_browser = new ChromiumWebBrowser(blank_url);
            offscreen_browser.FrameLoadEnd += OffscreenBrowserOnFrameLoadEnd;
        }

        private void OffscreenBrowserOnFrameLoadEnd(object sender, FrameLoadEndEventArgs args)
        {
            if (args.Url == blank_url && args.IsMainFrame && !ready_task_completion_source.Task.IsCompleted)
            {
                logger.Trace("Offscreen browser ready");
                offscreen_browser.FrameLoadEnd -= OffscreenBrowserOnFrameLoadEnd;
                ready_task_completion_source.SetResult(true);
            }
        }

        public Task<string> Load(string url, Action<string> load_start, Action<string> load_end, Predicate<string> predicate = null)
        {
            var tcs = new TaskCompletionSource<string>();

            EventHandler<FrameLoadStartEventArgs> load_start_handler = null;
            load_start_handler = (obj, args) =>
            {
                if (load_start != null)
                    load_start(args.Url);
            };

            EventHandler<FrameLoadEndEventArgs> load_end_handler = null;
            load_end_handler = (obj, args) =>
            {
                if (load_end != null)
                    load_end(args.Url);

                if (predicate == null || predicate(args.Url) && args.IsMainFrame)
                {
                    offscreen_browser.FrameLoadStart -= load_start_handler;
                    offscreen_browser.FrameLoadEnd -= load_end_handler;
                    tcs.SetResult(args.Url);
                }
            };

            offscreen_browser.FrameLoadStart += load_start_handler;
            offscreen_browser.FrameLoadEnd += load_end_handler;
            offscreen_browser.Load(url);

            return tcs.Task;
        }

        public Task<string> Load(string url, Predicate<string> predicate = null)
        {
            return Load(url, s => logger.Trace("Load start: " + s), s => logger.Trace("Load end: " + s), predicate);
        }

        public Task<JavascriptResponse> Evaluate(string script)
        {
            return offscreen_browser.EvaluateScriptAsync(script);
        }

        public Task Execute(string script, Predicate<string> predicate = null)
        {
            var tcs = new TaskCompletionSource<bool>();

            EventHandler<FrameLoadEndEventArgs> load_end_handler = null;
            load_end_handler = (obj, args) =>
            {
                if (predicate == null || predicate(args.Url) && args.IsMainFrame)
                {
                    offscreen_browser.FrameLoadEnd -= load_end_handler;
                    tcs.SetResult(true);
                }
            };

            offscreen_browser.FrameLoadEnd += load_end_handler;
            offscreen_browser.ExecuteScriptAsync(script);

            return tcs.Task;
        }

        public Task<string> GetSource()
        {
            return offscreen_browser.GetSourceAsync();
        }

        public async Task<HtmlDocument> GetSourceAsDocument()
        {
            var source = await offscreen_browser.GetSourceAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(source);
            return doc;
        }
    }
}
