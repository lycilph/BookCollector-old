using System;

namespace BookCollector.Apis.GoogleBooks
{
    public class GoogleBooksCredentials
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresIn { get; set; }

        public GoogleBooksCredentials() { }
        public GoogleBooksCredentials(GoogleBooksAuthorizationResponse response)
        {
            AccessToken = response.AccessToken;
            RefreshToken = response.RefreshToken;
            ExpiresIn = DateTime.Now.AddSeconds(response.ExpiresIn - 60); // Subtract 1 min, to be on the safe side
        }

        public void Update(GoogleBooksAuthorizationResponse response)
        {
            AccessToken = response.AccessToken;
            ExpiresIn = DateTime.Now.AddSeconds(response.ExpiresIn - 60); // Subtract 1 min, to be on the safe side
        }
    }
}
