using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Test
{
    public class StopWordHandler
    {
        private List<string> words;

        public StopWordHandler()
        {
            var settings_filename = Assembly.GetExecutingAssembly().GetManifestResourceNames().First(n => n.Contains("stopwords"));
            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(settings_filename))
            using (var sr = new StreamReader(s))
            {
                var text = sr.ReadToEnd();
                words = text.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }

        public bool IsStopWord(string s)
        {
            return words.Contains(s);
        }
    }
}
