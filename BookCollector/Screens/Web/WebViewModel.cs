using BookCollector.Domain;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.Screens.Web
{
    public class WebViewModel : ScreenBase
    {
        private string _Address;
        public string Address
        {
            get { return _Address; }
            set { this.RaiseAndSetIfChanged(ref _Address, value); }
        }

        private ReactiveCommand _HomeCommand;
        public ReactiveCommand HomeCommand
        {
            get { return _HomeCommand; }
            set { this.RaiseAndSetIfChanged(ref _HomeCommand, value); }
        }

        private ReactiveCommand _BackCommand;
        public ReactiveCommand BackCommand
        {
            get { return _BackCommand; }
            set { this.RaiseAndSetIfChanged(ref _BackCommand, value); }
        }

        public WebViewModel(IEventAggregator event_aggregator)
        {
            DisplayName = ScreenNames.WebName;
            Address = "http://www.goodreads.com";
            HomeCommand = ReactiveCommand.Create(() => Address = "http://www.goodreads.com");
            BackCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateToMessage(ScreenNames.BooksName)));
        }
    }
}
