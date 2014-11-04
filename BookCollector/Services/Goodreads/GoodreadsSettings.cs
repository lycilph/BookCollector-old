using BookCollector.Utilities;
using Newtonsoft.Json;

namespace BookCollector.Services.Goodreads
{
    public class GoodreadsSettings : IEncryptable<GoodreadsSettings>
    {
        // Settings
        public string UserId { get; set; }
        public string OAuthToken { get; set; }
        public string OAuthTokenSecret { get; set; }

        // Static data
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }

        [JsonIgnore]
        public bool IsAuthenticated
        {
            get { return !string.IsNullOrWhiteSpace(OAuthToken); }
        }

        public GoodreadsSettings()
        {
            UserId = string.Empty;
            OAuthToken = string.Empty;
            OAuthTokenSecret = string.Empty;
            ConsumerKey = string.Empty;
            ConsumerSecret = string.Empty;
        }

        public GoodreadsSettings Encrypt(string key)
        {
            var temp = new GoodreadsSettings
            {
                UserId = UserId,
                ConsumerKey = ConsumerKey.Encrypt(key),
                ConsumerSecret = ConsumerSecret.Encrypt(key)
            };
            if (!string.IsNullOrWhiteSpace(OAuthToken))
                temp.OAuthToken = OAuthToken.Encrypt(key);
            if (!string.IsNullOrWhiteSpace(OAuthTokenSecret))
                temp.OAuthTokenSecret = OAuthTokenSecret.Encrypt(key);
            return temp;
        }

        public GoodreadsSettings Decrypt(string key)
        {
            var temp = new GoodreadsSettings
            {
                UserId = UserId,
                ConsumerKey = ConsumerKey.Decrypt(key),
                ConsumerSecret = ConsumerSecret.Decrypt(key)
            };
            if (!string.IsNullOrWhiteSpace(OAuthToken))
                temp.OAuthToken = OAuthToken.Decrypt(key);
            if (!string.IsNullOrWhiteSpace(OAuthTokenSecret))
                temp.OAuthTokenSecret = OAuthTokenSecret.Decrypt(key);
            return temp;
        }
    }
}
