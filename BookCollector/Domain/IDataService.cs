using System.Collections.Generic;
using BookCollector.Models;

namespace BookCollector.Domain
{
    public interface IDataService
    {
        bool SettingsExists();
        SettingsModel LoadSettings();
        void SaveSettings(SettingsModel settings);

        bool CollectionExists(string path);
        Collection LoadCollection(string path);
        void SaveCollection(Collection collection);
        void DeleteCollection(string path);

        List<Description> GetAllCollectionDescriptions();
    }
}