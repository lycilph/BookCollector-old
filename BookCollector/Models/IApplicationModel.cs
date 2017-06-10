using System.Collections.Generic;
using BookCollector.Data;

namespace BookCollector.Models
{
    public interface IApplicationModel
    {
        Collection CurrentCollection { get; set; }

        void AddToCurrentCollection(List<Book> books);
        void LoadCurrentCollection(string path);
        void SaveCurrentCollection();

        void AddCollection(Description description);
        void UpdateCollection(Description description);
        void DeleteCollection(Description description);

        List<Description> GetAllCollectionDescriptions();
    }
}