using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using BookCollector.Domain;
using BookCollector.Domain.ThirdParty.Goodreads;
using BookCollector.Framework.Extensions;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Mapping;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.Models;
using CsvHelper.Configuration;
using Microsoft.Win32;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    public class ImportViewModel : MainScreenBase
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

        private ReactiveCommand _SelectFileCommand;
        public ReactiveCommand SelectFileCommand
        {
            get { return _SelectFileCommand; }
            set { this.RaiseAndSetIfChanged(ref _SelectFileCommand, value); }
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

        public ImportViewModel(IEventAggregator event_aggregator, IApplicationModel application_model)
        {
            this.event_aggregator = event_aggregator;
            this.application_model = application_model;
            DisplayName = ScreenNames.ImportName;

            var have_books = this.WhenAny(x => x.Books, x => x.Value != null && x.Value.Any());

            var list_changed = this.WhenAny(x => x.Books, x => x.Value != null && x.Value.Any(b => b.Selected));
            var item_changed = this.WhenAnyObservable(x => x.Books.ItemChanged)
                                          .Where(x => x.PropertyName == "Selected")
                                          .Select(x => Books.Any(b => b.Selected));
            var have_selected_books = Observable.Merge(list_changed, item_changed);

            have_selected_books.Subscribe(_ => UpdateSelectedBooksText());

            SelectFileCommand = ReactiveCommand.Create(SelectFile);
            SelectAllCommand = ReactiveCommand.Create(() => Books.Apply(b => b.Selected = true), have_books);
            DeselectAllCommand = ReactiveCommand.Create(() => Books.Apply(b => b.Selected = false), have_books);
            SelectBySimilarityCommand = ReactiveCommand.Create(() => Books.Apply(b => b.Selected = b.Similarity <= Similarity), have_books);
            ImportCommand = ReactiveCommand.Create(Import, have_selected_books);
            CancelCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateToMessage(ScreenNames.BooksName)));
        }

        public override void Deactivate()
        {
            Filename = string.Empty;
            Books = null;
        }

        private void UpdateSelectedBooksText()
        {
            if (Books == null)
            {
                SelectedBooksText = "No books selected";
                return;
            }

            var count = Books.Count;
            var selected_count = Books.Count(b => b.Selected);
            if (selected_count > 0)
                SelectedBooksText = $"{selected_count} books of {count} selected";
            else
                SelectedBooksText = "No books selected";
        }

        private void Import()
        {
            var books_to_import = Books.Where(b => b.Selected)
                                       .Select(b => b.Unwrap())
                                       .ToList();

            application_model.AddToCurrentCollection(books_to_import);
            event_aggregator.Publish(ApplicationMessage.NavigateToMessage(ScreenNames.BooksName));
        }

        private void SelectFile()
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
                CalculateSimilarity();
            }
        }

        private void ParseFile()
        {
            log.Info($"Parsing file \"{Filename}\" books");

            var configuration = new CsvConfiguration
            {
                IsHeaderCaseSensitive = false,
                IgnoreHeaderWhiteSpace = true,
                TrimFields = true
            };

            using (var sr = new StreamReader(Filename))
            using (var csv = new TrimmingCsvReader(sr, configuration))
            {
                Books = csv.GetRecords<GoodreadsCsvBook>()
                            .Select(b => Mapper.Map<Book>(b))
                            .Select(b => new ImportedBookViewModel(b))
                            .ToReactiveList();
                Books.ChangeTrackingEnabled = true;
            }

            log.Info($"Found {Books.Count} books in {Filename}");
        }

        private void CalculateSimilarity()
        {
            var books_in_collection = application_model.CurrentCollection.Books;

            foreach (var book in Books)
            {
                var tmp = book.Unwrap();
                var duplicates = books_in_collection.Where(b => tmp.Title.Equals(b.Title) ||
                                                                tmp.Authors.SequenceEqual(b.Authors) ||
                                                                tmp.ISBN10.Equals(b.ISBN10) ||
                                                                tmp.ISBN13.Equals(b.ISBN13))
                                                    .Select(b =>
                                                    {
                                                        var score = 0;
                                                        var text_short = string.Empty;
                                                        var text_full = string.Empty;
                                                        if (tmp.Title.Equals(b.Title))
                                                        {
                                                            score += 25;
                                                            text_short = "T";
                                                            text_full = "Title";
                                                        }
                                                        if (tmp.Authors.SequenceEqual(b.Authors))
                                                        {
                                                            score += 25;
                                                            text_short += ", A";
                                                            text_full += ", Authors";
                                                        }
                                                        if (!string.IsNullOrWhiteSpace(tmp.ISBN10) && tmp.ISBN10.Equals(b.ISBN10))
                                                        {
                                                            score += 25;
                                                            text_short += ", 10";
                                                            text_full += ", ISBN10";
                                                        }
                                                        if (!string.IsNullOrWhiteSpace(tmp.ISBN13) && tmp.ISBN13.Equals(b.ISBN13))
                                                        {
                                                            score += 25;
                                                            text_short += ", 13";
                                                            text_full += ", ISBN13";
                                                        }

                                                        text_short = text_short.TrimStart(new char[] { ',', ' ' });
                                                        text_full = text_full.TrimStart(new char[] { ',', ' ' });

                                                        return new { b, score, text_short, text_full };
                                                    })
                                                    .OrderByDescending(p => p.score);

                if (duplicates.Any())
                {
                    book.Similarity = duplicates.First().score;
                    book.SimilarityTextShort = duplicates.First().text_short;
                    book.SimilarityTextFull = duplicates.First().text_full;
                }
            }
        }
    }
}
