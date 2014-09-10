using System.Collections.Generic;

namespace BookCollector.Goodreads
{
    public class GoodreadsBook
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string ISBN13 { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public List<GoodreadsBook> SimilarBooks { get; set; }
    }
}
