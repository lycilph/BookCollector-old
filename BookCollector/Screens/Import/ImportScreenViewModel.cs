using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using BookCollector.Services;
using BookCollector.ThirdParty.Goodreads;
using Core;
using Core.Extensions;
using Microsoft.Win32;
using NLog;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    public class ImportScreenViewModel : ScreenBase, IImportScreen
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IImportService import_service;

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

        public ImportScreenViewModel(IImportService import_service)
        {
            DisplayName = "Import";
            this.import_service = import_service;

            Initialize();
        }

        private void Initialize()
        {
            PickFileCommand = ReactiveCommand.Create(PickFile);

            var have_imported_books = this.WhenAny(x => x.Books, x => x.Value != null && x.Value.Any());
            _HaveImportedBooks = have_imported_books.ToProperty(this, x => x.HaveImportedBooks);

            SelectAllCommand = ReactiveCommand.Create(() => Books.Apply(b => b.Selected = true), have_imported_books);
            DeselectAllCommand = ReactiveCommand.Create(() => Books.Apply(b => b.Selected = false), have_imported_books);
            SelectBySimilarityCommand = ReactiveCommand.Create(() => Books.Apply(b => b.Selected = b.SimilarityScore <= MaximumSimilarity), have_imported_books);

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
                Process();
            }
        }

        private void Process()
        {
            var sw = Stopwatch.StartNew();
            Books = GoodreadsImporter.Import(Filename)
                                     .Select(b => new ImportedBookViewModel(b))
                                     .ToReactiveList(true);
            sw.Stop();
            logger.Debug($"Importing {Books.Count} books took {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            Books.Apply(b => import_service.GetSimilarity(b.Obj));
            sw.Stop();
            logger.Debug($"Calculating similarity scores took {sw.ElapsedMilliseconds} ms");
        }

        private void Import()
        {
            var books_to_import = Books.Where(b => b.Selected)
                                       .Select(b => b.Obj)
                                       .ToList();
            import_service.Import(books_to_import);
            MessageBus.Current.SendMessage(ApplicationMessage.ShowBooksScreen);
        }
    }
}
