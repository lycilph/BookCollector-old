using System.ComponentModel.Composition;
using System.Linq;
using NLog;
using ReactiveUI;

namespace BookCollector.Model
{
    [Export(typeof(ProfileController))]
    public class ProfileController : ReactiveObject
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly BookRepository repository;
        private readonly ImageDownloader downloader;
        // SearchProvider

        private ReactiveList<ProfileDescription> _Profiles;
        public ReactiveList<ProfileDescription> Profiles
        {
            get { return _Profiles; }
            set { this.RaiseAndSetIfChanged(ref _Profiles, value); }
        }

        private ProfileDescription _CurrentProfile;
        public ProfileDescription CurrentProfile
        {
            get { return _CurrentProfile; }
            set { this.RaiseAndSetIfChanged(ref _CurrentProfile, value); }
        }

        private CollectionDescription _CurrentCollection;
        public CollectionDescription CurrentCollection
        {
            get { return _CurrentCollection; }
            set { this.RaiseAndSetIfChanged(ref _CurrentCollection, value); }
        }

        [ImportingConstructor]
        public ProfileController(BookRepository repository, ImageDownloader downloader)
        {
            this.repository = repository;
            this.downloader = downloader;

            // Debug
            Profiles = new ReactiveList<ProfileDescription>
            {
                new ProfileDescription {DisplayName = "Profile 1"},
                new ProfileDescription {DisplayName = "Profile 2"},
                new ProfileDescription {DisplayName = "Profile 3"},
            };

            foreach (var profile in Profiles)
            {
                profile.Collections = new ReactiveList<CollectionDescription>
                {
                    new CollectionDescription {DisplayName = profile.DisplayName + " - Collection 1"},
                    new CollectionDescription {DisplayName = profile.DisplayName + " - Collection 2"},
                    new CollectionDescription {DisplayName = profile.DisplayName + " - Collection 3"},
                };
            }

            CurrentProfile = Profiles[1];
            CurrentCollection = CurrentProfile.Collections.First();
        }

        public void Load()
        {
            
        }

        public void Save()
        {
            
        }
    }
}
