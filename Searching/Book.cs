using System.Collections.Generic;

namespace Searching
{
    public class Book
    {
        public string Title { get; set; }
        public List<string> Authors { get; private set; } = new List<string>();
        public string Description { get; set; }
        public string ISBN10 { get; set; }
        public string ISBN13 { get; set; }
        public List<Shelf> Shelves { get; private set; } = new List<Shelf>();
        public string Source { get; set; }
        public List<string> History { get; private set; } = new List<string>();
    }
}
