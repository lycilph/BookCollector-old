using System.ComponentModel.Composition;
using System.Linq;
using BookCollector.Apis;
using BookCollector.Model;
using BookCollector.Services;
using BookCollector.Shell;
using BookCollector.Utilities;
using Caliburn.Micro;
using Framework.Core.Dialogs;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;

namespace BookCollector.Screens.Settings
{
    [Export("Settings", typeof(IShellScreen))]
    public class SettingsViewModel : ShellScreenBase
    {
        private readonly IEventAggregator event_aggregator;
        private readonly ApplicationSettings application_settings;
        private readonly ProfileController profile_controller;
        private readonly ApiController api_controller;

        private string _DataDir;
        public string DataDir
        {
            get { return _DataDir; }
            set { this.RaiseAndSetIfChanged(ref _DataDir, value); }
        }

        private string _ImageDir;
        public string ImageDir
        {
            get { return _ImageDir; }
            set { this.RaiseAndSetIfChanged(ref _ImageDir, value); }
        }

        private bool _RememberLastCollection;
        public bool RememberLastCollection
        {
            get { return _RememberLastCollection; }
            set { this.RaiseAndSetIfChanged(ref _RememberLastCollection, value); }
        }

        private ReactiveList<ApiViewModel> _Apis;
        public ReactiveList<ApiViewModel> Apis
        {
            get { return _Apis; }
            set { this.RaiseAndSetIfChanged(ref _Apis, value); }
        }

        [ImportingConstructor]
        public SettingsViewModel(IEventAggregator event_aggregator, ApplicationSettings application_settings, ProfileController profile_controller, ApiController api_controller)
        {
            this.event_aggregator = event_aggregator;
            this.application_settings = application_settings;
            this.profile_controller = profile_controller;
            this.api_controller = api_controller;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            DataDir = application_settings.DataDir;
            ImageDir = application_settings.ImageDir;
            RememberLastCollection = application_settings.RememberLastCollection;

            Apis = api_controller.Apis
                                 .Select(a => new ApiViewModel(a) { IsEnabled = application_settings.IsApiEnabled(a.Name) })
                                 .ToReactiveList();
        }

        public async void ClearRepository()
        {
            var result = await DialogController.ShowMessageAsync("Warning", "Are you sure you want to clear the current collection?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
                profile_controller.Clear();
        }

        public void Ok()
        {
            application_settings.DataDir = DataDir;
            application_settings.ImageDir = ImageDir;
            application_settings.RememberLastCollection = RememberLastCollection;

            Apis.Apply(a => application_settings.SetApiEnabled(a.DisplayName, a.IsEnabled));

            Back();
        }

        public void Cancel()
        {
            Back();
        }

        public void Back()
        {
            event_aggregator.PublishOnUIThread(ShellMessage.Back());
        }
    }
}
