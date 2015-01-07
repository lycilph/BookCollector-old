using System.Collections.Generic;

namespace BookCollector.Apis.GoodReads
{
    public class GoodReadsImportResponse
    {
        public IEnumerable<GoodReadsBook> Books { get; set; }
        public int Total { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }
}
