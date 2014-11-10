using System;
using BookCollector.Utilities;

namespace BookCollector.Services.Settings
{
    public class GoogleBooksSettings : IEncryptable<GoogleBooksSettings>
    {
        // Settings
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresIn { get; set; }

        // Static data
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public GoogleBooksSettings()
        {
            AccessToken = string.Empty;
            RefreshToken = string.Empty;
            ExpiresIn = DateTime.Now;
            ClientId = string.Empty;
            ClientSecret = string.Empty;
        }

        public GoogleBooksSettings Encrypt(string key)
        {
            var temp = new GoogleBooksSettings
            {
                ExpiresIn = ExpiresIn,
                ClientId = ClientId.Encrypt(key),
                ClientSecret = ClientSecret.Encrypt(key)
            };
            if (!string.IsNullOrWhiteSpace(AccessToken))
                temp.AccessToken = AccessToken.Encrypt(key);
            if (!string.IsNullOrWhiteSpace(RefreshToken))
                temp.RefreshToken = RefreshToken.Encrypt(key);
            return temp;
        }

        public GoogleBooksSettings Decrypt(string key)
        {
            var temp = new GoogleBooksSettings
            {
                ExpiresIn = ExpiresIn,
                ClientId = ClientId.Decrypt(key),
                ClientSecret = ClientSecret.Decrypt(key)
            };
            if (!string.IsNullOrWhiteSpace(AccessToken))
                temp.AccessToken = AccessToken.Decrypt(key);
            if (!string.IsNullOrWhiteSpace(RefreshToken))
                temp.RefreshToken = RefreshToken.Decrypt(key);
            return temp;
        }
    }
}
