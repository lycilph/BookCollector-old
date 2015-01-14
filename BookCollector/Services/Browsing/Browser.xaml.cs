using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using Framework.Core.Dialogs;
using NLog;

namespace BookCollector.Services.Browsing
{
    [Export(typeof(Browser))]
    public partial class Browser : IHaveDoneTask
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private const string blank_url = "about:blank";

        private readonly TaskCompletionSource<bool> ready_task_completion_source = new TaskCompletionSource<bool>();

        private TaskCompletionSource<bool> dialog_task_completion_source;
        public Task Done
        {
            get { return dialog_task_completion_source.Task; }
        }

        public Browser()
        {
            InitializeComponent();

            WpfBrowser.Address = blank_url;
            WpfBrowser.FrameLoadEnd += BrowserOnFrameLoadEnd;
        }

        private void BrowserOnFrameLoadEnd(object sender, FrameLoadEndEventArgs args)
        {
            if (args.Url == blank_url && args.IsMainFrame && !ready_task_completion_source.Task.IsCompleted)
            {
                WpfBrowser.FrameLoadEnd -= BrowserOnFrameLoadEnd;
                ready_task_completion_source.SetResult(true);
            }
        }

        public TaskCompletionSource<bool> Show()
        {
            dialog_task_completion_source = new TaskCompletionSource<bool>();
            DialogController.ShowView(this);
            return dialog_task_completion_source;
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
                    WpfBrowser.FrameLoadStart -= load_start_handler;
                    WpfBrowser.FrameLoadEnd -= load_end_handler;
                    tcs.SetResult(args.Url);
                }
            };

            ready_task_completion_source.Task.ContinueWith(parent =>
            {
                WpfBrowser.FrameLoadStart += load_start_handler;
                WpfBrowser.FrameLoadEnd += load_end_handler;
                WpfBrowser.Address = url;
            }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

            return tcs.Task;
        }

        public Task<string> Load(string url, Predicate<string> predicate = null)
        {
            return Load(url, s => logger.Trace("Load start: " + s), s => logger.Trace("Load end: " + s), predicate);
        }
    }
}
