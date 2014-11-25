using System.Collections.Generic;

namespace BookCollector.Services.Goodreads
{
    public class GoodreadsBook
    {
        public string Title { get; set; }
        public string Isbn { get; set; }
        public string Isbn13 { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string SmallImageUrl { get; set; }
        public string Link { get; set; }
        public List<GoodreadsAuthor> Authors { get; set; }
    }
}
