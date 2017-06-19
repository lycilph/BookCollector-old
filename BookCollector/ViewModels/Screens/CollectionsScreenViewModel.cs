using System;
using System.Linq;
using BookCollector.Domain;
using BookCollector.Framework.Dialog;
using BookCollector.Framework.Extensions;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.ViewModels.Common;
using BookCollector.ViewModels.Dialogs;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace BookCollector.ViewModels.Screens
{
    class CollectionsScreenViewModel : ScreenBase, IHandle<ApplicationMessage>
    {
        private IEventAggregator event_aggregator;
        private ISnackbarMessageQueue message_queue;
        private IApplicationModel application_model;
        private IDialogService dialog_service;

        private ReactiveList<DescriptionViewModel> _CollectionDescriptions;
        public ReactiveList<DescriptionViewModel> CollectionDescriptions
        {
            get { return _CollectionDescriptions; }
            set { this.RaiseAndSetIfChanged(ref _CollectionDescriptions, value); }
        }

        private DescriptionViewModel _SelectedCollectionDescription;
        public DescriptionViewModel SelectedCollectionDescription
        {
            get { return _SelectedCollectionDescription; }
            set { this.RaiseAndSetIfChanged(ref _SelectedCollectionDescription, value); }
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

        private bool _CanCancel;
        public bool CanCancel
        {
            get { return _CanCancel; }
            set { this.RaiseAndSetIfChanged(ref _CanCancel, value); }
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

        public CollectionsScreenViewModel(IEventAggregator event_aggregator, ISnackbarMessageQueue message_queue, IApplicationModel application_model, IDialogService dialog_service)
        {
            this.event_aggregator = event_aggregator;
            this.message_queue = message_queue;
            this.application_model = application_model;
            this.dialog_service = dialog_service;
            DisplayName = Constants.CollectionsScreenDisplayName;

            event_aggregator.Subscribe(this);

            var have_selected_collection = this.WhenAny(x => x.SelectedCollectionDescription, c => c.Value != null);

            ContinueCommand = ReactiveCommand.Create(Continue, have_selected_collection);
            CancelCommand = ReactiveCommand.Create(Cancel, this.WhenAnyValue(x => x.CanCancel));

            EditCommand = ReactiveCommand.Create(EditCollectionAsync, have_selected_collection);
            AddCommand = ReactiveCommand.Create(AddCollectionAsync);
            DeleteCommand = ReactiveCommand.Create(DeleteCollectionAsync, have_selected_collection);
        }

        public override void Activate()
        {
            base.Activate();

            // Get collections from application model
            CollectionDescriptions = application_model.GetAllCollectionDescriptions()
                                                      .Select(d => new DescriptionViewModel(d))
                                                      .ToReactiveList();

            // Get current collection
            if (application_model.CurrentCollection != null)
                SelectedCollectionDescription = CollectionDescriptions.SingleOrDefault(d => d.Filename == application_model.CurrentCollection.Description.Filename);
            else
                SelectedCollectionDescription = CollectionDescriptions.FirstOrDefault();

            // Show snack message if no collections were found
            if (!CollectionDescriptions.Any())
                message_queue.Enqueue("Welcome", "Add Collection", () => AddCollectionAsync());
        }

        public void Handle(ApplicationMessage message)
        {
            if (message.Kind == ApplicationMessage.MessageKind.CollectionChanged)
                CanCancel = application_model.CurrentCollection != null;
        }

        private void Continue()
        {
            application_model.LoadCurrentCollection(SelectedCollectionDescription.Filename);
            event_aggregator.Publish(ApplicationMessage.NavigateTo(Constants.BooksScreenDisplayName));
        }

        private void Cancel()
        {
            event_aggregator.Publish(ApplicationMessage.NavigateTo(Constants.BooksScreenDisplayName));
        }

        private async void AddCollectionAsync()
        {
            var description = application_model.CreateCollectionDescription();
            var description_view_model = new DescriptionViewModel(description);

            var dialog = new CustomDialog { Title = Constants.AddCollectionDialogTitle };
            var vm = new DescriptionDialogViewModel(description_view_model, async (result) =>
            {
                await dialog_service.HideCustomDialogAsync(dialog);

                // Update the actual collection, if needed
                if (result == MessageDialogResult.Affirmative)
                {
                    CollectionDescriptions.Add(description_view_model);
                    SelectedCollectionDescription = description_view_model;
                    application_model.AddCollection(description);
                }
            });
            dialog.Content = vm;

            await dialog_service.ShowCustomDialogAsync(dialog);
        }

        private async void EditCollectionAsync()
        {
            var dialog = new CustomDialog { Title = Constants.EditCollectionDialogTitle };
            var vm = new DescriptionDialogViewModel(SelectedCollectionDescription, async (result) =>
            {
                await dialog_service.HideCustomDialogAsync(dialog);

                // Update the actual collection, if needed
                if (result == MessageDialogResult.Affirmative)
                    application_model.UpdateCollection(SelectedCollectionDescription.Unwrap());
            });
            dialog.Content = vm;

            await dialog_service.ShowCustomDialogAsync(dialog);
        }

        private async void DeleteCollectionAsync()
        {
            // Show confirmation dialog
            var result = await dialog_service.ShowMessageAsync("Warning", $"Are you sure you want to delete the collection \"{SelectedCollectionDescription.Name}\"?", MessageDialogStyle.AffirmativeAndNegative);
            if (result != MessageDialogResult.Affirmative)
                return;

            var collection_to_remove = SelectedCollectionDescription;
            var collection_index = CollectionDescriptions.Select((collection, index) => new { collection, index }).First(p => p.collection == SelectedCollectionDescription).index;

            if (CollectionDescriptions.Count == 1)
            {
                SelectedCollectionDescription = null;
            }
            else
            {
                if (collection_index == 0)
                    SelectedCollectionDescription = CollectionDescriptions.ElementAt(1);
                else
                    SelectedCollectionDescription = CollectionDescriptions.ElementAt(collection_index - 1);
            }

            CollectionDescriptions.Remove(collection_to_remove);
            application_model.DeleteCollection(collection_to_remove.Unwrap());
        }
    }
}
