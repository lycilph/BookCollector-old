using System;
using Newtonsoft.Json;

namespace BookCollector.Data
{
    public class Description
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime LastModifiedDate { get; set; }
        [JsonIgnore]
        public string Filename { get; set; }
        [JsonIgnore]
        public int BooksCount { get; set; }
        [JsonIgnore]
        public int ShelfCount { get; set; }
    }
}
