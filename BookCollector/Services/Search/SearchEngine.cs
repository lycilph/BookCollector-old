﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BookCollector.Data;
using BookCollector.Framework.Logging;

namespace BookCollector.Services.Search
{
    public class SearchEngine : ISearchEngine
    {
        private ILog log = LogManager.GetCurrentClassLogger();

        private readonly StopWordHandler stopword_handler = new StopWordHandler();
        private readonly PorterStemmer stemmer = new PorterStemmer();

        private List<Document> documents = new List<Document>();
        private IDictionary<string, double> inverse_document_frequency;

        public SearchEngine() { }
        public SearchEngine(List<Book> books)
        {
            Index(books);
        }

        public void Index(List<Book> books)
        {
            var sw = Stopwatch.StartNew();

            documents = books.Select(b => GetDocument(b))
                             .ToList();
            
            CalculateDocumentFrequencies();

            sw.Stop();
            log.Info($"Indexing collection took {sw.ElapsedMilliseconds} ms");
        }

        public List<SearchResult> Search(string query)
        {
            // Return null result if query is empty
            if (string.IsNullOrWhiteSpace(query))
                return null;

            var query_document = GetDocument(query);
            var query_terms = query_document.Terms.Where(t => inverse_document_frequency.ContainsKey(t));

            // Return empty list if no documents contains the query terms
            if (!query_terms.Any())
                return new List<SearchResult>();

            var query_idf = query_terms.Select(t => inverse_document_frequency[t]);
            var query_vector = query_document.TF.Zip(query_idf, (tf, idf) => tf * idf).Normalize();

            var results = new List<SearchResult>();
            foreach (var document in documents)
            {
                var document_tf = query_terms.Select(t => document.NormalizedTermFrequency.ContainsKey(t) ? document.NormalizedTermFrequency[t] : 0);
                var document_vector = document_tf.Zip(query_idf, (tf, idf) => tf * idf).Normalize();

                var score = query_vector.Zip(document_vector, (q, d) => q * d).Sum();
                if (score > 0)
                    results.Add(new SearchResult(document.Book, score));
            }
            return results.OrderByDescending(r => r.Score).ThenBy(r => r.Book.Title).ToList();
        }

        // Calculate inverse document term frequencies
        private void CalculateDocumentFrequencies()
        {
            // Find all unique terms
            var unique_terms = documents.SelectMany(d => d.Terms)
                                        .Distinct()
                                        .ToList();
            inverse_document_frequency = unique_terms.ToDictionary(t => t, t => CalculateInverseDocumentFrequency(t));
        }

        private double CalculateInverseDocumentFrequency(string term)
        {
            var documents_containing_term = documents.Count(d => d.Terms.Contains(term));
            return Math.Log(documents.Count / (double)(1 + documents_containing_term));
        }

        private Document GetDocument(Book book)
        {
            return GetDocument(book.Title + " " + string.Join(" ", book.Authors)).Add(book);
        }

        private Document GetDocument(string text)
        {
            return new Document(CalculateTermFrequencies(text));
        }

        // Calculate normalized term frequencies
        private IDictionary<string, double> CalculateTermFrequencies(string text)
        {
            var terms = GetTerms(text);
            // Find term frequency
            var term_frequency = terms.GroupBy(t => t)
                                      .ToDictionary(g => g.Key, g => (double)g.Count());
            // Return normalized term frequency
            return term_frequency.Normalize();
        }

        private IEnumerable<string> GetTerms(string text)
        {
            // Lower case all characters
            text = text.ToLowerInvariant();
            // Strip all non-alphanumeric characters
            var arr = text.ToCharArray();
            arr = Array.FindAll(arr, c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c));
            text = new string(arr);
            // Find terms (ie. split text on whitespace to find words)
            var terms = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            // Filter out stop words
            var filtered_terms = terms.Where(t => !stopword_handler.IsStopWord(t));
            // Stemmed tokens
            return filtered_terms.Select(t => stemmer.Stem(t));
        }
    }
}