using System.IO;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;

namespace Core.Extensions
{
    public static class JsonExtensions
    {
        public static void WriteToFile<T>(string filename, T obj)
        {
            WriteToFile(filename, obj, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }

        public static void WriteToFile<T>(string filename, T obj, JsonSerializerSettings settings)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
            File.WriteAllText(filename, json);
        }

        public static T ReadFromFile<T>(string filename)
        {
            var json = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static void ZipAndWriteToFile<T>(string filename, T obj)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            var bytes = Zip(json);
            File.WriteAllBytes(filename, bytes);
        }

        public static T ReadFromFileAndUnzip<T>(string filename)
        {
            var bytes = File.ReadAllBytes(filename);
            var json = Unzip(bytes);
            return JsonConvert.DeserializeObject<T>(json);
        }

        private static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                using (var gs = new GZipStream(output, CompressionMode.Compress))
                {
                    input.CopyTo(gs);
                }

                return output.ToArray();
            }
        }

        private static string Unzip(byte[] bytes)
        {
            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                using (var gs = new GZipStream(input, CompressionMode.Decompress))
                {
                    gs.CopyTo(output);
                }

                return Encoding.UTF8.GetString(output.ToArray());
            }
        }
    }
}
