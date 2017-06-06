using BookCollector.Domain;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.Screens.Collections
{
    public class CollectionsViewModel : MainScreenBase
    {
        private ReactiveCommand _ContinueCommand;
        public ReactiveCommand ContinueCommand
        {
            get { return _ContinueCommand; }
            set { this.RaiseAndSetIfChanged(ref _ContinueCommand, value); }
        }

        public CollectionsViewModel(IEventAggregator event_aggregator)
        {
            DisplayName = ScreenNames.CollectionsName;
            ShowCollectionCommand = false;

            ContinueCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateToMessage(ScreenNames.BooksName)));
        }
    }
}
