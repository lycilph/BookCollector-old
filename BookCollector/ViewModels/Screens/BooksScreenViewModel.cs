using BookCollector.Domain;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.ViewModels.Screens
{
    public class BooksScreenViewModel : ScreenBase
    {
        private ReactiveCommand _ShowCollectionsCommand;
        public ReactiveCommand ShowCollectionsCommand
        {
            get { return _ShowCollectionsCommand; }
            set { this.RaiseAndSetIfChanged(ref _ShowCollectionsCommand, value); }
        }

        public BooksScreenViewModel(IEventAggregator event_aggregator)
        {
            DisplayName = Constants.BooksScreenDisplayName;

            ShowCollectionsCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateTo(Constants.CollectionsScreenDisplayName)));
        }
    }
}
