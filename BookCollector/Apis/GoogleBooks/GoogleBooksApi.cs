using System.ComponentModel.Composition;

namespace BookCollector.Apis.GoogleBooks
{
    [Export(typeof(IApi))]
    [Export(typeof(GoogleBooksApi))]
    public class GoogleBooksApi : IApi
    {
    //    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    //    private static readonly Uri authorization_url = new Uri(@"https://accounts.google.com");

    //    private const string base_url = @"https://www.googleapis.com";
    //    private const string authorization_scope = @"https://www.googleapis.com/auth/books";

    //    private readonly ApplicationSettings application_settings;
    //    private readonly RestClient client;

    //    private DateTime last_execution_time_stamp;

        public string Name { get { return "Google Books"; } }

    //    [ImportingConstructor]
    //    public GoogleBooksApi(ApplicationSettings application_settings) : base("Google Books")
    //    {
    //        this.application_settings = application_settings;

    //        client = new RestClient(base_url);

    //        last_execution_time_stamp = DateTime.Now.AddSeconds(-1);
    //    }

    //    private IRestResponse Execute(IRestRequest request)
    //    {
    //        Delay().Wait();
    //        var response = client.Execute(request);
    //        last_execution_time_stamp = DateTime.Now;

    //        return response;
    //    }

    //    private T Execute<T>(IRestRequest request) where T : new()
    //    {
    //        Delay().Wait();
    //        var response = client.Execute<T>(request);
    //        last_execution_time_stamp = DateTime.Now;

    //        return response.Data;
    //    }

    //    private Task Delay()
    //    {
    //        var now = DateTime.Now;
    //        var next_execution = last_execution_time_stamp.AddSeconds(1);
    //        var difference = next_execution.Subtract(now);
    //        var delay = (difference.Milliseconds > 0 ? difference.Milliseconds : 0);

    //        logger.Trace("Waiting for {0} ms", delay);

    //        return Task.Delay(delay);
    //    }

    //    public Uri RequestAuthorizationUrl(string redirect_uri)
    //    {
    //        client.BaseUrl = authorization_url;

    //        var request = new RestRequest("o/oauth2/auth");
    //        request.AddParameter("scope", authorization_scope);
    //        request.AddParameter("response_type", "code");
    //        request.AddParameter("client_id", Settings.ClientId);
    //        request.AddParameter("redirect_uri", redirect_uri);
    //        var response = Execute(request);

    //        return response.ResponseUri;
    //    }

    //    public GoogleBooksAuthorizationResponse RequestAccessToken(string code, string redirect_uri)
    //    {
    //        client.BaseUrl = authorization_url;

    //        var request = new RestRequest("o/oauth2/token", Method.POST);
    //        request.AddParameter("code", code);
    //        request.AddParameter("client_id", Settings.ClientId);
    //        request.AddParameter("client_secret", Settings.ClientSecret);
    //        request.AddParameter("redirect_uri", redirect_uri);
    //        request.AddParameter("grant_type", "authorization_code");
    //        var response = Execute<GoogleBooksAuthorizationResponse>(request);

    //        return response;
    //    }

    //    private void RefreshAccessToken()
    //    {
    //        logger.Trace("Refreshing access token");

    //        client.BaseUrl = authorization_url;

    //        var request = new RestRequest("o/oauth2/token", Method.POST);
    //        request.AddParameter("refresh_token", Settings.RefreshToken);
    //        request.AddParameter("client_id", Settings.ClientId);
    //        request.AddParameter("client_secret", Settings.ClientSecret);
    //        request.AddParameter("grant_type", "refresh_token");
    //        var response = Execute<GoogleBooksAuthorizationResponse>(request);

    //        Settings.AccessToken = response.AccessToken;
    //        Settings.ExpiresIn = DateTime.Now.AddSeconds(response.ExpiresIn);
    //    }

    //    private void Setup(string url)
    //    {
    //        // Refresh access token if necessary
    //        if (DateTime.Now.CompareTo(Settings.ExpiresIn) > 0)
    //            RefreshAccessToken();

    //        client.BaseUrl = new Uri(url);
    //    }

    //    public List<GoogleBook> GetBooks()
    //    {
    //        Setup(base_url);

    //        var request = new RestRequest("books/v1/mylibrary/bookshelves/7/volumes", Method.GET);
    //        request.AddHeader("Authorization", "OAuth " + Settings.AccessToken);
    //        var response = Execute<GoogleBooksCollection>(request);
    //        return response.Items;
    //    }
    }
}
