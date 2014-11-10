using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using BookCollector.Services.Settings;
using NLog;
using RestSharp;

namespace BookCollector.Services.GoogleBooks
{
    [Export(typeof(GoogleBooksApi))]
    public class GoogleBooksApi : ApiBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private const string authorization_url = @"https://accounts.google.com";
        private const string authorization_scope = "https://www.googleapis.com/auth/books";

        private readonly ApplicationSettings application_settings;
        private readonly RestClient client;

        private DateTime last_execution_time_stamp;


        public override bool IsAuthenticated
        {
            get { return !string.IsNullOrWhiteSpace(application_settings.GoogleBooksSettings.AccessToken); }
        }

        public GoogleBooksSettings Settings
        {
            get { return application_settings.GoogleBooksSettings; }
        }

        [ImportingConstructor]
        public GoogleBooksApi(ApplicationSettings application_settings) : base("Google Books")
        {
            this.application_settings = application_settings;

            client = new RestClient(authorization_url);

            last_execution_time_stamp = DateTime.Now.AddSeconds(-1);
        }

        private IRestResponse Execute(IRestRequest request)
        {
            Delay().Wait();
            var response = client.Execute(request);
            last_execution_time_stamp = DateTime.Now;

            return response;
        }

        private T Execute<T>(IRestRequest request) where T : new()
        {
            Delay().Wait();
            var response = client.Execute<T>(request);
            last_execution_time_stamp = DateTime.Now;

            return response.Data;
        }

        private Task Delay()
        {
            var now = DateTime.Now;
            var next_execution = last_execution_time_stamp.AddSeconds(1);
            var difference = next_execution.Subtract(now);
            var delay = (difference.Milliseconds > 0 ? difference.Milliseconds : 0);

            logger.Trace("Waiting for {0} ms", delay);

            return Task.Delay(delay);
        }

        public Uri RequestAuthorizationUrl(string redirect_uri)
        {
            var request = new RestRequest("o/oauth2/auth");
            request.AddParameter("scope", authorization_scope);
            request.AddParameter("response_type", "code");
            request.AddParameter("client_id", Settings.ClientId);
            request.AddParameter("redirect_uri", redirect_uri);
            var response = Execute(request);
            return response.ResponseUri;
        }

        public GoogleBooksAuthorizationResponse RequestAccessToken(string code, string redirect_uri)
        {
            var request = new RestRequest("o/oauth2/token", Method.POST);
            request.AddParameter("code", code);
            request.AddParameter("client_id", Settings.ClientId);
            request.AddParameter("client_secret", Settings.ClientSecret);
            request.AddParameter("redirect_uri", redirect_uri);
            request.AddParameter("grant_type", "authorization_code");
            return Execute<GoogleBooksAuthorizationResponse>(request);
        }
    }
}
