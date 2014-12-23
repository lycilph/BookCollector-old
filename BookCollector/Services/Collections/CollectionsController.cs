using System;
using System.ComponentModel.Composition;
using System.IO;
using BookCollector.Services.Settings;
using BookCollector.Utilities;
using Newtonsoft.Json;
using NLog;
using ReactiveUI;
using LogManager = NLog.LogManager;

namespace BookCollector.Services.Collections
{
    [Export(typeof(CollectionsController))]
    [JsonObject(MemberSerialization.OptOut)]
    public class CollectionsController : ReactiveObject, IPersistable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private const string filename = "Collections.txt";

        private readonly ApplicationSettings settings;

        private CollectionDescription _Current;
        public CollectionDescription Current
        {
            get { return _Current; }
            set { this.RaiseAndSetIfChanged(ref _Current, value); }
        }

        private ReactiveList<CollectionDescription> _Collections = new ReactiveList<CollectionDescription>();
        public ReactiveList<CollectionDescription> Collections
        {
            get { return _Collections; }
            private set { this.RaiseAndSetIfChanged(ref _Collections, value); }
        }

        // This is called when deserializing (ie. in the load method)
        public CollectionsController() { }

        [ImportingConstructor]
        public CollectionsController(ApplicationSettings settings)
        {
            this.settings = settings;
        }

        public void Load()
        {
            logger.Trace("Loading");

            var path = Path.Combine(settings.DataDir, filename);
            if (path == null)
                throw new ArgumentException();

            if (!File.Exists(path))
                return;

            var json = File.ReadAllText(path);
            var temp = JsonConvert.DeserializeObject<CollectionsController>(json);

            Current = temp.Current;
            Collections = temp.Collections.ToReactiveList();
        }

        public void Save()
        {
            logger.Trace("Saving");

            var path = Path.Combine(settings.DataDir, filename);
            if (path == null)
                throw new ArgumentException();

            var json = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings {PreserveReferencesHandling = PreserveReferencesHandling.Objects});
            File.WriteAllText(path, json);
        }

        public CollectionDescription Create()
        {
            var collection = new CollectionDescription();
            Collections.Add(collection);
            return collection;
        }

        public void Remove(CollectionDescription collection)
        {
            Collections.Remove(collection);

            if (Current == collection)
                Current = null;
        }
    }
}
