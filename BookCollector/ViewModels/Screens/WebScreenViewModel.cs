using System;
using BookCollector.Domain;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.ViewModels.Screens
{
    public class WebScreenViewModel : ScreenBase, IHandle<ApplicationMessage>
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

        public WebScreenViewModel(IEventAggregator event_aggregator)
        {
            DisplayName = Constants.WebScreenDisplayName;
            Address = "http://www.goodreads.com";
            HomeCommand = ReactiveCommand.Create(() => Address = "http://www.goodreads.com");
            BackCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateTo(Constants.BooksScreenDisplayName)));

            event_aggregator.Subscribe(this);
        }

        public void Handle(ApplicationMessage message)
        {
            if (message.Kind == ApplicationMessage.MessageKind.SearchOnWeb)
            {
                var search_text = message.Text.Replace(' ', '+');
                Address = "https://www.goodreads.com/search?q=" + search_text;
            }
        }
    }
}
