using BookCollector.Application.Messages;
using BookCollector.Framework.MessageBus;
using BookCollector.Shell;
using MahApps.Metro.Controls;
using ReactiveUI;

namespace BookCollector.Screens.Main
{
    public class MainToolsViewModel : FlyoutBase
    {
        private ReactiveCommand _ChangeCollectionCommand;
        public ReactiveCommand ChangeCollectionCommand
        {
            get { return _ChangeCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _ChangeCollectionCommand, value); }
        }

        private ReactiveCommand _ImportCommand;
        public ReactiveCommand ImportCommand
        {
            get { return _ImportCommand; }
            set { this.RaiseAndSetIfChanged(ref _ImportCommand, value); }
        }

        public MainToolsViewModel(IEventAggregator event_aggregator) : base("Tools", Position.Right)
        {
            ChangeCollectionCommand = ReactiveCommand.Create(() =>
            {
                Hide();
                event_aggregator.Publish(new NavigationMessage(ScreenNames.StartScreenName));
            });

            ImportCommand = ReactiveCommand.Create(Hide);
        }
    }
}
