using System;

namespace BookCollector.Services.GoogleBooks
{
    public class GoogleBooksAuthorizationResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
