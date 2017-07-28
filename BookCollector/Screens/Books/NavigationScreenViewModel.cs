using Core;
using ReactiveUI;

namespace BookCollector.Screens.Books
{
    public class NavigationScreenViewModel : ScreenBase, INavigationScreen
    {
        private ReactiveCommand _ShowWebCommand;
        public ReactiveCommand ShowWebCommand
        {
            get { return _ShowWebCommand; }
            set { this.RaiseAndSetIfChanged(ref _ShowWebCommand, value); }
        }

        public NavigationScreenViewModel()
        {
            ShowWebCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(ApplicationMessage.ShowWebScreen));
        }
    }
}
