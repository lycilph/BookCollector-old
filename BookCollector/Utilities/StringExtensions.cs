using System;
using System.Security.Cryptography;
using System.Text;

namespace BookCollector.Utilities
{
    public static class StringExtensions
    {
        public static string Encrypt(this string text, string key)
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

        public static string Decrypt(this string text, string key)
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
