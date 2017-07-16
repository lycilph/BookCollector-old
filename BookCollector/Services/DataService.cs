using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BookCollector.Data;
using BookCollector.Domain;
using BookCollector.Framework.Extensions;

namespace BookCollector.Services
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

        public Settings LoadSettings()
        {
            if (!SettingsExists())
                return null;

            var settings = JsonExtensions.ReadFromFile<Settings>(GetSettingsPath());
            settings.IsDirty = false;
            return settings;
        }

        public void SaveSettings(Settings settings)
        {
            if (settings.IsDirty || !SettingsExists())
            {
                JsonExtensions.WriteToFile<Settings>(GetSettingsPath(), settings);
                settings.IsDirty = false;
            }
        }

        public bool CollectionExists(string path)
        {
            return File.Exists(path);
        }

        public Collection LoadCollection(string path)
        {
            var collection = JsonExtensions.ReadFromFile<Collection>(path);
            // Update default shelf
            collection.SetDefaultShelf(collection.Shelves.Single(s => s.Name == Constants.DefaultShelfName));
            // Update books with shelves (since it is NOT serialized due to loops)
            collection.Shelves.Apply(s => s.Books.Apply(b => b.Add(s)));
            collection.Description.Filename = path;
            collection.Description.BooksCount = collection.Books.Count;
            collection.Description.ShelfCount = collection.Shelves.Count;
            collection.IsDirty = false;
            return collection;
        }

        public void SaveCollection(Collection collection)
        {
            if (collection.IsDirty || !CollectionExists(collection.Description.Filename))
            {
                // Set filename if empty
                if (string.IsNullOrWhiteSpace(collection.Description.Filename))
                    collection.Description.Filename = Path.Combine(GetDataDirectory(), collection.Description.Name.MakeFilenameSafe() + collection_extension);

                // Set last modified date
                collection.Description.LastModifiedDate = DateTime.Now;

                JsonExtensions.WriteToFile(collection.Description.Filename, collection);
                collection.IsDirty = false;
            }
        }

        public void DeleteCollection(string path)
        {
            if (CollectionExists(path))
                File.Delete(path);
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
