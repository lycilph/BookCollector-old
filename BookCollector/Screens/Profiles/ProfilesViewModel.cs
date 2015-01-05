using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using BookCollector.Model;
using BookCollector.Shell;
using Caliburn.Micro;
using ReactiveUI;

namespace BookCollector.Screens.Profiles
{
    [Export("Profiles", typeof(IShellScreen))]
    public class ProfilesViewModel : ShellScreenBase
    {
        private readonly IEventAggregator event_aggregator;
        private readonly ProfileController profile_controller;

        public override bool IsCommandsEnabled { get { return false; } }

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

        private readonly ObservableAsPropertyHelper<bool> _CanRemoveProfile;
        public bool CanRemoveProfile { get { return _CanRemoveProfile.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanAddCollection;
        public bool CanAddCollection { get { return _CanAddCollection.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanRemoveCollection;
        public bool CanRemoveCollection { get { return _CanRemoveCollection.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanOk;
        public bool CanOk { get { return _CanOk.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanCancel;
        public bool CanCancel { get { return _CanCancel.Value; } }
            
        [ImportingConstructor]
        public ProfilesViewModel(ProfileController profile_controller, IEventAggregator event_aggregator)
        {
            this.profile_controller = profile_controller;
            this.event_aggregator = event_aggregator;

            _Profiles = profile_controller.Profiles.CreateDerivedCollection(p => new ProfileDescriptionViewModel(p));

            var current_profile_observable = this.WhenAny(x => x.CurrentProfile, x => x.Value != null);
            var current_observable = Observable.Merge(this.WhenAny(x => x.CurrentProfile, x => x.Value != null), 
                                                      this.WhenAny(x => x.CurrentProfile.CurrentCollection, x => x.Value != null));
            var controller_observable = profile_controller.WhenAny(x => x.CurrentProfile, x => x.Value != null);

            _CanRemoveProfile = current_profile_observable.ToProperty(this, x => x.CanRemoveProfile);
            _CanAddCollection = current_profile_observable.ToProperty(this, x => x.CanAddCollection);

            _CanRemoveCollection = current_observable.ToProperty(this, x => x.CanRemoveCollection);
            _CanOk = current_observable.ToProperty(this, x => x.CanOk);

            _CanCancel = controller_observable.ToProperty(this, x => x.CanCancel);
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
            var profile = profile_controller.CreateProfile();
            CurrentProfile = Profiles.Single(p => p.AssociatedObject == profile);
        }

        public void RemoveProfile()
        {
            var profile = CurrentProfile.AssociatedObject;
            profile_controller.RemoveProfile(profile);

            CurrentProfile = Profiles.FirstOrDefault();
        }

        public void AddCollection()
        {
            var profile = CurrentProfile.AssociatedObject;
            var collection = profile_controller.CreateCollection(profile);

            CurrentProfile.CurrentCollection = CurrentProfile.Collections.Single(c => c.AssociatedObject == collection);
        }

        public void RemoveCollection()
        {
            var profile = CurrentProfile.AssociatedObject;
            var collection = CurrentProfile.CurrentCollection.AssociatedObject;
            profile_controller.RemoveCollection(profile, collection);

            CurrentProfile.CurrentCollection = CurrentProfile.Collections.FirstOrDefault();
        }

        public void Ok()
        {
            var profile = CurrentProfile.AssociatedObject;
            var collection = CurrentProfile.CurrentCollection.AssociatedObject;
            profile_controller.SetCurrent(profile, collection);
            Cancel();
        }

        public void Cancel()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.Back());            
        }
    }
}
