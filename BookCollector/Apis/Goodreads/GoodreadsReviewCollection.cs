using System.Collections.Generic;

namespace BookCollector.Apis.GoodReads
{
    public class GoodReadsReviewCollection
    {
        public int Start { get; set; }
        public int End { get; set; }
        public int Total { get; set; }
        public List<GoodReadsReview> Reviews { get; set; }
    }
}
