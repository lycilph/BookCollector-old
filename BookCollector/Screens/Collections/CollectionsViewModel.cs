using System.Linq;
using BookCollector.Domain;
using BookCollector.Framework.Extensions;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.Models;
using ReactiveUI;

namespace BookCollector.Screens.Collections
{
    public class CollectionsViewModel : MainScreenBase
    {
        private IEventAggregator event_aggregator;
        private IApplicationModel application_model;

        private ReactiveList<DescriptionViewModel> _Collections;
        public ReactiveList<DescriptionViewModel> Collections
        {
            get { return _Collections; }
            set { this.RaiseAndSetIfChanged(ref _Collections, value); }
        }

        private DescriptionViewModel _SelectedCollection;
        public DescriptionViewModel SelectedCollection
        {
            get { return _SelectedCollection; }
            set { this.RaiseAndSetIfChanged(ref _SelectedCollection, value); }
        }

        private ReactiveCommand _ContinueCommand;
        public ReactiveCommand ContinueCommand
        {
            get { return _ContinueCommand; }
            set { this.RaiseAndSetIfChanged(ref _ContinueCommand, value); }
        }

        private ReactiveCommand _CancelCommand;
        public ReactiveCommand CancelCommand
        {
            get { return _CancelCommand; }
            set { this.RaiseAndSetIfChanged(ref _CancelCommand, value); }
        }

        public CollectionsViewModel(IEventAggregator event_aggregator, IApplicationModel application_model)
        {
            this.event_aggregator = event_aggregator;
            this.application_model = application_model;

            DisplayName = ScreenNames.CollectionsName;
            ShowCollectionCommand = false;

            var have_selected_collection = this.WhenAny(x => x.SelectedCollection, c => c.Value != null);
            var have_current_collection = application_model.WhenAny(x => x.CurrentCollection, c => c.Value != null);

            ContinueCommand = ReactiveCommand.Create(Continue, have_selected_collection);
            CancelCommand = ReactiveCommand.Create(Cancel, have_current_collection);
        }

        public override void Activate()
        {
            Collections = application_model.GetAllCollectionDescriptions().Select(d => new DescriptionViewModel(d)).ToReactiveList();
            SelectedCollection = Collections.FirstOrDefault();
        }

        private void Continue()
        {
            application_model.LoadCurrentCollection(SelectedCollection.Filename);
            event_aggregator.Publish(ApplicationMessage.NavigateToMessage(ScreenNames.BooksName));
        }

        private void Cancel()
        {
            event_aggregator.Publish(ApplicationMessage.NavigateToMessage(ScreenNames.BooksName));
        }
    }
}
