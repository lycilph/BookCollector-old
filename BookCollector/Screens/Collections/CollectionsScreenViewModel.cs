using Core;
using ReactiveUI;

namespace BookCollector.Screens.Collections
{
    public class CollectionsScreenViewModel : ScreenBase, ICollectionsScreen
    {
        private ReactiveCommand _ShowBooksCommand;
        public ReactiveCommand ShowBooksCommand
        {
            get { return _ShowBooksCommand; }
            set { this.RaiseAndSetIfChanged(ref _ShowBooksCommand, value); }
        }

        public CollectionsScreenViewModel()
        {
            DisplayName = "Collections";
            ShowBooksCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(ApplicationMessage.ShowBooksScreen));
        }
    }
}
