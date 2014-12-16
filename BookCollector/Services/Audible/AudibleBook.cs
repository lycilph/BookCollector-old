using System.Collections.Generic;

namespace BookCollector.Services.Audible
{
    public class AudibleBook
    {
        public string Title { get; set; }
        public string Asin { get; set; }
        public List<string> Authors { get; set; }
        public List<string> Narrators { get; set; }
        public string Description { get; set; }
        public List<string> PartsAsin { get; set; }

        public AudibleBook()
        {
            PartsAsin = new List<string>();
        }
    }
}
