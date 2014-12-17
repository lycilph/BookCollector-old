using System.Collections.Generic;

namespace BookCollector.Apis.GoogleBooks
{
    public class GoogleBooksVolumeInfo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Authors { get; set; }
        public List<GoogleBooksIndustryIdentifiers> IndustryIdentifiers { get; set; }
        public GoogleBooksImageLinks ImageLinks { get; set; }
    }
}
