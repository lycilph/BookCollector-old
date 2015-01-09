using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using BookCollector.Utilities;
using Newtonsoft.Json;
using NLog;
using ReactiveUI;

namespace BookCollector.Model
{
    [Export(typeof(ProfileController))]
    [JsonObject(MemberSerialization.OptOut)]
    public class ProfileController : ReactiveObject
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private const string filename = "Profiles.txt";

        private readonly BookRepository repository;
        private readonly ImageDownloader downloader;
        // SearchProvider

        private ReactiveList<ProfileDescription> _Profiles = new ReactiveList<ProfileDescription>();
        public ReactiveList<ProfileDescription> Profiles
        {
            get { return _Profiles; }
            set { this.RaiseAndSetIfChanged(ref _Profiles, value); }
        }

        private ProfileDescription _CurrentProfile;
        [JsonProperty]
        public ProfileDescription CurrentProfile
        {
            get { return _CurrentProfile; }
            private set { this.RaiseAndSetIfChanged(ref _CurrentProfile, value); }
        }

        private CollectionDescription _CurrentCollection;
        [JsonProperty]
        public CollectionDescription CurrentCollection
        {
            get { return _CurrentCollection; }
            private set { this.RaiseAndSetIfChanged(ref _CurrentCollection, value); }
        }

        [ImportingConstructor]
        public ProfileController(BookRepository repository, ImageDownloader downloader)
        {
            this.repository = repository;
            this.downloader = downloader;
        }

        public void SetCurrent(ProfileDescription profile, CollectionDescription collection)
        {
            if (CurrentCollection != null)
            {
                repository.Save(CurrentCollection);
                downloader.Stop();
                downloader.Save(CurrentCollection);
            }

            CurrentProfile = profile;
            CurrentCollection = collection;

            if (CurrentCollection != null)
            {
                repository.Load(CurrentCollection);
                downloader.Load(CurrentCollection, repository);
                downloader.Start();
            }

            // Load search provider
        }

        public void Import(List<ImportedBook> imported_books)
        {
            repository.Add(imported_books.Select(i => i.Book));
            downloader.Add(imported_books);
            CurrentCollection.LastModified = DateTime.Now;
        }

        public void Clear()
        {
            repository.Clear();
            downloader.Clear();
            CurrentCollection.LastModified = DateTime.Now;
        }

        public ProfileDescription CreateProfile()
        {
            var profile = new ProfileDescription { DisplayName = "No name" };
            Profiles.Add(profile);
            return profile;
        }

        public void RemoveProfile(ProfileDescription profile)
        {
            Profiles.Remove(profile);
        }

        public CollectionDescription CreateCollection(ProfileDescription profile)
        {
            var collection = new CollectionDescription { DisplayName = profile.DisplayName + " - Collection " + (profile.Collections.Count + 1) };
            profile.Collections.Add(collection);
            return collection;
        }

        public void RemoveCollection(ProfileDescription profile, CollectionDescription collection)
        {
            profile.Collections.Remove(collection);
        }

        public void Load(string dir)
        {
            var path = Path.Combine(dir, filename);
            if (!File.Exists(path))
                return;

            logger.Trace("Loading (path = {0})", path);
            var controller = JsonExtensions.DeserializeFromFile<ProfileController>(path);

            Profiles = controller.Profiles;
            SetCurrent(controller.CurrentProfile, controller.CurrentCollection);
        }

        public void Save(string dir)
        {
            var path = Path.Combine(dir, filename);
            logger.Trace("Saving (path = {0})", path);
            JsonExtensions.SerializeToFile(path, this, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

            SetCurrent(null, null); // This forces the repository to save the current collection
        }
    }
}
