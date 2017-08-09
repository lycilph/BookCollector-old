﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using BookCollector.Data;
using BookCollector.Services;
using BookCollector.ThirdParty.Goodreads;
using Core;
using Core.Extensions;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using NLog;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    public class ImportScreenViewModel : ScreenBase, IImportScreen
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private ICollectionsService collections_service;
        private IImportService import_service;
        private IDialogService dialog_service;

        private string _Filename = string.Empty;
        public string Filename
        {
            get { return _Filename; }
            set { this.RaiseAndSetIfChanged(ref _Filename, value); }
        }

        private int _MaximumSimilarity = 25;
        public int MaximumSimilarity
        {
            get { return _MaximumSimilarity; }
            set { this.RaiseAndSetIfChanged(ref _MaximumSimilarity, value); }
        }

        private string _SelectedBooksText = string.Empty;
        public string SelectedBooksText
        {
            get { return _SelectedBooksText; }
            set { this.RaiseAndSetIfChanged(ref _SelectedBooksText, value); }
        }

        private ReactiveList<ImportedBookViewModel> _Books = new ReactiveList<ImportedBookViewModel>();
        public ReactiveList<ImportedBookViewModel> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private ReactiveList<ShelfMappingViewModel> _ShelfMappings;
        public ReactiveList<ShelfMappingViewModel> ShelfMappings
        {
            get { return _ShelfMappings; }
            set { this.RaiseAndSetIfChanged(ref _ShelfMappings, value); }
        }

        public ObservableAsPropertyHelper<bool> _HaveImportedBooks;
        public bool HaveImportedBooks { get { return _HaveImportedBooks.Value; } }

        private ReactiveCommand _PickFileCommand;
        public ReactiveCommand PickFileCommand
        {
            get { return _PickFileCommand; }
            set { this.RaiseAndSetIfChanged(ref _PickFileCommand, value); }
        }

        private ReactiveCommand _SelectAllCommand;
        public ReactiveCommand SelectAllCommand
        {
            get { return _SelectAllCommand; }
            set { this.RaiseAndSetIfChanged(ref _SelectAllCommand, value); }
        }

        private ReactiveCommand _DeselectAllCommand;
        public ReactiveCommand DeselectAllCommand
        {
            get { return _DeselectAllCommand; }
            set { this.RaiseAndSetIfChanged(ref _DeselectAllCommand, value); }
        }

        private ReactiveCommand _SelectBySimilarityCommand;
        public ReactiveCommand SelectBySimilarityCommand
        {
            get { return _SelectBySimilarityCommand; }
            set { this.RaiseAndSetIfChanged(ref _SelectBySimilarityCommand, value); }
        }

        private ReactiveCommand _CreateImportedShelves;
        public ReactiveCommand CreateImportedShelves
        {
            get { return _CreateImportedShelves; }
            set { this.RaiseAndSetIfChanged(ref _CreateImportedShelves, value); }
        }

        private ReactiveCommand _EditShelfCommand;
        public ReactiveCommand EditShelfCommand
        {
            get { return _EditShelfCommand; }
            set { this.RaiseAndSetIfChanged(ref _EditShelfCommand, value); }
        }

        private ReactiveCommand _AddShelfCommand;
        public ReactiveCommand AddShelfCommand
        {
            get { return _AddShelfCommand; }
            set { this.RaiseAndSetIfChanged(ref _AddShelfCommand, value); }
        }

        private ReactiveCommand _DeleteShelfCommand;
        public ReactiveCommand DeleteShelfCommand
        {
            get { return _DeleteShelfCommand; }
            set { this.RaiseAndSetIfChanged(ref _DeleteShelfCommand, value); }
        }

        private ReactiveCommand _ImportCommand;
        public ReactiveCommand ImportCommand
        {
            get { return _ImportCommand; }
            set { this.RaiseAndSetIfChanged(ref _ImportCommand, value); }
        }

        private ReactiveCommand _CancelCommand;
        public ReactiveCommand CancelCommand
        {
            get { return _CancelCommand; }
            set { this.RaiseAndSetIfChanged(ref _CancelCommand, value); }
        }

        public ImportScreenViewModel(ICollectionsService collections_service, IImportService import_service, IDialogService dialog_service)
        {
            DisplayName = "Import";
            this.collections_service = collections_service;
            this.import_service = import_service;
            this.dialog_service = dialog_service;

            Initialize();
        }

        private void Initialize()
        {
            PickFileCommand = ReactiveCommand.Create(PickFileAsync);

            var have_imported_books = this.WhenAny(x => x.Books, x => x.Value != null && x.Value.Any());
            _HaveImportedBooks = have_imported_books.ToProperty(this, x => x.HaveImportedBooks);

            SelectAllCommand = ReactiveCommand.Create(() => Books.Apply(b => b.Selected = true), have_imported_books);
            DeselectAllCommand = ReactiveCommand.Create(() => Books.Apply(b => b.Selected = false), have_imported_books);
            SelectBySimilarityCommand = ReactiveCommand.Create(() => Books.Apply(b => b.Selected = b.SimilarityScore <= MaximumSimilarity), have_imported_books);

            CreateImportedShelves = ReactiveCommand.Create(CreateShelves, have_imported_books);
            AddShelfCommand = ReactiveCommand.Create(AddShelf);
            DeleteShelfCommand = ReactiveCommand.Create(DeleteShelf);

            var any_selected_books = this.WhenAny(x => x.Books, x => x.Value.Any(b => b.Selected));
            var books_selection_changed = this.WhenAnyObservable(x => x.Books.ItemChanged)
                                              .Select(x => Books.Any(b => b.Selected));
            var have_selected_books = Observable.Merge(any_selected_books, books_selection_changed);

            ImportCommand = ReactiveCommand.Create(Import, have_selected_books);
            CancelCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(ApplicationMessage.ShowBooksScreen));

            have_selected_books.Subscribe(_ => 
            {
                var count = Books.Count(b => b.Selected);
                if (count > 0)
                    SelectedBooksText = $"{count} of {Books.Count} selected";
                else
                    SelectedBooksText = "No books selected";
            });
        }

        public override void Deactivate()
        {
            base.Deactivate();

            Filename = string.Empty;
            Books = new ReactiveList<ImportedBookViewModel> { ChangeTrackingEnabled = true };
            ShelfMappings = new ReactiveList<ShelfMappingViewModel>();
        }

        private void PickFileAsync()
        {
            var ofd = new OpenFileDialog
            {
                Title = "Please select file to import",
                InitialDirectory = Assembly.GetExecutingAssembly().Location,
                DefaultExt = ".csv",
                Filter = "Goodreads CSV files |*.csv"
            };
            var result = ofd.ShowDialog();
            if (result == true)
            {
                Filename = ofd.FileName;
                Process();
            }
        }

        private void Process()
        {
            // Parse the file
            var sw = Stopwatch.StartNew();
            var imported_books = GoodreadsImporter.Import(Filename).ToList();
            sw.Stop();
            logger.Debug($"Importing {imported_books.Count} books took {sw.ElapsedMilliseconds} ms");

            // Calculate the similarity for each book to the ones in the current collection
            sw.Restart();
            imported_books.Apply(import_service.GetSimilarity);
            sw.Stop();
            logger.Debug($"Calculating similarity scores took {sw.ElapsedMilliseconds} ms");

            Books = imported_books.Select(b => new ImportedBookViewModel(b))
                                  .ToReactiveList(true);

            ShelfMappings = imported_books.SelectMany(b => b.Shelves)
                                          .Distinct()
                                          .Select(s => new ShelfMappingViewModel(s, collections_service.Current.Shelves))
                                          .ToReactiveList();

            // Try to map to existing shelves
            sw.Restart();
            ShelfMappings.Apply(s => s.SetCurrent(import_service.Map(s.ImportedShelf)));
            sw.Stop();
            logger.Debug($"Mapping shelves took {sw.ElapsedMilliseconds} ms");
        }

        private void CreateShelves()
        {
            var collection = collections_service.Current;
            foreach (var shelf_mapping in ShelfMappings)
            {
                // Only create a shelf if it is not mapped to the default shelf in the collection
                if (shelf_mapping.SelectedShelf.IsDefault)
                {
                    logger.Debug($"Creating shelf for {shelf_mapping.ImportedShelf}");
                    var shelf = collection.AddShelf(shelf_mapping.ImportedShelf);
                    shelf_mapping.SetCurrent(shelf);
                }
            }
        }

        //private void EditShelf()
        //{
        //    dialog_service.ShowDialogAsync(new EditShelfDialogViewModel(), (result) => 
        //    {
        //        logger.Debug($"Result was: {result}");
        //    });
        //}
        
        private void AddShelf()
        {

        }

        private void DeleteShelf()
        {

        }

        private void Import()
        {
            var books_to_import = Books.Where(b => b.Selected)
                                       .Select(b => b.Obj)
                                       .ToList();
            var shelf_mapping = ShelfMappings.ToDictionary(s => s.ImportedShelf, s => s.SelectedShelf);
            import_service.Import(books_to_import, shelf_mapping);
            MessageBus.Current.SendMessage(ApplicationMessage.ShowBooksScreen);
        }
    }
}
