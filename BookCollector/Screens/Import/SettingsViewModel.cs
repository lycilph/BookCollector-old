using MahApps.Metro.Controls;
using Panda.ApplicationCore.Shell;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    public class SettingsViewModel : FlyoutBase
    {
        private bool _ImportShelves = true;
        public bool ImportShelves
        {
            get { return _ImportShelves; }
            set { this.RaiseAndSetIfChanged(ref _ImportShelves, value); }
        }

        public SettingsViewModel() : base("Settings", Position.Right) { }
    }
}
