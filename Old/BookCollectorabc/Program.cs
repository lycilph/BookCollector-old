using System;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using RestSharp;
using RestSharp.Contrib;

namespace BookCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RestClient("https://accounts.google.com");
            var request = new RestRequest("o/oauth2/auth");
            request.AddParameter("scope", "https://www.googleapis.com/auth/books");
            request.AddParameter("response_type", "code");
            request.AddParameter("client_id", "993143219615-eilhiq4e6jov2a071nakp46r29b1m0sr.apps.googleusercontent.com");
            request.AddParameter("redirect_uri", "urn:ietf:wg:oauth:2.0:oob");
            var response = client.Execute(request);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
