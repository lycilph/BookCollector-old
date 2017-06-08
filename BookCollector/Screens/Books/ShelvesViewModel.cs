using BookCollector.Domain;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.Screens.Books
{
    public class ShelvesViewModel : ScreenBase
    {
        private ReactiveCommand _AddCommand;
        public ReactiveCommand AddCommand
        {
            get { return _AddCommand; }
            set { this.RaiseAndSetIfChanged(ref _AddCommand, value); }
        }

        public ShelvesViewModel(IEventAggregator event_aggregator)
        {
            DisplayName = "Shelves";

            AddCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.ToggleMainMenuMessage()));
        }
    }
}
