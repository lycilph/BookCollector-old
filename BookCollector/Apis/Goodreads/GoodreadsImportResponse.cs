using System.Collections.Generic;

namespace BookCollector.Apis.Goodreads
{
    public class GoodreadsImportResponse
    {
        public IEnumerable<GoodreadsBook> Books { get; set; }
        public int Total { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }
}
