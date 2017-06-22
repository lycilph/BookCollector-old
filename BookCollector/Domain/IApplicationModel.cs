using System.Collections.Generic;
using BookCollector.Data;

namespace BookCollector.Domain
{
    public interface IApplicationModel
    {
        Settings Settings { get; }
        Collection CurrentCollection { get; }

        void Load();
        void Save();

        void AddToCurrentCollection(List<Book> books);
        void AddToCurrentCollection(Shelf shelf);
        void RemoveFromCurrentCollection(Shelf shelf);

        void LoadCurrentCollection(string path);
        void AddCollection(Description description);
        void UpdateCollection(Description description);
        void DeleteCollection(Description description);

        Description CreateCollectionDescription();
        List<Description> GetAllCollectionDescriptions();
    }
}