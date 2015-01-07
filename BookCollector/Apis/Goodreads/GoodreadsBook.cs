using System.Collections.Generic;

namespace BookCollector.Apis.GoodReads
{
    public class GoodReadsBook
    {
        public string Title { get; set; }
        public string Isbn { get; set; }
        public string Isbn13 { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string SmallImageUrl { get; set; }
        public string Link { get; set; }
        public List<GoodReadsAuthor> Authors { get; set; }
    }
}
