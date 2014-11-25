using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Services.Goodreads;
using Caliburn.Micro;

namespace BookCollector.Import
{
    [Export(typeof(IImportController))]
    public class GoodreadsImportController : IImportController
    {
        private static readonly Uri callback_uri = new Uri(@"http://bookcollector.com/oauth_callback");

        private readonly GoodreadsApi api;
        private readonly ImportInformationViewModel information;
        private GoodreadsAuthorizationResponse authorization_response;

        public string Name { get { return "Goodreads"; } }

        private readonly List<ImportStepViewModel> _Steps;
        public IEnumerable<ImportStepViewModel> Steps
        {
            get { return _Steps; }
        }

        [ImportingConstructor]
        public GoodreadsImportController(GoodreadsApi api, ImportInformationViewModel information)
        {
            this.api = api;
            this.information = information;

            _Steps = new List<ImportStepViewModel>
            {
                new ImportStepViewModel("Check authentication"),
                new ImportStepViewModel("Request authorization token"),
                new ImportStepViewModel("Import books")
            };
        }

        private void ResetSteps()
        {
            Steps.Apply(s =>
            {
                s.IsActive = false;
                s.IsEnabled = true;
            });
        }

        private void ActivateStep(int i)
        {
            Steps.Apply(s => s.IsActive = false);
            _Steps[i].IsActive = true;
        }

        public async void Start()
        {
            ActivateStep(0);
            information.AddMessage("Checking authentication");

            if (api.IsAuthenticated)
            {
                ActivateStep(2);
                information.AddMessage("Fetching data");
                information.AddMessage("Parsing data");
            }
            else
            {
                ActivateStep(1);
                information.AddMessage("Requesting authorization token");
                authorization_response = await api.RequestAuthorizationTokenAsync(callback_uri.ToString());
            }
            

            

            /*public override async void Start()
        {
            authorization_response = await Task.Factory.StartNew(() =>
            {
                progress.Report("Requesting authorization token");
                return api.RequestAuthorizationToken(callback_uri.ToString());
            });
            handler.Navigate(authorization_response.Url);
        }
        
        public override async void Navigating(Uri uri)
        {
            if (uri.Host != callback_uri.Host || uri.AbsolutePath != callback_uri.AbsolutePath) 
                return;

            var settings = api.Settings;
            var query_string = HttpUtility.ParseQueryString(uri.Query);
            if (query_string["authorize"] != "1")
                return;

            handler.NavigationDone();

            progress.Report("Requesting access token");
            var access_response = await Task.Factory.StartNew(() => api.RequestAccessToken(authorization_response.OAuthToken, authorization_response.OAuthTokenSecret));
            settings.OAuthToken = access_response.OAuthToken;
            settings.OAuthTokenSecret = access_response.OAuthTokenSecret;

            progress.Report("Requesting user id");
            var user_response = await Task.Factory.StartNew(() => api.GetUserId());
            settings.UserId = user_response;

            progress.Report("Authorization done!");
            handler.AuthorizationDone();
        }*/
        }
    }
}
