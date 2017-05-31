using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AutoMapper;
using BookCollector.Data;
using BookCollector.Framework.Extensions;

namespace BookCollector.Controllers
{
    public class DataController : IDataController
    {
        private const string settings_filename = "ApplicationSettings.json";
        private const string collection_extension = ".bcdb"; // bcdb = Book Collector DataBase
        private const string collection_search_pattern = "*" + collection_extension;

        public bool HasSettings()
        {
            return File.Exists(GetSettingsPath());
        }

        public Settings LoadSettings()
        {
            if (!HasSettings())
                return null;

            return JsonExtensions.ReadFromFile<Settings>(GetSettingsPath());
        }

        public void SaveSettings(Settings settings)
        {
            JsonExtensions.WriteToFile<Settings>(GetSettingsPath(), settings);
        }

        public bool CollectionExists(string path)
        {
            return File.Exists(path);
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
                collection.Description.Filename = collection.Description.Name.MakeFilenameSafe() + collection_extension;

            // Set last modified date
            collection.Description.LastModfied = DateTime.Now;

            JsonExtensions.WriteToFile(collection.Description.Filename, collection);
        }

        public void DeleteCollection(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        public List<CollectionDescription> GetAllCollectionDescriptions()
        {
            var collection_descriptions = new List<CollectionDescription>();
            foreach (var filename in Directory.EnumerateFiles(GetDataDirectory(), collection_search_pattern))
            {
                var collection = JsonExtensions.ReadFromFile<Collection>(filename);
                collection.Description.Filename = filename;
                collection_descriptions.Add(collection.Description);
            }
            return collection_descriptions;
        }

        public void UpdateCollectionDescription(CollectionDescription collection_description)
        {
            if (!File.Exists(collection_description.Filename))
                return;

            var collection = LoadCollection(collection_description.Filename);
            Mapper.Map(collection_description, collection.Description);
            SaveCollection(collection);
        }

        private string GetDataDirectory()
        {
            var app_path = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(app_path);
        }

        private string GetSettingsPath()
        {
            var app_path = Assembly.GetExecutingAssembly().Location;
            var app_dir = Path.GetDirectoryName(app_path);
            return Path.Combine(app_dir, settings_filename);
        }
    }
}
