using System.ComponentModel.Composition;
using BookCollector.Data;
using Framework.Window;
using MahApps.Metro.Controls;
using ReactiveUI;

namespace BookCollector.Settings
{
    [Export("Settings", typeof(IFlyout))]
    public class SettingsViewModel : FlyoutBase
    {
        private readonly ApplicationSettings application_settings;

        public bool KeepStartOpen
        {
            get { return application_settings.KeepStartOpen; }
            set { application_settings.KeepStartOpen = value; }
        }

        [ImportingConstructor]
        public SettingsViewModel(ApplicationSettings application_settings) : base("Settings", Position.Right)
        {
            this.application_settings = application_settings;
            application_settings.PropertyChanging += (sender, args) => this.RaisePropertyChanged(args.PropertyName);
            application_settings.PropertyChanged += (sender, args) => this.RaisePropertyChanged(args.PropertyName);
        }
    }
}
