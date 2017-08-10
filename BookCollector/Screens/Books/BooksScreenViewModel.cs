using Core.Shell;
using ReactiveUI;

namespace BookCollector.Screens.Books
{
    public class BooksScreenViewModel : ScreenBase, IBooksScreen
    {
        private ReactiveCommand _ShowImportCommand;
        public ReactiveCommand ShowImportCommand
        {
            get { return _ShowImportCommand; }
            set { this.RaiseAndSetIfChanged(ref _ShowImportCommand, value); }
        }

        public BooksScreenViewModel()
        {
            DisplayName = "Books";
            ShowImportCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(ApplicationMessage.ShowImportScreen));
        }
    }
}
