using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using BookCollector.Model;
using BookCollector.Screens.Import;
using BookCollector.Services;
using BookCollector.Services.Browsing;
using BookCollector.Utilities;
using Caliburn.Micro;
using NLog;
using LogManager = NLog.LogManager;

namespace BookCollector.Apis.Audible
{
    [Export(typeof(IImportController))]
    public class AudibleImportController : IImportController, IHandle<BrowsingMessage>
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Uri audible_uri = new Uri(@"http://www.audible.com");
        private static readonly Uri amazon_uri = new Uri(@"http://www.amazon.com");

        private enum State
        {
            LoadingMainPage,
            LoadingSignInPage,
            SigningIn,
            SigningOut
        }

        private readonly AudibleApi api;
        private readonly ApplicationSettings application_settings;
        private readonly IEventAggregator event_aggregator;
        private readonly IProgress<string> progress;
        private State current_state;
        private ProfileDescription current_profile;
        private TaskCompletionSource<bool> tcs;

        public string ApiName { get { return api.Name; } }

        [ImportingConstructor]
        public AudibleImportController(AudibleApi api, ApplicationSettings application_settings, IEventAggregator event_aggregator)
        {
            this.api = api;
            this.application_settings = application_settings;
            this.event_aggregator = event_aggregator;

            progress = new Progress<string>(str =>
            {
                logger.Trace(str);
                event_aggregator.PublishOnUIThread(ImportMessage.Information(str));
            });
        }

        public void Start(ProfileDescription profile)
        {
            current_profile = profile;
            event_aggregator.Subscribe(this);

            LoadMainPage();
        }

        public void Handle(BrowsingMessage message)
        {
            switch (message.Kind)
            {
                case BrowsingMessage.MessageKind.LoadStart:
                    LoadStart(message.Uri);
                    break;
                case BrowsingMessage.MessageKind.LoadEnd:
                    if (message.Uri.Host == audible_uri.Host || message.Uri.Host == amazon_uri.Host)
                        LoadEnd(message.Uri);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void LoadEnd(Uri uri)
        {
            switch (current_state)
            {
                case State.LoadingMainPage:
                    HandleLoadingMainPage();
                    break;
                case State.LoadingSignInPage:
                    HandleLoadingSignInPage();
                    break;
                case State.SigningIn:
                    HandleSigningIn();
                    break;
                case State.SigningOut:
                    HandleSigningOut();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void LoadStart(Uri uri)
        {
            switch (current_state)
            {
                case State.SigningIn:
                    HandleStartSigningIn();
                    break;
            }
        }

        private void LoadMainPage()
        {
            progress.Report("Loading main page");
            
            current_state = State.LoadingMainPage;
            BrowserController.NavigateOffscreen(audible_uri.ToString());
        }

        private async void HandleLoadingMainPage()
        {
            progress.Report("Main page loaded");

            var credentials = application_settings.GetCredentials<AudibleCredentials>(current_profile.Id, api.Name);
            var name = await AudibleImportHelper.GetSignInName();

            logger.Trace("Signed in as: " + name);

            if (string.IsNullOrWhiteSpace(name))
            {
                SignIn(); // The user is not is not signed in
            }
            else
            {
                if (credentials == null || name != credentials.LoginName)
                    SignOut(); // Another user is signed in
                //else
                //    LoadLibraryPage(); // The User is already signed in
            }
        }

        private async void SignIn()
        {
            progress.Report("Loading sign in page");

            var doc = await BrowserController.GetOffscreenSourceAsDocument();
            var link = doc.ChildLinkById("anon_header_v2_signin");
            var sign_in_uri = new Uri(audible_uri, link);

            logger.Trace("login page url: " + sign_in_uri);

            current_state = State.LoadingSignInPage;
            tcs = BrowserController.ShowAndNavigate(sign_in_uri.ToString());
        }

        private void HandleLoadingSignInPage()
        {
            progress.Report("Sign in page loaded");
            current_state = State.SigningIn;
        }

        private void HandleStartSigningIn()
        {
            if (tcs.Task.IsCompleted) 
                return;

            logger.Trace("Hiding browser");
            tcs.SetResult(true);
        }

        private async void HandleSigningIn()
        {
            progress.Report("Signing in done");

            var name = await AudibleImportHelper.GetSignInName();
            logger.Trace("Signed in as: " + name);
        }

        private async void SignOut()
        {
            progress.Report("Signing out");

            var link = await BrowserController.EvaluateOffscreen("document.getElementById('barker-sign-out').getAttribute('href')");
            var sign_out_uri = new Uri(audible_uri, (string)link.Result);

            logger.Trace("Sign out page url: " + sign_out_uri);

            current_state = State.SigningOut;
            BrowserController.NavigateOffscreen(sign_out_uri.ToString());
        }

        private async void HandleSigningOut()
        {
            progress.Report("Signed out");

            await Task.Delay(5000);
            LoadMainPage();
        }
    }
}
