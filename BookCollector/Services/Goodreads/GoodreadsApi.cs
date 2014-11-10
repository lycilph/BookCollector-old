﻿using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookCollector.Services.Settings;
using BookCollector.Utilities;
using NLog;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Contrib;

namespace BookCollector.Services.Goodreads
{
    [Export(typeof(GoodreadsApi))]
    public class GoodreadsApi : ApiBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private const string base_url = @"https://www.goodreads.com";

        private readonly ApplicationSettings application_settings;
        private readonly RestClient client;

        private DateTime last_execution_time_stamp;

        public override bool IsAuthenticated
        {
            get { return !string.IsNullOrWhiteSpace(application_settings.GoodreadsSettings.OAuthToken); }
        }

        public GoodreadsSettings Settings
        {
            get { return application_settings.GoodreadsSettings; }
        }

        [ImportingConstructor]
        public GoodreadsApi(ApplicationSettings application_settings) : base("Goodreads")
        {
            this.application_settings = application_settings;

            client = new RestClient(base_url);
            client.AddHandler("application/xml", new CustomDeserializer());

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

        public GoodreadsAuthorizationResponse RequestAuthorizationToken(string callback_uri)
        {
            client.Authenticator = OAuth1Authenticator.ForRequestToken(Settings.ConsumerKey, Settings.ConsumerSecret);

            var request = new RestRequest("oauth/request_token", Method.POST);
            var response = Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Response: " + response.StatusDescription);

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

        public GoodreadsAccessResponse RequestAccessToken(string token, string secret)
        {
            client.Authenticator = OAuth1Authenticator.ForAccessToken(Settings.ConsumerKey, Settings.ConsumerSecret, token, secret);

            var request = new RestRequest("oauth/access_token", Method.POST);
            var response = client.Execute(request);

            var query_string = HttpUtility.ParseQueryString(response.Content);
            return new GoodreadsAccessResponse
            {
                OAuthToken = query_string["oauth_token"],
                OAuthTokenSecret = query_string["oauth_token_secret"]
            };
        }

        public string GetUserId()
        {
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(Settings.ConsumerKey, Settings.ConsumerSecret, Settings.OAuthToken, Settings.OAuthTokenSecret);

            var request = new RestRequest("api/auth_user") { RequestFormat = DataFormat.Xml };
            var response = Execute<GoodreadsUser>(request);
            return response.Id;
        }

        public GoodreadsImportResponse GetBooks(int page, int items_per_page, string shelf)
        {
            client.Authenticator = OAuth1Authenticator.ForProtectedResource(Settings.ConsumerKey, Settings.ConsumerSecret, Settings.OAuthToken, Settings.OAuthTokenSecret);

            var request = new RestRequest("review/list");
            request.AddParameter("v", "2");
            request.AddParameter("id", Settings.UserId);
            request.AddParameter("page", page);
            request.AddParameter("per_page", items_per_page);
            request.AddParameter("shelf", shelf);
            var review_collection = Execute<GoodreadsReviewCollection>(request);

            return new GoodreadsImportResponse
            {
                Books = review_collection.Reviews.Select(r => r.Book),
                Total = review_collection.Total,
                Start = review_collection.Start,
                End = review_collection.End
            };
        }

        //public GoodreadsImportResponse GetBooks(int page, int items_per_page, string shelf)
        //{
        //    var settings = application_settings.GoodreadsSettings;
        //    var client = new RestClient
        //    {
        //        BaseUrl = base_url,
        //        Authenticator = OAuth1Authenticator.ForProtectedResource(settings.ConsumerKey, settings.ConsumerSecret, settings.OAuthToken, settings.OAuthTokenSecret)
        //    };
        //    client.AddHandler("application/xml", new CustomDeserializer());

        //    var request = new RestRequest("review/list");
        //    request.AddParameter("v", "2");
        //    request.AddParameter("id", settings.UserId);
        //    request.AddParameter("page", page);
        //    request.AddParameter("per_page", items_per_page);
        //    request.AddParameter("shelf", shelf);
        //    var review_collection = client.Execute<GoodreadsReviewCollection>(request).Data;

        //    return new GoodreadsImportResponse
        //    {
        //        Books = review_collection.Reviews.Select(r => r.Book),
        //        Total = review_collection.Total,
        //        Start = review_collection.Start,
        //        End = review_collection.End
        //    };
        //}

        //public List<GoodreadsBook> GetBooks(IProgress<GoodreadsProgressStatus> progress, int items_per_page)
        //{
        //    logger.Trace("Importing books");
        //    progress.Report(new GoodreadsProgressStatus("Importing books"));

        //    var settings = application_settings.GoodreadsSettings;
        //    var client = new RestClient
        //    {
        //        BaseUrl = base_url,
        //        Authenticator = OAuth1Authenticator.ForProtectedResource(settings.ConsumerKey, settings.ConsumerSecret, settings.OAuthToken, settings.OAuthTokenSecret)
        //    };
        //    client.AddHandler("application/xml", new CustomDeserializer());
        //    var request = new RestRequest("review/list");
        //    request.AddParameter("v", "2");
        //    request.AddParameter("id", settings.UserId);
        //    request.AddParameter("per_page", items_per_page);
        //    request.AddParameter("shelf", "all");
        //    var review_collection = client.Execute<GoodreadsReviewCollection>(request).Data;
        //    var books = review_collection.Reviews.Select(r => r.Book).ToList();
        //    var result = new List<GoodreadsBook>(books);

        //    logger.Trace(string.Format("{0} books found", result.Count));
        //    progress.Report(new GoodreadsProgressStatus(string.Format("{0} books found", result.Count), books));


        //    if (review_collection.End < review_collection.Total)
        //    {
        //        var pages = (int)Math.Ceiling((double)review_collection.Total / review_collection.End);
        //        for (var i = 2; i <= pages; i++)
        //        {
        //            Thread.Sleep(1000);

        //            request = new RestRequest("review/list");
        //            request.AddParameter("v", "2");
        //            request.AddParameter("id", settings.UserId);
        //            request.AddParameter("per_page", items_per_page);
        //            request.AddParameter("page", i);
        //            request.AddParameter("shelf", "all");
        //            review_collection = client.Execute<GoodreadsReviewCollection>(request).Data;
        //            books = review_collection.Reviews.Select(r => r.Book).ToList();
        //            result.AddRange(books);

        //            logger.Trace(string.Format("{0} books found", result.Count));
        //            progress.Report(new GoodreadsProgressStatus(string.Format("{0} books found", result.Count), books));
        //        }
        //    }

        //    logger.Trace("Importing done");
        //    progress.Report(new GoodreadsProgressStatus("Importing done"));

        //    return result;            
        //}

        //public Task<List<GoodreadsBook>> GetBooksAsync(IProgress<GoodreadsProgressStatus> progress, int items_per_page)
        //{
        //    return Task.Factory.StartNew(() => GetBooks(progress, items_per_page));
        //}
    }
}
