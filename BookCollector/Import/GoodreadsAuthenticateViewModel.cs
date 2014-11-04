using System;
using System.Threading.Tasks;
using System.Windows.Navigation;
using BookCollector.Services.Goodreads;
using BookCollector.Shell;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using NLog;
using ReactiveUI;
using IScreen = Caliburn.Micro.IScreen;
using LogManager = NLog.LogManager;

namespace BookCollector.Import
{
    public class GoodreadsAuthenticateViewModel : ReactiveScreen
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IEventAggregator event_aggregator;
        private readonly IScreen import_view_model;
        private readonly IProgress<string> progress;
        private readonly GoodreadsApi api;
        private GoodreadsAuthorizationResponse authorization_response;

        private string _Message;
        public string Message
        {
            get { return _Message; }
            set { this.RaiseAndSetIfChanged(ref _Message, value); }
        }

        private bool _ShowBrowser;
        public bool ShowBrowser
        {
            get { return _ShowBrowser; }
            set { this.RaiseAndSetIfChanged(ref _ShowBrowser, value); }
        }

        private string _Url;
        public string Url
        {
            get { return _Url; }
            set { this.RaiseAndSetIfChanged(ref _Url, value); }
        }

        public GoodreadsAuthenticateViewModel(GoodreadsApi api, IScreen import_view_model)
        {
            event_aggregator = IoC.Get<IEventAggregator>();
            progress = new Progress<string>(s => Message = s);
            this.api = api;
            this.import_view_model = import_view_model;
        }

        protected override async void OnViewLoaded(object view)
        {
            logger.Trace("Initializing");

            base.OnInitialize();

            authorization_response = await Task.Factory.StartNew(() => api.RequestAuthorizationToken(progress));
            ShowBrowser = true;
            Url = authorization_response.Url;
        }

        public void Navigating(NavigatingCancelEventArgs args)
        {
            var task = new Task(() =>
            {
                api.RequestAccessToken(authorization_response, progress);
                api.GetUserId(progress);
            });
            task.ContinueWith(async _ =>
            {
                ShowBrowser = false;
                await Task.Delay(1000);
                event_aggregator.PublishOnCurrentThread(ShellMessage.BackMessage());
                event_aggregator.PublishOnCurrentThread(ShellMessage.ShowMessage(import_view_model));
            }, TaskScheduler.FromCurrentSynchronizationContext());

            if (api.HandleAuthorizationCallback(args.Uri, task))
                args.Cancel = true;
        }
    }
}
