using System;
using System.Linq;
using System.Threading.Tasks;
using BookCollector.Domain;
using BookCollector.Framework.Dialog;
using BookCollector.Framework.Extensions;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.Models;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;

namespace BookCollector.Screens.Collections
{
    public class CollectionsViewModel : MainScreenBase
    {
        private IEventAggregator event_aggregator;
        private IDialogService dialog_service;
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

        private ReactiveCommand _AddCommand;
        public ReactiveCommand AddCommand
        {
            get { return _AddCommand; }
            set { this.RaiseAndSetIfChanged(ref _AddCommand, value); }
        }

        private ReactiveCommand _EditCommand;
        public ReactiveCommand EditCommand
        {
            get { return _EditCommand; }
            set { this.RaiseAndSetIfChanged(ref _EditCommand, value); }
        }

        private ReactiveCommand _DeleteCommand;
        public ReactiveCommand DeleteCommand
        {
            get { return _DeleteCommand; }
            set { this.RaiseAndSetIfChanged(ref _DeleteCommand, value); }
        }

        public CollectionsViewModel(IEventAggregator event_aggregator, IDialogService dialog_service, IApplicationModel application_model)
        {
            this.event_aggregator = event_aggregator;
            this.dialog_service = dialog_service;
            this.application_model = application_model;

            DisplayName = ScreenNames.CollectionsName;
            ShowCollectionCommand = false;

            var have_selected_collection = this.WhenAny(x => x.SelectedCollection, c => c.Value != null);
            var have_current_collection = application_model.WhenAny(x => x.CurrentCollection, c => c.Value != null);

            ContinueCommand = ReactiveCommand.Create(Continue, have_selected_collection);
            CancelCommand = ReactiveCommand.Create(Cancel, have_current_collection);

            EditCommand = ReactiveCommand.Create(EditCollectionAsync, have_selected_collection);
            DeleteCommand = ReactiveCommand.Create(DeleteCollectionAsync, have_selected_collection);
            AddCommand = ReactiveCommand.Create(AddCollectionAsync);
        }

        public override void Activate()
        {
            Collections = application_model.GetAllCollectionDescriptions().Select(d => new DescriptionViewModel(d)).ToReactiveList();

            // See if the model already has a current collection and try to match it to the list
            if (application_model.CurrentCollection != null)
                SelectedCollection = Collections.SingleOrDefault(c => c.Matches(application_model.CurrentCollection.Description));

            // If no current collection, take the first
            if (SelectedCollection == null && Collections.Any())
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

        private async void EditCollectionAsync()
        {
            var dialog = new CustomDialog { Title = "Edit Collection" };
            var vm = new CollectionDialogViewModel(SelectedCollection, async (result) =>
            {
                await dialog_service.HideCustomDialogAsync(dialog);

                // Update the actual collection, if needed
                if (result == MessageDialogResult.Affirmative)
                    application_model.UpdateCollection(SelectedCollection.Unwrap());
            });
            dialog.Content = vm;

            await dialog_service.ShowCustomDialogAsync(dialog);
        }

        private async void AddCollectionAsync()
        {
            var description = new DescriptionViewModel(new Description());

            var dialog = new CustomDialog { Title = "Add Collection" };
            var vm = new CollectionDialogViewModel(description, async (result) =>
            {
                await dialog_service.HideCustomDialogAsync(dialog);

                // Update the actual collection, if needed
                if (result == MessageDialogResult.Affirmative)
                {
                    Collections.Add(description);
                    SelectedCollection = description;
                    application_model.AddCollection(description.Unwrap());
                }
            });
            dialog.Content = vm;

            await dialog_service.ShowCustomDialogAsync(dialog);
        }

        private async void DeleteCollectionAsync()
        {
            // Show confirmation dialog
            var result = await dialog_service.ShowMessageAsync("Warning", $"Are you sure you want to delete the collection \"{SelectedCollection.Name}\"?", MessageDialogStyle.AffirmativeAndNegative);
            if (result != MessageDialogResult.Affirmative)
                return;

            var collection_to_remove = SelectedCollection;
            var collection_index = Collections.Select((collection, index) => new { collection, index }).First(p => p.collection == SelectedCollection).index;

            if (Collections.Count == 1)
            {
                SelectedCollection = null;
            }
            else
            {
                if (collection_index == 0)
                    SelectedCollection = Collections.ElementAt(1);
                else
                    SelectedCollection = Collections.ElementAt(collection_index - 1);
            }

            Collections.Remove(collection_to_remove);
            application_model.DeleteCollection(collection_to_remove.Unwrap());
        }
    }
}
