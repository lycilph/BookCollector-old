namespace BookCollector.Api.Goodreads
{
    public class GoodreadsAuthorizationResponse
    {
        public string OAuthToken { get; set; }
        public string OAuthTokenSecret { get; set; }
        public string Url { get; set; }
    }
}
