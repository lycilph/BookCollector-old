using System;
using System.Reactive.Linq;
using BookCollector.Domain;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.ViewModels.Screens
{
    public class SearchScreenViewModel : ScreenBase
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

        public SearchScreenViewModel(IEventAggregator event_aggregator)
        {
            DisplayName = Constants.SearchScreenDisplayName;

            this.WhenAny(x => x.Text, x => !string.IsNullOrWhiteSpace(x.Value))
                .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
                .Subscribe(_ => event_aggregator.Publish(ApplicationMessage.SearchTextChanged(Text)));

            ClearCommand = ReactiveCommand.Create(() => Text = string.Empty);
        }
    }
}
