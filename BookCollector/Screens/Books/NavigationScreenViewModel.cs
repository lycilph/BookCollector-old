using System.Reactive.Linq;
using Core.Shell;
using ReactiveUI;

namespace BookCollector.Screens.Books
{
    public class NavigationScreenViewModel : ScreenBase, INavigationScreen
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

        public NavigationScreenViewModel()
        {
            DisplayName = "Navigation";

            ImportCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(ApplicationMessage.ShowImportScreen));
            GoodreadsCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(ApplicationMessage.ShowWebScreen));
            GooglePlayCommand = ReactiveCommand.Create(() => { }, Observable.Return(false));
        }
    }
}
