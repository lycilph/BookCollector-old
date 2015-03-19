using System.Collections.Generic;

namespace BookCollector.Api.ImportProvider
{
    public class ImportedBook
    {
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public string ISBN10 { get; set; }
        public string ISBN13 { get; set; }
        public string Shelf { get; set; }
        public string Source { get; set; }
    }
}
