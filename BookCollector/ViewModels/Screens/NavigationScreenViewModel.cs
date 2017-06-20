using System.Reactive.Linq;
using BookCollector.Domain;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.ViewModels.Screens
{
    public class NavigationScreenViewModel : ScreenBase
    {
        private ReactiveCommand _ImportCommand;
        public ReactiveCommand ImportCommand
        {
            get { return _ImportCommand; }
            set { this.RaiseAndSetIfChanged(ref _ImportCommand, value); }
        }

        private ReactiveCommand _GoodreadsCommand;
        public ReactiveCommand GoodreadsCommand
        {
            get { return _GoodreadsCommand; }
            set { this.RaiseAndSetIfChanged(ref _GoodreadsCommand, value); }
        }

        private ReactiveCommand _GooglePlayCommand;
        public ReactiveCommand GooglePlayCommand
        {
            get { return _GooglePlayCommand; }
            set { this.RaiseAndSetIfChanged(ref _GooglePlayCommand, value); }
        }

        public NavigationScreenViewModel(IEventAggregator event_aggregator)
        {
            DisplayName = Constants.NavigationScreenDisplayName;

            ImportCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateTo(Constants.ImportScreenDisplayName)));

            var disable = Observable.Return(false);
            GoodreadsCommand = ReactiveCommand.Create(() => { }, disable);
            GooglePlayCommand = ReactiveCommand.Create(() => { }, disable);
        }
    }
}
