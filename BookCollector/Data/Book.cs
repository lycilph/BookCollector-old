using System.Collections.Generic;

namespace BookCollector.Data
{
    public class Book
    {
        public string Title { get; set; }
        public List<string> Authors { get; private set; } = new List<string>();
        public List<string> Narrators { get; private set; } = new List<string>();
        public string Description { get; set; }
        public string Asin { get; set; }
        public string ISBN10 { get; set; }
        public string ISBN13 { get; set; }
        public string Source { get; set; }
        public List<string> History { get; private set; } = new List<string>();
    }
}
