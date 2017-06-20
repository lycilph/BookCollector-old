using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BookCollector.Services.Search
{
    public class StopWordHandler
    {
        private readonly HashSet<string> word_set;

        public StopWordHandler()
        {
            var settings_filename = Assembly.GetExecutingAssembly().GetManifestResourceNames().First(n => n.Contains("stopwords"));
            using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(settings_filename))
            using (var sr = new StreamReader(s))
            {
                var text = sr.ReadToEnd();
                var words = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                word_set = new HashSet<string>(words);
            }
        }

        public bool IsStopWord(string s)
        {
            return word_set.Contains(s);
        }
    }
}
