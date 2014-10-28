using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace GoogleBooksTest
{
    public class GoogleUser
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public DateTime AccessTokenTimeout { get; set; }
        public string UserId { get; set; }

        public GoogleUser() { }

        public GoogleUser(GoogleUser user)
        {
            RefreshToken = user.RefreshToken;
            AccessToken = user.AccessToken;
            AccessTokenTimeout = user.AccessTokenTimeout;
            UserId = user.UserId;
        }

        public void Save(string filename, string encryption_key)
        {
            var temp = new GoogleUser(this)
            {
                RefreshToken = Encrypt(RefreshToken, encryption_key),
                AccessToken = Encrypt(AccessToken, encryption_key)
            };
            var json = JsonConvert.SerializeObject(temp, Formatting.Indented);
            File.WriteAllText(filename, json);
        }

        public static GoogleUser Load(string filename, string encryption_key)
        {
            var json = File.ReadAllText(filename);
            var temp = JsonConvert.DeserializeObject<GoogleUser>(json);
            return new GoogleUser(temp)
            {
                AccessToken = Decrypt(temp.AccessToken, encryption_key),
                RefreshToken = Decrypt(temp.RefreshToken, encryption_key)
            };
        }

        private static string Encrypt(string text, string key)
        {
            var text_array = Encoding.UTF8.GetBytes(text);

            var hashmd5 = new MD5CryptoServiceProvider();
            var key_array = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();

            // Set the secret key for the tripleDES algorithm
            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = key_array,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            // Transform the specified region of bytes array to resultArray
            var transform = tdes.CreateEncryptor();
            var result_array = transform.TransformFinalBlock(text_array, 0, text_array.Length);
            tdes.Clear();

            // Return the encrypted data into unreadable string format
            return Convert.ToBase64String(result_array, 0, result_array.Length);
        }

        private static string Decrypt(string text, string key)
        {
            var text_array = Convert.FromBase64String(text);

            // If hashing was used get the hash code with regards to your key
            var hashmd5 = new MD5CryptoServiceProvider();
            var key_array = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();

            // Set the secret key for the tripleDES algorithm
            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = key_array,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var transform = tdes.CreateDecryptor();
            var result_array = transform.TransformFinalBlock(text_array, 0, text_array.Length);
            tdes.Clear();

            // Return the Clear decrypted TEXT
            return Encoding.UTF8.GetString(result_array);
        }
    }
}
