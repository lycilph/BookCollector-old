using Core;
using ReactiveUI;

namespace BookCollector.Screens.Web
{
    public class WebScreenViewModel : ScreenBase, IWebScreen
    {
        private ReactiveCommand _ShowBooksCommand;
        public ReactiveCommand ShowBooksCommand
        {
            get { return _ShowBooksCommand; }
            set { this.RaiseAndSetIfChanged(ref _ShowBooksCommand, value); }
        }

        public WebScreenViewModel()
        {
            ShowBooksCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(ApplicationMessage.ShowBooksScreen));
        }
    }
}
