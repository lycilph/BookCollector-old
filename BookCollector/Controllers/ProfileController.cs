using System.ComponentModel.Composition;
using System.IO;
using BookCollector.Model;
using BookCollector.Services;
using BookCollector.Utilities;
using Newtonsoft.Json;
using NLog;
using ReactiveUI;

namespace BookCollector.Controllers
{
    [Export(typeof(ProfileController))]
    [JsonObject(MemberSerialization.OptOut)]
    public class ProfileController : ReactiveObject
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private const string filename = "Profiles.txt";

        private readonly ApplicationSettings application_settings;

        private ReactiveList<ProfileDescription> _Profiles = new ReactiveList<ProfileDescription>();
        public ReactiveList<ProfileDescription> Profiles
        {
            get { return _Profiles; }
            set { this.RaiseAndSetIfChanged(ref _Profiles, value); }
        }

        private CollectionDescription _CurrentCollection;
        [JsonProperty]
        public CollectionDescription CurrentCollection
        {
            get { return _CurrentCollection; }
            private set { this.RaiseAndSetIfChanged(ref _CurrentCollection, value); }
        }

        [JsonIgnore]
        public ProfileDescription CurrentProfile {
            get
            {
                return (CurrentCollection == null ? null : CurrentCollection.Profile);
            } 
        }

        // This is used by json.net to deserialize an object
        public ProfileController() { }

        [ImportingConstructor]
        public ProfileController(ApplicationSettings application_settings)
        {
            this.application_settings = application_settings;
        }

        public void SetCurrent(CollectionDescription collection)
        {
            CurrentCollection = collection;
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
            profile.Add(collection);
            return collection;
        }

        public void RemoveCollection(ProfileDescription profile, CollectionDescription collection)
        {
            profile.Remove(collection);
        }

        public void Load()
        {
            var file_path = Path.Combine(application_settings.DataDir, filename);
            if (!File.Exists(file_path))
                return;

            logger.Trace("Loading (path = {0})", file_path);
            var temp = JsonExtensions.DeserializeFromFile<ProfileController>(file_path);

            Profiles = temp.Profiles;
            CurrentCollection = temp.CurrentCollection;
        }

        public void Save()
        {
            var file_path = Path.Combine(application_settings.DataDir, filename);
            logger.Trace("Saving (path = {0})", file_path);
            JsonExtensions.SerializeToFile(file_path, this, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
        }
    }
}
