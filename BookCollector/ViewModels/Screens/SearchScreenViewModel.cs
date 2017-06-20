using BookCollector.Domain;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.ViewModels.Screens
{
    public class SearchScreenViewModel : ScreenBase
    {
        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { this.RaiseAndSetIfChanged(ref _Text, value); }
        }

        private ReactiveCommand _ClearCommand;
        public ReactiveCommand ClearCommand
        {
            get { return _ClearCommand; }
            set { this.RaiseAndSetIfChanged(ref _ClearCommand, value); }
        }

        public SearchScreenViewModel()
        {
            DisplayName = Constants.SearchScreenDisplayName;

            ClearCommand = ReactiveCommand.Create(() => Text = string.Empty);
        }
    }
}
