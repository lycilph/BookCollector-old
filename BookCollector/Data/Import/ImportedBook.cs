using System.Collections.Generic;
using System.Diagnostics;

namespace BookCollector.Data.Import
{
    [DebuggerDisplay("Title = {Title}")]
    public class ImportedBook
    {
        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public string ISBN10 { get; set; }
        public string ISBN13 { get; set; }
        public List<string> Shelves { get; set; }
        public string Source { get; set; }

        public SimilarityInformation Similarity { get; set; }
    }
}
