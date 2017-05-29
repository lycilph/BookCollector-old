using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BookCollector.Data;
using BookCollector.Extensions;
using BookCollector.Framework.Logging;
using ReactiveUI;

namespace BookCollector.Models
{
    public class BookCollectorModel : ReactiveObject, IBookCollectorModel
    {
        private const string collection_extension = ".bcdb"; // bcdb = Book Collector DataBase

        private ILog log = LogManager.GetCurrentClassLogger();

        private Collection _CurrentCollection;
        public Collection CurrentCollection
        {
            get { return _CurrentCollection; }
            set { this.RaiseAndSetIfChanged(ref _CurrentCollection, value); }
        }

        public List<CollectionDescription> GetAllCollectionDescriptions()
        {
            var app_path = Assembly.GetExecutingAssembly().Location;
            var dir = Path.GetDirectoryName(app_path);

            var collection_descriptions = new List<CollectionDescription>();
            foreach (var filename in Directory.EnumerateFiles(dir, "*" + collection_extension))
            {
                log.Info($"Loading file {filename}");
                var collection = JsonExtensions.ReadFromFile<Collection>(filename);
                collection.Description.Filename = filename;
                collection_descriptions.Add(collection.Description);
            }
            return collection_descriptions;
        }

        //private string GetDatabasePath()
        //{
        //    var app_path = Assembly.GetExecutingAssembly().Location;
        //    return Path.Combine(Path.GetDirectoryName(app_path), database_name);
        //}

        //public void LoadData()
        //{
        //    log.Info("Loading model data");

        //    //var database_path = GetDatabasePath();

        //    // Load users and collections
        //    //if (File.Exists(database_path))
        //    //{
        //    //    var data = JsonExtensions.ReadFromFile<BookCollectorModelSerializer>(database_path);
        //    //    Mapper.Map(data, this);
        //    //}
        //}

        //public void SaveData()
        //{
        //    log.Info("Saving model data");

        //    //var database_path = GetDatabasePath();

        //    // On application exit, save current state
        //    //var data = Mapper.Map<BookCollectorModelSerializer>(this);
        //    //JsonExtensions.WriteToFile(database_path, data);
        //}
    }
}
