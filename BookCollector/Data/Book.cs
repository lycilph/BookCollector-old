using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace BookCollector.Data
{
    [DebuggerDisplay("Title = {Title}")]
    [JsonObject(MemberSerialization.OptOut)]
    public class Book
    {
        public string Title { get; set; }
        public List<string> Authors { get; set; } = new List<string>();
        public string ISBN10 { get; set; }
        public string ISBN13 { get; set; }
        [JsonIgnore]
        public List<Shelf> Shelves { get; set; } = new List<Shelf>();
        public string Source { get; set; }
        public List<string> History { get; set; } = new List<string>();
    }
}
