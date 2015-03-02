using System;
using System.Collections.Generic;
using System.Linq;
using BookSearch.Data;

namespace BookSearch.Services.Scoring
{
    public class Document
    {
        private static readonly StopWordHandler stopword_handler = new StopWordHandler();
        private static readonly IStemmer stemmer = new PorterStemmer();

        public Book Book { get; set; }
        public IDictionary<string, double> NormalizedTermFrequency { get; set; }
        public double Score { get; set; }

        public Document(Book book)
        {
            Book = book;
            Initialize(book.Title + " " + book.Author);
        }
        public Document(string text)
        {
            Initialize(text);
        }

        private void Initialize(string text)
        {
            var terms = text.Split(" @$/#.-:&*+=[]?!(){},''\">_<;%\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            .Select(t => t.ToLowerInvariant())
                            .ToList();
            var stemmed_terms = terms.Where(t => !stopword_handler.IsStopWord(t))
                                     .Select(stemmer.StemTerm)
                                     .ToList();
            var term_frequency = stemmed_terms.GroupBy(t => t)
                                              .ToDictionary(g => g.Key, g => (double)g.Count());
            NormalizedTermFrequency = term_frequency.Normalize();
        }

        public bool Contains(string term)
        {
            return NormalizedTermFrequency.ContainsKey(term);
        }

        public List<double> GetTermVector(ICollection<string> query_terms)
        {
            return query_terms.Select(t => NormalizedTermFrequency.ContainsKey(t) ? NormalizedTermFrequency[t] : 0).ToList();
        }
    }
}
