using System.Collections.Generic;
using BookCollector.Data;

namespace BookCollector.Models
{
    public interface ICollectionModel
    {
        Collection CurrentCollection { get; }

        Collection CreateDefaultCollection();
        List<SimilarityInformation> CalculateBookSimilarities(List<Book> c1, List<Book> c2);
        void Import(List<Book> books);
    }
}