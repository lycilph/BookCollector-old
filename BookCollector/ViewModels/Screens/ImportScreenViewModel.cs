using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using BookCollector.Domain;
using BookCollector.Framework.Extensions;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.Models;
using BookCollector.ViewModels.Data;
using Microsoft.Win32;
using ReactiveUI;

namespace BookCollector.ViewModels.Screens
{
    public class ImportScreenViewModel : ScreenBase
    {
        private IEventAggregator event_aggregator;
        private ICollectionModel collection_model;
        private IImporter importer;

        private string _FilenameShort = string.Empty;
        public string FilenameShort
        {
            get { return _FilenameShort; }
            set { this.RaiseAndSetIfChanged(ref _FilenameShort, value); }
        }

        private string _Filename = string.Empty;
        public string Filename
        {
            get { return _Filename; }
            set { this.RaiseAndSetIfChanged(ref _Filename, value); }
        }

        private int _Similarity = 25;
        public int Similarity
        {
            get { return _Similarity; }
            set { this.RaiseAndSetIfChanged(ref _Similarity, value); }
        }

        private string _SelectedBooksText;
        public string SelectedBooksText
        {
            get { return _SelectedBooksText; }
            set { this.RaiseAndSetIfChanged(ref _SelectedBooksText, value); }
        }

        private ReactiveList<ImportBookViewModel> _Books = new ReactiveList<ImportBookViewModel>();
        public ReactiveList<ImportBookViewModel> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private ReactiveList<ImportShelfViewModel> _Shelves = new ReactiveList<ImportShelfViewModel>();
        public ReactiveList<ImportShelfViewModel> Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
        }

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

        public ImportScreenViewModel(IEventAggregator event_aggregator, ICollectionModel collection_model, IImporter importer)
        {
            DisplayName = Constants.ImportScreenDisplayName;
            this.event_aggregator = event_aggregator;
            this.collection_model = collection_model;
            this.importer = importer;

            PickFileCommand = ReactiveCommand.Create(PickFile);

            var have_books = this.WhenAny(x => x.Books, x => x.Value != null && x.Value.Any());
            SelectAllCommand = ReactiveCommand.Create(() => Books.Apply(b => b.Selected = true), have_books);
            DeselectAllCommand = ReactiveCommand.Create(() => Books.Apply(b => b.Selected = false), have_books);
            SelectBySimilarityCommand = ReactiveCommand.Create(() => Books.Apply(b => b.Selected = b.SimilarityScore <= Similarity), have_books);

            var have_selected_books = Observable.Merge(this.WhenAny(x => x.Books, x => x.Value.Any(b => b.Selected)),
                                                       this.WhenAnyObservable(x => x.Books.ItemChanged)
                                                           .Where(x => x.PropertyName == "Selected")
                                                           .Select(x => Books.Any(b => b.Selected)));
            ImportCommand = ReactiveCommand.Create(Import, have_selected_books);
            CancelCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateTo(Constants.BooksScreenDisplayName)));

            have_selected_books.Subscribe(_ =>
            {
                UpdateSelectedBooksText();
                UpdateSelectedShelves();
            });

            // Update the short filename, when the full filename is changed
            this.WhenAnyValue(x => x.Filename)
                .Subscribe(f => FilenameShort = Path.GetFileName(Filename));
        }

        public override void Activate()
        {
            base.Activate();

            Filename = string.Empty;
            Books = new ReactiveList<ImportBookViewModel>();
            Shelves = new ReactiveList<ImportShelfViewModel>();
        }

        private void UpdateSelectedBooksText()
        {
            var count = Books.Count;
            var selected_count = Books.Count(b => b.Selected);
            if (selected_count > 0)
                SelectedBooksText = $"{selected_count} books of {count} selected";
            else
                SelectedBooksText = "No books selected";
        }

        private void UpdateSelectedShelves()
        {
            var selected_shelves = Books.Where(b => b.Selected)
                                        .SelectMany(b => b.Obj.Shelves)
                                        .Distinct()
                                        .ToList();
            Shelves.Apply(s => s.Enabled = selected_shelves.Contains(s.Obj));
        }

        private void PickFile()
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
                ParseFile();
            }
        }

        private void ParseFile()
        {
            var imported_books = importer.Import(Filename);

            var similarities = collection_model.CalculateBookSimilarities(imported_books, collection_model.CurrentCollection.Books);
            Books = similarities.Select(s => new ImportBookViewModel(s.Book)
            {
                SimilarityScore = s.Score,
                SimilarityText = s.Text,
                SimilarityTextShort = s.TextShort
            })
            .ToReactiveList(true);

            Shelves = Books.SelectMany(b => b.Obj.Shelves)
                           .Distinct()
                           .Select(s => new ImportShelfViewModel(s))
                           .ToReactiveList();
        }

        private void Import()
        {
            var books_to_import = Books.Where(b => b.Selected)
                                       .Select(b => b.Obj)
                                       .ToList();
            collection_model.Import(books_to_import);
            event_aggregator.Publish(ApplicationMessage.NavigateTo(Constants.BooksScreenDisplayName));
        }
    }
}
