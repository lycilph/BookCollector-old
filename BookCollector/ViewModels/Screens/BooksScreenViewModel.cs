using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Data;
using BookCollector.Data;
using BookCollector.Domain;
using BookCollector.Framework.Dialog;
using BookCollector.Framework.Extensions;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.Models;
using BookCollector.Services.Search;
using BookCollector.ViewModels.Data;
using BookCollector.ViewModels.Dialogs;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;

namespace BookCollector.ViewModels.Screens
{
    public class BooksScreenViewModel : ScreenBase, IHandle<ApplicationMessage>
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private ICollectionModel collection_model;
        private IDialogService dialog_service;
        private ISearchEngine search_engine;
        private List<SearchResult> search_results;

        private ICollectionView _Books;
        public ICollectionView Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private ReactiveList<ShelfViewModel> _Shelves;
        public ReactiveList<ShelfViewModel> Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
        }

        private ShelfViewModel _SelectedShelf;
        public ShelfViewModel SelectedShelf
        {
            get { return _SelectedShelf; }
            set { this.RaiseAndSetIfChanged(ref _SelectedShelf, value); }
        }

        private ReactiveList<ShelfSelectionViewModel> _ShelfSelections = new ReactiveList<ShelfSelectionViewModel>();
        public ReactiveList<ShelfSelectionViewModel> ShelfSelections
        {
            get { return _ShelfSelections; }
            set { this.RaiseAndSetIfChanged(ref _ShelfSelections, value); }
        }

        private ReactiveList<string> _SortProperties;
        public ReactiveList<string> SortProperties
        {
            get { return _SortProperties; }
            set { this.RaiseAndSetIfChanged(ref _SortProperties, value); }
        }

        private string _SelectedSortProperty;
        public string SelectedSortProperty
        {
            get { return _SelectedSortProperty; }
            set { this.RaiseAndSetIfChanged(ref _SelectedSortProperty, value); }
        }

        private bool _SortAscending;
        public bool SortAscending
        {
            get { return _SortAscending; }
            set { this.RaiseAndSetIfChanged(ref _SortAscending, value); }
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

        public BooksScreenViewModel(ICollectionModel collection_model, IDialogService dialog_service, IEventAggregator event_aggregator, ISearchEngine search_engine)
        {
            DisplayName = Constants.BooksScreenDisplayName;
            this.collection_model = collection_model;
            this.dialog_service = dialog_service;
            this.search_engine = search_engine;

            event_aggregator.Subscribe(this);

            SortProperties = new ReactiveList<string> { "Title", "Authors" };
            SelectedSortProperty = SortProperties.First();
            SortAscending = true;

            var can_edit_or_delete = this.WhenAny(x => x.SelectedShelf, x => x.Value != null && x.Value.IsLocked == false);
            EditCommand = ReactiveCommand.Create(EditShelfAsync, can_edit_or_delete);
            DeleteCommand = ReactiveCommand.Create(DeleteShelfAsync, can_edit_or_delete);
            AddCommand = ReactiveCommand.Create(AddShelfAsync);

            this.WhenAnyValue(x => x.SelectedSortProperty, x => x.SortAscending)
                .Subscribe(_ => UpdateSorting());

            this.WhenAnyValue(x => x.SelectedShelf)
                .Subscribe(_ =>
                {
                    Books?.Refresh();
                    Books?.MoveCurrentToFirst();
                });
        }

        public override void Activate()
        {
            base.Activate();

            var vms = collection_model.CurrentCollection.Books.Select(b => new BookViewModel(b));
            Books = CollectionViewSource.GetDefaultView(vms);
            Books.Filter = Filter;
            Books.CurrentChanged += (sender, args) => 
            {
                if (Books.CurrentItem != null)
                    ShelfSelections.Apply(s => s.Book = (Books.CurrentItem as BookViewModel).Obj);
            };

            UpdateSorting();

            Shelves = collection_model.CurrentCollection.Shelves.Select(s => new ShelfViewModel(s)).ToReactiveList();
            SelectedShelf = Shelves.Single(s => s.Obj == collection_model.CurrentCollection.DefaultShelf);

            if (vms.Any())
            {
                var selected_book = (Books.CurrentItem as BookViewModel).Obj;
                ShelfSelections = collection_model.CurrentCollection.Shelves.Select(s => new ShelfSelectionViewModel(selected_book, s)).ToReactiveList();
            }
        }

        public void Handle(ApplicationMessage message)
        {
            if (message.Kind == ApplicationMessage.MessageKind.SearchTextChanged)
            {
                log.Info($"Search text changed - {message.Text}");
                var sw = Stopwatch.StartNew();
                search_results = search_engine.Search(message.Text);
                sw.Stop();
                log.Info($"Search took {sw.ElapsedMilliseconds} ms and gave {(search_results == null ? 0 : search_results.Count)} results");

                Books?.Refresh();
                Books?.MoveCurrentToFirst();
            }
        }

        private void UpdateSorting()
        {
            if (Books == null)
                return;

            var direction = (SortAscending ? ListSortDirection.Ascending : ListSortDirection.Descending);

            var primary_sort_property = SelectedSortProperty;
            var secondary_sort_property = SortProperties.First(p => p != primary_sort_property);

            Books.SortDescriptions.Clear();
            Books.SortDescriptions.Add(new SortDescription(primary_sort_property, direction));
            Books.SortDescriptions.Add(new SortDescription(secondary_sort_property, direction));
        }

        private bool Filter(object o)
        {
            var book = o as BookViewModel;
            if (book == null || SelectedShelf == null)
                return false;

            if (search_results != null)
                return search_results.Any(s => s.Book.Title == book.Title) && book.IsOnShelf(SelectedShelf.Obj);
            else
                return book.IsOnShelf(SelectedShelf.Obj);
        }

        private async void EditShelfAsync()
        {
            var dialog = new CustomDialog { Title = Constants.EditShelfDialogTitle };
            var vm = new ShelfDialogViewModel(SelectedShelf, async (result) => await dialog_service.HideCustomDialogAsync(dialog));
            dialog.Content = vm;

            await dialog_service.ShowCustomDialogAsync(dialog);
        }

        private async void AddShelfAsync()
        {
            var shelf = new Shelf("");
            var shelf_view_model = new ShelfViewModel(shelf);

            Book selected_book = null;
            if (Books.CurrentItem != null)
                selected_book = (Books.CurrentItem as BookViewModel).Obj;
            var shelf_selection_view_model = new ShelfSelectionViewModel(selected_book, shelf);

            var dialog = new CustomDialog { Title = Constants.AddShelfDialogTitle };
            var vm = new ShelfDialogViewModel(shelf_view_model, async (result) =>
            {
                await dialog_service.HideCustomDialogAsync(dialog);

                if (result == MessageDialogResult.Affirmative)
                {
                    collection_model.CurrentCollection.Add(shelf);
                    Shelves.Add(shelf_view_model);
                    ShelfSelections.Add(shelf_selection_view_model);
                }
            });
            dialog.Content = vm;

            await dialog_service.ShowCustomDialogAsync(dialog);
        }

        public async void DeleteShelfAsync()
        {
            // Show confirmation dialog
            var result = await dialog_service.ShowMessageAsync("Warning", $"Are you sure you want to delete the shelf \"{SelectedShelf.Name}\"?", MessageDialogStyle.AffirmativeAndNegative);
            if (result != MessageDialogResult.Affirmative)
                return;

            // Find current index of selected shelf
            var shelf_to_remove = SelectedShelf;
            var selection_shelf_to_remove = ShelfSelections.Single(s => s.Shelf == shelf_to_remove.Obj);
            var shelf_index = Shelves.Select((shelf, index) => new { shelf, index }).First(p => p.shelf == SelectedShelf).index;

            // Select new item
            SelectedShelf = Shelves.ElementAt(shelf_index - 1);

            // Remove item
            collection_model.CurrentCollection.Remove(shelf_to_remove.Obj);
            Shelves.Remove(shelf_to_remove);
            ShelfSelections.Remove(selection_shelf_to_remove);
        }
    }
}
