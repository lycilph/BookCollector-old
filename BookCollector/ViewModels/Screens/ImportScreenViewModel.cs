using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using BookCollector.Domain;
using BookCollector.Domain.Goodreads;
using BookCollector.Framework.Extensions;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.ViewModels.Common;
using Microsoft.Win32;
using ReactiveUI;

namespace BookCollector.ViewModels.Screens
{
    public class ImportScreenViewModel : ScreenBase
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IEventAggregator event_aggregator;
        private IApplicationModel application_model;

        private ReactiveList<ImportedBookViewModel> _Books = new ReactiveList<ImportedBookViewModel>();
        public ReactiveList<ImportedBookViewModel> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private ReactiveList<ImportedShelfViewModel> _Shelves = new ReactiveList<ImportedShelfViewModel>();
        public ReactiveList<ImportedShelfViewModel> Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
        }

        private string _FilenameShort;
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

        private string _ShelvesText;
        public string ShelvesText
        {
            get { return _ShelvesText; }
            set { this.RaiseAndSetIfChanged(ref _ShelvesText, value); }
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

        public ImportScreenViewModel(IApplicationModel application_model, IEventAggregator event_aggregator)
        {
            this.application_model = application_model;
            this.event_aggregator = event_aggregator;
            DisplayName = Constants.ImportScreenDisplayName;

            // Update the short filename, when the full filename is changed
            this.WhenAnyValue(x => x.Filename)
                .Subscribe(f => FilenameShort = Path.GetFileName(Filename));

            var have_books = this.WhenAny(x => x.Books, x => x.Value != null && x.Value.Any());

            var list_changed = this.WhenAny(x => x.Books, x => x.Value != null && x.Value.Any(b => b.Selected));
            var item_changed = this.WhenAnyObservable(x => x.Books.ItemChanged)
                                   .Where(x => x.PropertyName == "Selected")
                                   .Select(x => Books.Any(b => b.Selected));
            var have_selected_books = Observable.Merge(list_changed, item_changed);

            have_selected_books.Subscribe(_ =>
            {
                UpdateSelectedBooksText();
                UpdateSelectedShelves();
            });

            PickFileCommand = ReactiveCommand.Create(PickFile);

            SelectAllCommand = ReactiveCommand.Create(() => Books.Apply(b => b.Selected = true), have_books);
            DeselectAllCommand = ReactiveCommand.Create(() => Books.Apply(b => b.Selected = false), have_books);
            SelectBySimilarityCommand = ReactiveCommand.Create(() => Books.Apply(b => b.Selected = b.Similarity <= Similarity), have_books);

            ImportCommand = ReactiveCommand.Create(Import, have_selected_books);
            CancelCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateTo(Constants.BooksScreenDisplayName)));
        }

        public override void Activate()
        {
            base.Activate();

            Books = new ReactiveList<ImportedBookViewModel>();
            Shelves = new ReactiveList<ImportedShelfViewModel>();
            Filename = string.Empty;
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
            if (!Books.Any())
            {
                ShelvesText = "No shelves";
                return;
            }
            else
                ShelvesText = "New shelves found";

            var selected_shelves = Books.Where(b => b.Selected)
                                        .SelectMany(b => b.Shelves)
                                        .Distinct()
                                        .ToList();
            Shelves.Apply(s => s.Enabled = selected_shelves.Contains(s.Unwrap()));
        }

        private void Import()
        {
            var books_to_import = Books.Where(b => b.Selected)
                                       .Select(b => b.Unwrap())
                                       .ToList();

            application_model.AddToCurrentCollection(books_to_import);
            event_aggregator.Publish(ApplicationMessage.NavigateTo(Constants.BooksScreenDisplayName));
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

                var importer = new GoodreadsImporter(Filename, application_model.CurrentCollection);
                Books = importer.GetViewModels();
                Books.ChangeTrackingEnabled = true;
                Shelves = importer.ImportedShelves.Select(s => new ImportedShelfViewModel(s)).ToReactiveList();
            }
        }
    }
}
