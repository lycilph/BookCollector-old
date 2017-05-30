using System.Linq;
using System.Threading.Tasks;
using BookCollector.Controllers;
using BookCollector.Data;
using BookCollector.Extensions;
using BookCollector.Framework.Logging;
using BookCollector.Framework.MessageBus;
using BookCollector.Framework.Messages;
using BookCollector.Models;
using BookCollector.Shell;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;

namespace BookCollector.Screens.Start
{
    public class StartViewModel : ShellScreenBase
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IEventAggregator event_aggregator;
        private IDialogService dialog_service;
        private IDataController data_controller;
        private IBookCollectorModel book_collector_model;

        private ReactiveList<CollectionDescriptionViewModel> _Collections = new ReactiveList<CollectionDescriptionViewModel>();
        public ReactiveList<CollectionDescriptionViewModel> Collections
        {
            get { return _Collections; }
            set { this.RaiseAndSetIfChanged(ref _Collections, value); }
        }

        private CollectionDescriptionViewModel _SelectedCollection;
        public CollectionDescriptionViewModel SelectedCollection
        {
            get { return _SelectedCollection; }
            set { this.RaiseAndSetIfChanged(ref _SelectedCollection, value); }
        }

        private ReactiveCommand _AddCollectionCommand;
        public ReactiveCommand AddCollectionCommand
        {
            get { return _AddCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _AddCollectionCommand, value); }
        }

        private ReactiveCommand _RemoveCollectionCommand;
        public ReactiveCommand RemoveCollectionCommand
        {
            get { return _RemoveCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _RemoveCollectionCommand, value); }
        }


        private ReactiveCommand _EditCollectionCommand;
        public ReactiveCommand EditCollectionCommand
        {
            get { return _EditCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _EditCollectionCommand, value); }
        }

        private ReactiveCommand _ContinueCommand;
        public ReactiveCommand ContinueCommand
        {
            get { return _ContinueCommand; }
            set { this.RaiseAndSetIfChanged(ref _ContinueCommand, value); }
        }

        public StartViewModel(IEventAggregator event_aggregator, IDialogService dialog_service, IDataController data_controller, IBookCollectorModel book_collector_model)
        {
            log.Info("StartViewModel created");

            this.event_aggregator = event_aggregator;
            this.dialog_service = dialog_service;
            this.data_controller = data_controller;
            this.book_collector_model = book_collector_model;

            DisplayName = ScreenNames.StartScreenName;

            var have_selected_collection = this.WhenAny(x => x.SelectedCollection, c => c.Value != null);

            ContinueCommand = ReactiveCommand.Create(() => Continue(SelectedCollection.Filename), have_selected_collection);
            RemoveCollectionCommand = ReactiveCommand.Create(() => RemoveCollectionAsync(), have_selected_collection);
            EditCollectionCommand = ReactiveCommand.Create(() => EditCollectionAsync(), have_selected_collection);
            AddCollectionCommand = ReactiveCommand.Create(() => AddCollectionAsync());
        }

        public override void Activate()
        {
            // Load collection descriptions from data
            Collections = data_controller.GetAllCollectionDescriptions()
                                         .Select(c => new CollectionDescriptionViewModel(c))
                                         .ToReactiveList();

            // See if the model already has a current collection and try to match it to the list
            if (book_collector_model.CurrentCollection != null)
                SelectedCollection = Collections.SingleOrDefault(x => x.Matches(book_collector_model.CurrentCollection.Description));

            // If no current collection, take the first
            if (SelectedCollection == null && Collections.Any())
                SelectedCollection = Collections.First();
        }

        private void Continue(string filename)
        {
            // Update model
            book_collector_model.LoadAndSetCurrentCollection(filename);
            // Navigate to main screen
            event_aggregator.Publish(new NavigationMessage(ScreenNames.MainScreenName));
        }

        private async Task AddCollectionAsync()
        {
            var collection = new Collection
            {
                Description = new CollectionDescription
                {
                    Name = "New Collection",
                    Description = "Description of collection"
                }
            };
            var description_view_model = new CollectionDescriptionViewModel(collection.Description);

            var dialog = new CustomDialog { Title = "Add Collection" };
            var vm = new CollectionDialogViewModel(description_view_model, async () =>
            {
                await dialog_service.HideCustomDialogAsync(dialog);
                // Save collection to file
                data_controller.SaveCollection(collection);
                // Continue with new collection
                Continue(collection.Description.Filename);
            });
            dialog.Content = vm;

            await dialog_service.ShowCustomDialogAsync(dialog);
        }

        private async Task EditCollectionAsync()
        {
            var dialog = new CustomDialog { Title = "Edit Collection" };
            var vm = new CollectionDialogViewModel(SelectedCollection, async () =>
            {
                await dialog_service.HideCustomDialogAsync(dialog);
                // Update the actual collection
                data_controller.UpdateCollectionDescription(SelectedCollection.Unwrap());
            });
            dialog.Content = vm;

            await dialog_service.ShowCustomDialogAsync(dialog);
        }

        private async Task RemoveCollectionAsync()
        {
            // Show confirmation dialog
            var result = await dialog_service.ShowMessageAsync("Warning", "Are you sure you want to delete the collection?", MessageDialogStyle.AffirmativeAndNegative);
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
            data_controller.DeleteCollection(collection_to_remove.Filename);
        }
    }
}
