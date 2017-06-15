using System.Collections.Generic;
using BookCollector.Data;

namespace BookCollector.Services
{
    public interface IDataService
    {
        bool SettingsExists();
        Settings LoadSettings();
        void SaveSettings(Settings settings);

        bool CollectionExists(string path);
        Collection LoadCollection(string path);
        void SaveCollection(Collection collection);
        void DeleteCollection(string path);

        List<Description> GetAllCollectionDescriptions();
    }
}