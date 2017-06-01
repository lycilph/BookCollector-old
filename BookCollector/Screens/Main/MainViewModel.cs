using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using BookCollector.Data;
using BookCollector.Framework.Logging;
using BookCollector.Models;
using BookCollector.Shell;
using BookCollector.ThirdParty.Goodreads;
using CsvHelper.Configuration;
using Microsoft.Win32;
using ReactiveUI;

namespace BookCollector.Screens.Main
{
    public class MainViewModel : ShellScreenBase
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private IBookCollectorModel book_collector_model;

        private IReactiveDerivedList<BookViewModel> _Books;
        public IReactiveDerivedList<BookViewModel> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }
        
        private BookViewModel _SelectedBook;
        public BookViewModel SelectedBook
        {
            get { return _SelectedBook; }
            set { this.RaiseAndSetIfChanged(ref _SelectedBook, value); }
        }

        private ReactiveCommand _ImportCommand;
        public ReactiveCommand ImportCommand
        {
            get { return _ImportCommand; }
            set { this.RaiseAndSetIfChanged(ref _ImportCommand, value); }
        }

        public MainViewModel(IBookCollectorModel book_collector_model, MainToolsViewModel tools_view_model)
        {
            this.book_collector_model = book_collector_model;

            DisplayName = ScreenNames.MainScreenName;
            Tools = tools_view_model;

            ImportCommand = ReactiveCommand.Create(() => Import());
        }

        public override void Activate()
        {
            Books = book_collector_model.CurrentCollection
                                        .Books
                                        .CreateDerivedCollection(b => new BookViewModel(b));

            SelectedBook = Books.FirstOrDefault();
        }

        private void Import()
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
                var configuration = new CsvConfiguration
                {
                    IsHeaderCaseSensitive = false,
                    IgnoreHeaderWhiteSpace = true,
                    TrimFields = true
                };

                using (var sr = new StreamReader(ofd.FileName))
                using (var csv = new TrimmingCsvReader(sr, configuration))
                {
                    var csv_books = csv.GetRecords<GoodreadsCsvBook>().ToList();
                    var books = Mapper.Map<IEnumerable<GoodreadsCsvBook>, IEnumerable<Book>>(csv_books).ToList();
                    book_collector_model.AddToCurrentCollection(books);
                }

                if (Books.Count > 0)
                    SelectedBook = Books.First();

                log.Info($"Imported {Books.Count} books from {ofd.FileName}");
            }
        }
    }
}
