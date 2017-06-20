using System.Collections.Generic;
using BookCollector.Data;

namespace BookCollector.Services.Search
{
    public class Document
    {
        public Book Book { get; set; }
        public IDictionary<string, double> NormalizedTermFrequency { get; set; }
        public ICollection<string> Terms { get { return NormalizedTermFrequency.Keys; } }
        public ICollection<double> TF { get { return NormalizedTermFrequency.Values; } }

        public Document(IDictionary<string, double> term_frequencies)
        {
            NormalizedTermFrequency = term_frequencies;
        }

        public Document Add(Book book)
        {
            Book = book;
            return this;
        }
    }
}
