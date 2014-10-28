using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RestSharp;

namespace GoogleBooksTest
{
    public partial class MainWindow
    {
        private const string client_id = "993143219615-eilhiq4e6jov2a071nakp46r29b1m0sr.apps.googleusercontent.com";
        private const string client_secret = "R5pQkEgJBulusJGOFDbIjdL2";
        private const string local_host = "http://localhost:9327";
        private const string filename = "user.txt";

        private GoogleUser user;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(folder, filename);
            if (File.Exists(path))
            {
                user = GoogleUser.Load(path, client_id);
                GetBooks();
            }
            else
            {
                user = await Authenticate();
                GetUserId();
                user.Save(path, client_id);
            }
        }

        public void GetBooks()
        {
            // Check timestamp of access token

            var client = new RestClient("https://www.googleapis.com");
            var request = new RestRequest("books/v1/mylibrary/bookshelves/7/volumes", Method.GET);
            request.AddHeader("Authorization", "OAuth " + user.AccessToken);
            var response = client.Execute<GoogleBooksCollection>(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                RefreshAccessToken();
                request = new RestRequest("books/v1/mylibrary/bookshelves/7/volumes", Method.GET);
                request.AddHeader("Authorization", "OAuth " + user.AccessToken);
                response = client.Execute<GoogleBooksCollection>(request);
            }
        }

        private void RefreshAccessToken()
        {
            var client = new RestClient("https://accounts.google.com");
            var request = new RestRequest("o/oauth2/token", Method.POST);
            request.AddParameter("refresh_token", user.RefreshToken);
            request.AddParameter("client_id", client_id);
            request.AddParameter("client_secret", client_secret);
            request.AddParameter("grant_type", "refresh_token");
            var response = client.Execute<GoogleResponse>(request);
            user.AccessToken = response.Data.AccessToken;
            user.AccessTokenTimeout = DateTime.Now.AddSeconds(response.Data.ExpiresIn);
        }

        private void GetUserId()
        {
            var client = new RestClient("https://www.googleapis.com");
            var request = new RestRequest("books/v1/mylibrary/bookshelves/7", Method.GET);
            request.AddHeader("Authorization", "OAuth " + user.AccessToken);
            var shelf = client.Execute<GoogleShelf>(request).Data;
            var uri = new Uri(shelf.SelfLink);
            user.UserId = uri.Segments[4];
        }

        private async Task<GoogleUser> Authenticate()
        {
            var client = new RestClient("https://accounts.google.com");
            var request = new RestRequest("o/oauth2/auth");
            request.AddParameter("scope", "https://www.googleapis.com/auth/books");
            request.AddParameter("response_type", "code");
            request.AddParameter("client_id", client_id);
            request.AddParameter("redirect_uri", local_host);
            var response = client.Execute(request);

            Browser.Navigate(response.ResponseUri);

            var addr = IPAddress.Loopback;
            var listener = new TcpListener(addr, 9327);
            listener.Start();

            var tcp_client = await listener.AcceptTcpClientAsync();
            
            var data = new byte[tcp_client.ReceiveBufferSize];
            string str;
            using (var ns = tcp_client.GetStream())
            {
                var read_count = ns.Read(data, 0, tcp_client.ReceiveBufferSize);
                str = Encoding.UTF8.GetString(data, 0, read_count);
            }

            listener.Stop();

            var elements = str.Split(' ');
            const string code_prefix = "/?code=";
            var code_element = elements.Single(element => element.StartsWith(code_prefix));
            var code = code_element.Substring(code_prefix.Length);

            request = new RestRequest("o/oauth2/token", Method.POST);
            request.AddParameter("code", code);
            request.AddParameter("client_id", client_id);
            request.AddParameter("client_secret", client_secret);
            request.AddParameter("redirect_uri", local_host);
            request.AddParameter("grant_type", "authorization_code");
            var google_response = client.Execute<GoogleResponse>(request).Data;

            return new GoogleUser
            {
                AccessToken = google_response.AccessToken,
                RefreshToken = google_response.RefreshToken,
                AccessTokenTimeout = DateTime.Now.AddSeconds(google_response.ExpiresIn)
            };
        }
    }
}
