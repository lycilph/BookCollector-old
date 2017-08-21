using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BookCollector.Data;
using Core.Extensions;

namespace BookCollector.Services
{
    public class CollectionsRepository : ICollectionsRepository
    {
        private const string collection_extension = ".bcdb"; // bcdb = Book Collector DataBase

        public Collection Load(string filename)
        {
            var collection = JsonExtensions.ReadFromFile<Collection>(filename);

            // Update books with shelves (since it is NOT serialized due to loops)
            collection.Shelves.Apply(s => s.Books.Apply(b => b.Shelves.Add(s)));

            collection.Description.Filename = filename;
            collection.IsDirty = false;
            return collection;
        }

        public void Save(Collection collection)
        {
            if (collection.IsDirty || !Exists(collection.Description.Filename))
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

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public List<string> GetCollectionsList()
        {
            return Directory.EnumerateFiles(GetDataDirectory(), "*" + collection_extension).ToList();
        }

        private string GetDataDirectory()
        {
            var app_path = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(app_path);
        }
    }
}
