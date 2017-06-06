using BookCollector.Domain;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.Screens.Web
{
    public class WebViewModel : ScreenBase
    {
        private ReactiveCommand _BackCommand;
        public ReactiveCommand BackCommand
        {
            get { return _BackCommand; }
            set { this.RaiseAndSetIfChanged(ref _BackCommand, value); }
        }

        public WebViewModel(IEventAggregator event_aggregator)
        {
            DisplayName = ScreenNames.WebName;

            BackCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateToMessage(ScreenNames.BooksName)));
        }
    }
}
