namespace BookCollector.Api.Goodreads
{
    public class GoodreadsCredential
    {
        public string UserId { get; set; }
        public string OAuthToken { get; set; }
        public string OAuthTokenSecret { get; set; }

        public GoodreadsCredential() { }
        public GoodreadsCredential(GoodreadsAccessResponse access_response)
        {
            OAuthToken = access_response.OAuthToken;
            OAuthTokenSecret = access_response.OAuthTokenSecret;
        }
    }
}
