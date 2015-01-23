using System.Collections.Generic;

namespace BookCollector.Apis.Audible
{
    public class AudibleBook
    {
        public string Title { get; set; }
        public string Asin { get; set; }
        public string ParentAsin { get; set; }
        public List<string> Authors { get; set; }
        public List<string> Narrators { get; set; }
        public string Description { get; set; }
        public List<string> PartsAsin { get; set; }
        public string ImageUrl { get; set; }

        public AudibleBook()
        {
            PartsAsin = new List<string>();
        }
    }
}
