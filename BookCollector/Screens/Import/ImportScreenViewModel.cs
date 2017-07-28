using Core;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    public class ImportScreenViewModel : ScreenBase, IImportScreen
    {
        private ReactiveCommand _ShowBooksCommand;
        public ReactiveCommand ShowBooksCommand
        {
            get { return _ShowBooksCommand; }
            set { this.RaiseAndSetIfChanged(ref _ShowBooksCommand, value); }
        }

        public ImportScreenViewModel()
        {
            DisplayName = "Import";
            ShowBooksCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(ApplicationMessage.ShowBooksScreen));
        }
    }
}
