using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Model;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using IScreen = Caliburn.Micro.IScreen;

namespace BookCollector.Screens.Profiles
{
    [Export("Profiles", typeof(IScreen))]
    public class ProfilesViewModel : ReactiveScreen
    {
        private readonly ProfileController profile_controller;

        private IReactiveDerivedList<ProfileDescriptionViewModel> _Profiles;
        public IReactiveDerivedList<ProfileDescriptionViewModel> Profiles
        {
            get { return _Profiles; }
            set { this.RaiseAndSetIfChanged(ref _Profiles, value); }
        }

        private ProfileDescriptionViewModel _CurrentProfile;
        public ProfileDescriptionViewModel CurrentProfile
        {
            get { return _CurrentProfile; }
            set { this.RaiseAndSetIfChanged(ref _CurrentProfile, value); }
        }

        [ImportingConstructor]
        public ProfilesViewModel(ProfileController profile_controller)
        {
            this.profile_controller = profile_controller;

            _Profiles = profile_controller.Profiles.CreateDerivedCollection(p => new ProfileDescriptionViewModel(p));
        }

        protected override void OnActivate()
        {
            if (profile_controller.CurrentProfile == null) 
                return;

            CurrentProfile = Profiles.Single(p => p.AssociatedObject == profile_controller.CurrentProfile);
            CurrentProfile.CurrentCollection = CurrentProfile.Collections.Single(c => c.AssociatedObject == profile_controller.CurrentCollection);
        }

        public void AddProfile()
        {
            
        }
    }
}
