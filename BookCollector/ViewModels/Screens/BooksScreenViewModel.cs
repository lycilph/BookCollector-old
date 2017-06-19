using BookCollector.Domain;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using MaterialDesignThemes.Wpf;

namespace BookCollector.ViewModels.Screens
{
    public class BooksScreenViewModel : ScreenBase
    {
        private IEventAggregator event_aggregator;
        private ISnackbarMessageQueue message_queue;

        public BooksScreenViewModel(IEventAggregator event_aggregator, ISnackbarMessageQueue message_queue)
        {
            this.event_aggregator = event_aggregator;
            this.message_queue = message_queue;

            DisplayName = Constants.BooksScreenDisplayName;
        }

        public override void Activate()
        {
            base.Activate();

            message_queue.Enqueue("No Books?", "Import Here", () => event_aggregator.Publish(ApplicationMessage.NavigateTo(Constants.ImportScreenDisplayName)));
        }
    }
}
