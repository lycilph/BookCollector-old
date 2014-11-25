using ReactiveUI;

namespace BookCollector.Import
{
    public class ImportStepViewModel : ReactiveObject
    {
        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
        }

        private bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }
            set { this.RaiseAndSetIfChanged(ref _IsActive, value); }
        }

        private bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { this.RaiseAndSetIfChanged(ref _IsEnabled, value); }
        }

        public ImportStepViewModel(string name)
        {
            _DisplayName = name;
        }
    }
}
