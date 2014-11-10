using System;
using System.Threading.Tasks;
using BookCollector.Services.Authentication;
using RestSharp.Contrib;

namespace BookCollector.Services.Goodreads
{
    public class GoodreadsAuthenticator : IAuthenticator
    {
        private static readonly Uri callback_uri = new Uri(@"http://bookcollector.com/oauth_callback");

        private readonly IAuthenticationHandler handler;
        private readonly IProgress<string> progress;
        private readonly GoodreadsApi api;

        private GoodreadsAuthorizationResponse authorization_response;

        public GoodreadsAuthenticator(GoodreadsApi api, IProgress<string> progress, IAuthenticationHandler handler)
        {
            this.progress = progress;
            this.handler = handler;
            this.api = api;
        }

        public async void Start()
        {
            authorization_response = await Task.Factory.StartNew(() =>
            {
                progress.Report("Requesting authorization token");
                return api.RequestAuthorizationToken(callback_uri.ToString());
            });
            handler.Navigate(authorization_response.Url);
        }
        
        public async void Handle(Uri uri)
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
        }
    }
}
