using ReactiveUI;

namespace BookCollector.Screens.Import
{
    public class ColumnViewModel : ReactiveObject
    {
        public string Name { get; set; }

        private string _PropertyName;
        public string PropertyName
        {
            get { return _PropertyName ?? Name; }
            set { _PropertyName = value; }
        }

        public bool UseTemplate { get; set; }

        private bool _CanSetVisibility = true;
        public bool CanSetVisibility
        {
            get { return _CanSetVisibility; }
            set { _CanSetVisibility = value; }
        }

        private bool _IsVisible = true;
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { this.RaiseAndSetIfChanged(ref _IsVisible, value); }
        }
    }
}
