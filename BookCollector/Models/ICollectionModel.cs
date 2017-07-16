using System.Collections.Generic;
using BookCollector.Data;

namespace BookCollector.Models
{
    public interface ICollectionModel
    {
        Collection CurrentCollection { get; }

        void LoadCurrentCollection(string path);
        void SaveCurrentCollection();

        void SaveCollection(Collection collection);
        void UpdateCollection(Description description);
        void DeleteCollection(Description description);

        Collection CreateDefaultCollection();
        List<Description> GetAllCollectionDescriptions();
        List<SimilarityInformation> CalculateBookSimilarities(List<Book> c1, List<Book> c2);
        void Import(List<Book> books);
    }
}