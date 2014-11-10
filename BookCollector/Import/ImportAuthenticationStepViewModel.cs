using System;
using System.Windows.Navigation;
using BookCollector.Services;
using BookCollector.Services.Authentication;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;

namespace BookCollector.Import
{
    public sealed class ImportAuthenticationStepViewModel : ReactiveScreen, IAuthenticationHandler
    {
        private readonly ImportViewModel parent;
        private readonly IProgress<string> progress;
        private IApi api;
        private IAuthenticator authenticator;

        private bool _IsBrowserVisible;
        public bool IsBrowserVisible
        {
            get { return _IsBrowserVisible; }
            set { this.RaiseAndSetIfChanged(ref _IsBrowserVisible, value); }
        }

        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsBusy, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _CanContinue;
        public bool CanContinue
        {
            get { return _CanContinue.Value; }
        }

        private string _Url;
        public string Url
        {
            get { return _Url; }
            set { this.RaiseAndSetIfChanged(ref _Url, value); }
        }

        private ReactiveList<string> _Messages = new ReactiveList<string>();
        public ReactiveList<string> Messages
        {
            get { return _Messages; }
            set { this.RaiseAndSetIfChanged(ref _Messages, value); }
        }

        public ImportAuthenticationStepViewModel(ImportViewModel parent)
        {
            this.parent = parent;
            DisplayName = "Authenticate";
            progress = new Progress<string>(s => Messages.Add(s));
            _CanContinue = this.WhenAny(x => x.IsBusy, x => !x.Value)
                               .ToProperty(this, x => x.CanContinue);
        }

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);

            if (authenticator == null) 
                throw new Exception();

            IsBusy = true;
            authenticator.Start();
        }

        public void Setup(IApi api)
        {
            this.api = api;
            authenticator = AuthenticatorFactory.Create(api, progress, this);
        }

        public void Navigate(string url)
        {
            IsBrowserVisible = true;
            Url = url;
        }

        public void NavigationDone()
        {
            IsBrowserVisible = false;
        }

        public void AuthorizationDone()
        {
            IsBusy = false;
        }

        public void Continue()
        {
            parent.Import(api);
        }

        public void Navigating(NavigatingCancelEventArgs args)
        {
            if (authenticator == null)
                return;

            authenticator.Handle(args.Uri);
        }
    }
}
