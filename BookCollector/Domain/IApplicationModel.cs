using System.Collections.Generic;
using BookCollector.Data;

namespace BookCollector.Domain
{
    public interface IApplicationModel
    {
        Collection CurrentCollection { get; set; }

        void Load();
        void Save();

        void LoadCurrentCollection(string path);
        void AddCollection(Description description);
        void UpdateCollection(Description description);
        void DeleteCollection(Description description);

        Description CreateCollectionDescription();
        List<Description> GetAllCollectionDescriptions();
    }
}