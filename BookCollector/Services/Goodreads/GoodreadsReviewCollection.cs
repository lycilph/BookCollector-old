using System.Collections.Generic;

namespace BookCollector.Services.Goodreads
{
    public class GoodreadsReviewCollection
    {
        public int Start { get; set; }
        public int End { get; set; }
        public int Total { get; set; }
        public List<GoodreadsReview> Reviews { get; set; }
    }
}
