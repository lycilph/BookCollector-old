using BookCollector.Domain;
using BookCollector.Framework.Dialog;
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

        private ReactiveCommand _DialogCommand;
        public ReactiveCommand DialogCommand
        {
            get { return _DialogCommand; }
            set { this.RaiseAndSetIfChanged(ref _DialogCommand, value); }
        }

        public CollectionsViewModel(IEventAggregator event_aggregator, IDialogService dialog_service)
        {
            DisplayName = ScreenNames.CollectionsName;
            ShowCollectionCommand = false;

            ContinueCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateToMessage(ScreenNames.BooksName)));
            DialogCommand = ReactiveCommand.Create(() => dialog_service.ShowMessageAsync("Dialog", "Testing the dialog service"));
        }
    }
}
