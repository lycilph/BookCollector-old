namespace BookCollector.Apis.GoodReads
{
    public class GoodReadsCredentials
    {
        public string UserId { get; set; }
        public string OAuthToken { get; set; }
        public string OAuthTokenSecret { get; set; }

        public GoodReadsCredentials() { }
        public GoodReadsCredentials(GoodReadsAccessResponse access_response)
        {
            OAuthToken = access_response.OAuthToken;
            OAuthTokenSecret = access_response.OAuthTokenSecret;
        }
    }
}
