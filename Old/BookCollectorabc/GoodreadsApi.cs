using System;
using System.Diagnostics;
using System.Net;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Contrib;

namespace BookCollector
{
    public class GoodreadsApi
    {
        public void Main()
        {
            const string consumer_key = "XJA3uzcNTPrWIJdfCHqVxQ";
            const string consumer_secret = "TnEHj2EUzSaMvfMfWSPpJA5lgrqK9Q7BxTpdhelhl4";
            const string base_url = @"https://www.goodreads.com";

            //Authenticate(base_url, consumer_key, consumer_secret);
            const string oauth_token = "v3TbMHXVHMoKusacp6Jkw";
            const string oauth_token_secret = "2jFh1ZeOyLOuvIEJfn4B933WuyOyUIlXHNmEL3HmBI";

            //GetUserId(base_url, consumer_key, consumer_secret, oauth_token, oauth_token_secret);
            const string user_id = "5445166";

            //GetBooks(base_url, user_id, consumer_key);
        }

        private static void GetBooks(string base_url, string user_id, string consumer_key)
        {
            var client = new RestClient(base_url);
            var request = new RestRequest("review/list");
            request.AddParameter("v", "2");
            request.AddParameter("id", user_id);
            request.AddParameter("key", consumer_key);
            var response = client.Execute(request);
        }

        private static void GetUserId(string base_url, string consumer_key, string consumer_secret, string oauth_token, string oauth_token_secret)
        {
            var client = new RestClient
            {
                BaseUrl = base_url,
                Authenticator = OAuth1Authenticator.ForProtectedResource(consumer_key, consumer_secret, oauth_token, oauth_token_secret)
            };
            client.AddHandler("application/xml", new CustomDeserializer());

            var request = new RestRequest("api/auth_user")
            {
                RequestFormat = DataFormat.Xml
            };
            var response = client.Execute<GoodreadsUser>(request);
        }

        private static void Authenticate(string base_url, string consumer_key, string consumer_secret)
        {
            var client = new RestClient
            {
                BaseUrl = base_url,
                Authenticator = OAuth1Authenticator.ForRequestToken(consumer_key, consumer_secret)
            };
            var request = new RestRequest("oauth/request_token", Method.POST);
            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Response: " + response.StatusDescription);

            var query_string = HttpUtility.ParseQueryString(response.Content);
            var oauth_token = query_string["oauth_token"];
            var oauth_token_secret = query_string["oauth_token_secret"];

            Console.WriteLine("Request token");
            Console.WriteLine("Token:  " + oauth_token);
            Console.WriteLine("Secret: " + oauth_token_secret);

            request = new RestRequest("oauth/authorize");
            request.AddParameter("oauth_token", oauth_token);
            var url = client.BuildUri(request).ToString();
            Process.Start(url);

            request = new RestRequest("oauth/access_token", Method.POST);
            client.Authenticator = OAuth1Authenticator.ForAccessToken(consumer_key, consumer_secret, oauth_token, oauth_token_secret);
            response = client.Execute(request);

            query_string = HttpUtility.ParseQueryString(response.Content);
            oauth_token = query_string["oauth_token"];
            oauth_token_secret = query_string["oauth_token_secret"];

            Console.WriteLine("Access token");
            Console.WriteLine("Token:  " + oauth_token);
            Console.WriteLine("Secret: " + oauth_token_secret);
        }
    }
}