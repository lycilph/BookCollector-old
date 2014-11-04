using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BookCollector.Utilities;
using NLog;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Contrib;

namespace BookCollector.Services.Goodreads
{
    [Export(typeof(GoodreadsApi))]
    public class GoodreadsApi
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Uri callback_uri = new Uri(@"http://bookcollector.com/oauth_callback");

        private const string base_url = @"https://www.goodreads.com";

        private GoodreadsSettings settings;

        public bool IsAuthorized
        {
            get { return !string.IsNullOrWhiteSpace(settings.OAuthToken); }
        }

        [ImportingConstructor]
        public GoodreadsApi(ApplicationSettings application_settings)
        {
            application_settings.Loaded += (sender, args) => settings = application_settings.GoodreadsSettings;
        }

        public void RequestAccessToken(GoodreadsAuthorizationResponse authorization_response, IProgress<string> progress)
        {
            logger.Trace("Requesting access token");
            var client = new RestClient
            {
                BaseUrl = base_url,
                Authenticator = OAuth1Authenticator.ForAccessToken(settings.ConsumerKey, settings.ConsumerSecret, authorization_response.OAuthToken, authorization_response.OAuthTokenSecret)
            };
            var request = new RestRequest("oauth/access_token", Method.POST);
            var response = client.Execute(request);

            var query_string = HttpUtility.ParseQueryString(response.Content);
            settings.OAuthToken = query_string["oauth_token"];
            settings.OAuthTokenSecret = query_string["oauth_token_secret"];
        }

        public bool HandleAuthorizationCallback(Uri uri, Task task)
        {
            if (uri.Host == callback_uri.Host && uri.AbsolutePath == callback_uri.AbsolutePath)
            {
                var query_string = HttpUtility.ParseQueryString(uri.Query);
                if (query_string["authorize"] == "1")
                {
                    task.Start();
                    return true;
                }
            }
            return false;
        }

        public GoodreadsAuthorizationResponse RequestAuthorizationToken(IProgress<string> progress)
        {
            var client = new RestClient
            {
                BaseUrl = base_url,
                Authenticator = OAuth1Authenticator.ForRequestToken(settings.ConsumerKey, settings.ConsumerSecret)
            };

            logger.Trace("Requesting token");
            progress.Report("Requesting Token");
            var request = new RestRequest("oauth/request_token", Method.POST);
            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Response: " + response.StatusDescription);

            logger.Trace("Token received");
            progress.Report("Token received");

            var query_string = HttpUtility.ParseQueryString(response.Content);
            var oauth_token = query_string["oauth_token"];
            var oauth_token_secret = query_string["oauth_token_secret"];

            request = new RestRequest("oauth/authorize");
            request.AddParameter("oauth_token", oauth_token);
            request.AddParameter("oauth_callback", callback_uri);
            var url = client.BuildUri(request).ToString();

            return new GoodreadsAuthorizationResponse
            {
                OAuthToken = oauth_token,
                OAuthTokenSecret = oauth_token_secret,
                Url = url
            };
        }

        public void GetUserId(IProgress<string> progress)
        {
            logger.Trace("Getting user id");
            progress.Report("Getting user id");

            var client = new RestClient
            {
                BaseUrl = base_url,
                Authenticator = OAuth1Authenticator.ForProtectedResource(settings.ConsumerKey, settings.ConsumerSecret, settings.OAuthToken, settings.OAuthTokenSecret)
            };
            client.AddHandler("application/xml", new CustomDeserializer());
            var request = new RestRequest("api/auth_user") { RequestFormat = DataFormat.Xml };
            var response = client.Execute<GoodreadsUser>(request);
            settings.UserId = response.Data.Id;
        }

        public Task<List<GoodreadsBook>> GetBooksAsync(IProgress<string> progress, int items_per_page)
        {
            return Task.Factory.StartNew(() =>
            {
                logger.Trace("Importing books");
                progress.Report("Importing books");

                var client = new RestClient
                {
                    BaseUrl = base_url,
                    Authenticator = OAuth1Authenticator.ForProtectedResource(settings.ConsumerKey, settings.ConsumerSecret, settings.OAuthToken, settings.OAuthTokenSecret)
                };
                client.AddHandler("application/xml", new CustomDeserializer());
                var request = new RestRequest("review/list");
                request.AddParameter("v", "2");
                request.AddParameter("id", settings.UserId);
                request.AddParameter("per_page", items_per_page);
                request.AddParameter("shelf", "all");
                var review_collection = client.Execute<GoodreadsReviewCollection>(request).Data;
                var result = new List<GoodreadsBook>(review_collection.Reviews.Select(r => r.Book));

                logger.Trace(string.Format("{0} books found", result.Count));
                progress.Report(string.Format("{0} books found", result.Count));

                if (review_collection.End < review_collection.Total)
                {
                    var pages = (int) Math.Ceiling((double) review_collection.Total/review_collection.End);
                    for (var i = 2; i <= pages; i++)
                    {
                        Thread.Sleep(1000);

                        request = new RestRequest("review/list");
                        request.AddParameter("v", "2");
                        request.AddParameter("id", settings.UserId);
                        request.AddParameter("per_page", items_per_page);
                        request.AddParameter("page", i);
                        request.AddParameter("shelf", "all");
                        review_collection = client.Execute<GoodreadsReviewCollection>(request).Data;
                        result.AddRange(review_collection.Reviews.Select(r => r.Book));

                        logger.Trace(string.Format("{0} books found", result.Count));
                        progress.Report(string.Format("{0} books found", result.Count));
                    }
                }

                logger.Trace("Importing done");
                progress.Report("Importing done");

                return result;
            });
        }
    }
}
