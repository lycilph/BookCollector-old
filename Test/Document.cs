using System.Collections.Generic;
using System.Diagnostics;

namespace Test
{
    [DebuggerDisplay("Title = {Book.Title}, Similarity = {CosineSimilarity}")]
    public class Document
    {
        public Book Book { get; set; }
        public List<string> Terms { get; set; }
        public List<string> StemmedTerms { get; set; }
        public IDictionary<string,int> TermFrequency { get; set; }
        public IDictionary<string,double> NormalizedTermFrequency { get; set; }
        public List<double> tfidf { get; set; }
        public List<double> Normalizedtfidf { get; set; }
        public double CosineSimilarity { get; set; }
    }
}
