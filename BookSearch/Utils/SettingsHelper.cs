using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace BookSearch.Utils
{
    public static class SettingsHelper
    {
        public static T Get<T>(string api_name) where T : class
        {
            var settings_filename = Assembly.GetExecutingAssembly().GetManifestResourceNames().First(n => n.Contains(api_name));
            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(settings_filename))
            using (var sr = new StreamReader(s))
            {
                var json = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
    }
}
