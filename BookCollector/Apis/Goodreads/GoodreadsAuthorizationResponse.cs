namespace BookCollector.Apis.GoodReads
{
    public class GoodReadsAuthorizationResponse
    {
        public string OAuthToken { get; set; }
        public string OAuthTokenSecret { get; set; }
        public string Url { get; set; }
    }
}
