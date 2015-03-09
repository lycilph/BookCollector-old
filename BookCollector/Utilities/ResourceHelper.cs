using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace BookCollector.Utilities
{
    public static class ResourceHelper
    {
        public static T GetAndDeserialize<T>(string name) where T : class
        {
            var settings_filename = Assembly.GetExecutingAssembly().GetManifestResourceNames().First(n => n.Contains(name));
            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(settings_filename))
            using (var sr = new StreamReader(s))
            {
                var json = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
    }
}
