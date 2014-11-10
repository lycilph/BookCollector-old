using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BookCollector.Services.Authentication;

namespace BookCollector.Services.GoogleBooks
{
    public class GoogleBooksAuthenticator : IAuthenticator
    {
        private static readonly Uri redirect_uri = new Uri(@"http://localhost:9327");

        private readonly IAuthenticationHandler handler;
        private readonly IProgress<string> progress;
        private readonly GoogleBooksApi api;

        public GoogleBooksAuthenticator(GoogleBooksApi api, IProgress<string> progress, IAuthenticationHandler handler)
        {
            this.api = api;
            this.progress = progress;
            this.handler = handler;
        }

        public async void Start()
        {
            var task = Task.Factory.StartNew(() => Listen())
                                   .ContinueWith(parent => RequestToken(parent.Result), TaskScheduler.FromCurrentSynchronizationContext());

            var uri = await Task.Factory.StartNew(() =>
            {
                progress.Report("Requesting authorization url");
                return api.RequestAuthorizationUrl(redirect_uri.ToString());
            });
            handler.Navigate(uri.ToString());

            await task;
        }

        private async void RequestToken(string code)
        {
            handler.NavigationDone();

            progress.Report("Requesting access token");
            var response = await Task.Factory.StartNew(() => api.RequestAccessToken(code, redirect_uri.ToString()));
            api.Settings.AccessToken = response.AccessToken;
            api.Settings.RefreshToken = response.RefreshToken;
            api.Settings.ExpiresIn = DateTime.Now.AddSeconds(response.ExpiresIn);

            progress.Report("Authorization done!");
            handler.AuthorizationDone();
        }

        public void Handle(Uri uri) { }

        private static string Listen()
        {
            var addr = IPAddress.Loopback;
            var listener = new TcpListener(addr, 9327);
            listener.Start();

            var tcp_client = listener.AcceptTcpClient();

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
            return code;
        }
    }
}
