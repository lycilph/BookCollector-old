using System.IO;
using Newtonsoft.Json;

namespace BookCollector.Utilities
{
    public static class JsonExtensions
    {
        public static void SerializeToFile<T>(string filename, T obj)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(filename, json);
        }

        public static T DeserializeFromFile<T>(string filename)
        {
            var json = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
