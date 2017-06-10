using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BookCollector.Data;
using BookCollector.Framework.Extensions;
using BookCollector.Models;

namespace BookCollector.Domain
{
    public class DataService : IDataService
    {
        private const string settings_filename = "ApplicationSettings.json";
        private const string collection_extension = ".bcdb"; // bcdb = Book Collector DataBase
        private const string collection_search_pattern = "*" + collection_extension;

        public bool SettingsExists()
        {
            return File.Exists(GetSettingsPath());
        }

        public SettingsModel LoadSettings()
        {
            if (!SettingsExists())
                return null;

            return JsonExtensions.ReadFromFile<SettingsModel>(GetSettingsPath());
        }

        public void SaveSettings(SettingsModel settings)
        {
            JsonExtensions.WriteToFile<SettingsModel>(GetSettingsPath(), settings);
        }
        
        public bool CollectionExists(string path)
        {
            return File.Exists(path);
        }

        public void DeleteCollection(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        public Collection LoadCollection(string path)
        {
            var collection = JsonExtensions.ReadFromFile<Collection>(path);
            collection.Description.Filename = path;
            return collection;
        }

        public void SaveCollection(Collection collection)
        {
            // Set filename if zero
            if (string.IsNullOrWhiteSpace(collection.Description.Filename))
                collection.Description.Filename = Path.Combine(GetDataDirectory(), collection.Description.Name.MakeFilenameSafe() + collection_extension);

            // Set last modified date
            collection.Description.LastModfied = DateTime.Now;

            JsonExtensions.WriteToFile(collection.Description.Filename, collection);
        }

        public List<Description> GetAllCollectionDescriptions()
        {
            var descriptions = new List<Description>();
            foreach (var path in Directory.EnumerateFiles(GetDataDirectory(), collection_search_pattern))
            {
                var collection = LoadCollection(path);
                descriptions.Add(collection.Description);
            }
            return descriptions;
        }

        private string GetDataDirectory()
        {
            var app_path = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(app_path);
        }

        private string GetSettingsPath()
        {
            return Path.Combine(GetDataDirectory(), settings_filename);
        }
    }
}
