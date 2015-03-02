using System;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Text.RegularExpressions;

namespace BookSearch.Api.Amazon
{
    public class AmazonSigningMessageInspector : IClientMessageInspector
    {
        private readonly string access_key_id = "";
        private readonly string secret_key = "";

        public AmazonSigningMessageInspector(string access_key_id, string secret_key)
        {
            this.access_key_id = access_key_id;
            this.secret_key = secret_key;
        }

        public Object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var operation = Regex.Match(request.Headers.Action, "[^/]+$").ToString();
            var now = DateTime.UtcNow;
            var timestamp = now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var sign_me = operation + timestamp;
            var bytes_to_sign = Encoding.UTF8.GetBytes(sign_me);

            var secret_key_bytes = Encoding.UTF8.GetBytes(secret_key);
            HMAC hmac_sha256 = new HMACSHA256(secret_key_bytes);
            var hash_bytes = hmac_sha256.ComputeHash(bytes_to_sign);
            var signature = Convert.ToBase64String(hash_bytes);

            request.Headers.Add(new AmazonHeader("AWSAccessKeyId", access_key_id));
            request.Headers.Add(new AmazonHeader("Timestamp", timestamp));
            request.Headers.Add(new AmazonHeader("Signature", signature));
            return null;
        }

        void IClientMessageInspector.AfterReceiveReply(ref Message message, Object correlation_state) { }
    }
}
