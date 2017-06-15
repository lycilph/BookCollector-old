using BookCollector.Domain;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.ViewModels.Screens
{
    class CollectionsScreenViewModel : ScreenBase
    {
        private ReactiveCommand _ShowBooksCommand;
        public ReactiveCommand ShowBooksCommand
        {
            get { return _ShowBooksCommand; }
            set { this.RaiseAndSetIfChanged(ref _ShowBooksCommand, value); }
        }
        
        public CollectionsScreenViewModel(IEventAggregator event_aggregator)
        {
            DisplayName = Constants.CollectionsScreenDisplayName;

            ShowBooksCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateTo(Constants.BooksScreenDisplayName)));
        }
    }
}
