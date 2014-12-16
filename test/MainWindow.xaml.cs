using System;
using System.Net;
using System.Windows;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Contrib;

namespace test
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TestClick(object sender, RoutedEventArgs e)
        {
            var client = new RestClient(@"https://www.goodreads.com");
            client.Authenticator = OAuth1Authenticator.ForRequestToken(Settings.ConsumerKey, Settings.ConsumerSecret);

            var request = new RestRequest("oauth/request_token", Method.POST);
            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Response: " + response.StatusDescription);

            var query_string = HttpUtility.ParseQueryString(response.Content);
            var oauth_token = query_string["oauth_token"];
            var oauth_token_secret = query_string["oauth_token_secret"];

            request = new RestRequest("oauth/authorize");
            request.AddParameter("oauth_token", oauth_token);
            request.AddParameter("oauth_callback", @"http://bookcollector.com/oauth_callback");
            var url = client.BuildUri(request).ToString();
        }
    }
}
