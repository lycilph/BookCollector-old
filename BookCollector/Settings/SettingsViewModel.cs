using System.ComponentModel.Composition;
using BookCollector.Services;
using BookCollector.Shell;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;

namespace BookCollector.Settings
{
    [Export("Settings", typeof(IScreen))]
    public class SettingsViewModel : ReactiveScreen
    {
        private readonly IEventAggregator event_aggregator;
        private readonly BookRepository book_repository;

        [ImportingConstructor]
        public SettingsViewModel(BookRepository book_repository, IEventAggregator event_aggregator)
        {
            this.book_repository = book_repository;
            this.event_aggregator = event_aggregator;
        }

        public void Clear()
        {
            book_repository.Clear();
        }

        public void Back()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.BackMessage());
        }
    }
}
