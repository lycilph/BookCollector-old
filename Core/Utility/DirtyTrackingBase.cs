using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using ReactiveUI;

namespace Core.Utility
{
    // http://www.ibiliskov.info/2011/07/isdirty-object-dirtiness/
    public class DirtyTrackingBase : ReactiveObject
    {
        private static readonly MD5 md5 = new MD5CryptoServiceProvider();
        private string clean_hash;

        [JsonIgnore]
        public bool IsDirty
        {
            get { return !string.IsNullOrWhiteSpace(clean_hash) && GetHash() != clean_hash; }
            set { if (!value) clean_hash = GetHash(); }
        }

        private string GetHash()
        {
            using (var ms = new MemoryStream())
            using (var wr = new BsonDataWriter(ms))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(wr, this);
                return GetMd5Sum(ms.ToArray());
            }
        }

        private static string GetMd5Sum(byte[] buffer)
        {
            var result = md5.ComputeHash(buffer);
            // X2 prints the string as two uppercase hexadecimal characters
            return String.Join("", result.Select(b => b.ToString("X2")));
        }
    }
}
