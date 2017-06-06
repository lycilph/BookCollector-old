using BookCollector.Domain;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.Screens.Books
{
    public class BooksViewModel : MainScreenBase
    {
        private ReactiveCommand _WebCommand;
        public ReactiveCommand WebCommand
        {
            get { return _WebCommand; }
            set { this.RaiseAndSetIfChanged(ref _WebCommand, value); }
        }

        public BooksViewModel(IEventAggregator event_aggregator, SearchViewModel search_view_model, MenuViewModel menu_view_model)
        {
            DisplayName = ScreenNames.BooksName;
            ShowCollectionCommand = true;
            ExtraContent = search_view_model;
            MenuContent = menu_view_model;

            WebCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateToMessage(ScreenNames.WebName)));
        }
    }
}
