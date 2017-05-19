using Caliburn.Micro;
using ReactiveUI;

namespace BookCollector.Shell
{
    public class ShellViewModel : ReactiveObject, IHaveDisplayName
    {
        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
        }

        public ShellViewModel()
        {
            DisplayName = "Book Collector";
        }
    }
}
