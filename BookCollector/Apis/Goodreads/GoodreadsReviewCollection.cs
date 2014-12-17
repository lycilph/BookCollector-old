using System.Collections.Generic;

namespace BookCollector.Apis.Goodreads
{
    public class GoodreadsReviewCollection
    {
        public int Start { get; set; }
        public int End { get; set; }
        public int Total { get; set; }
        public List<GoodreadsReview> Reviews { get; set; }
    }
}
