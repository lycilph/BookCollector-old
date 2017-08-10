using Core.Shell;
using ReactiveUI;

namespace BookCollector.Screens.Books
{
    public class SearchScreenViewModel : ScreenBase, ISearchScreen
    {
        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { this.RaiseAndSetIfChanged(ref _Text, value); }
        }

        private ReactiveCommand _ClearCommand;
        public ReactiveCommand ClearCommand
        {
            get { return _ClearCommand; }
            set { this.RaiseAndSetIfChanged(ref _ClearCommand, value); }
        }

        public SearchScreenViewModel()
        {
            //this.WhenAny(x => x.Text, x => !string.IsNullOrWhiteSpace(x.Value))
            //    .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
            //    .Subscribe(_ => event_aggregator.Publish(ApplicationMessage.SearchTextChanged(Text)));

            ClearCommand = ReactiveCommand.Create(() => Text = string.Empty);
        }
    }
}
