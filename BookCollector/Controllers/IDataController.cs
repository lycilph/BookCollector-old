using System.Collections.Generic;
using BookCollector.Data;

namespace BookCollector.Controllers
{
    public interface IDataController
    {
        bool HasSettings();
        Settings LoadSettings();
        void SaveSettings(Settings settings);

        bool CollectionExists(string path);
        Collection LoadCollection(string path);
        void SaveCollection(Collection collection);
        void DeleteCollection(string path);

        List<CollectionDescription> GetAllCollectionDescriptions();
        void UpdateCollectionDescription(CollectionDescription collection_description);
    }
}