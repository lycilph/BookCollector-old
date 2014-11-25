using System.Collections.Generic;

namespace BookCollector.Model
{
    public class Book
    {
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public List<string> Narrators { get; set; }
        public string Description { get; set; }
        public string Asin { get; set; }
        public string ISBN10 { get; set; }
        public string ISBN13 { get; set; }
        public string Image { get; set; }
        public string SmallImage { get; set; }
        public string ImportSource { get; set; }
    }
}
