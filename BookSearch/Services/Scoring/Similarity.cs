using System;
using System.Collections.Generic;
using System.Linq;
using BookSearch.Data;

namespace BookSearch.Services.Scoring
{
    public class Similarity
    {
        private readonly Document query_document;
        private readonly ICollection<string> query_terms;

        public Similarity(string query)
        {
            query_document = new Document(query);
            query_terms = query_document.NormalizedTermFrequency.Keys;
        }

        public List<Document> Score(List<Book> books)
        {
            var documents = books.Select(b => new Document(b)).ToList();
            var idf_vector = query_terms.Select(t => Math.Log(documents.Count / (double)(1 + documents.Count(d => d.Contains(t))))).ToList();
            var query_vector = GetDocumentVector(query_document, idf_vector);

            foreach (var document in documents)
            {
                var document_vector = GetDocumentVector(document, idf_vector);
                document.Score = query_vector.Zip(document_vector, (q, d) => q*d).Sum();
            }
            return documents;
        }

        private List<double> GetDocumentVector(Document document, IEnumerable<double> idf_vector)
        {
            var term_vector = document.GetTermVector(query_terms);
            return term_vector.Zip(idf_vector, (tf, idf) => tf*idf).ToList().Normalize();
        }
    }
}
