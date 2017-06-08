using System.Collections.Generic;

namespace BookCollector.Models
{
    public interface IApplicationModel
    {
        Collection CurrentCollection { get; set; }

        void AddToCurrentCollection(List<Book> books);
        void LoadCurrentCollection(string path);
        void SaveCurrentCollection();

        List<Description> GetAllCollectionDescriptions();
    }
}