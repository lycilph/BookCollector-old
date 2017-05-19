using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using BookCollector.Data;
using BookCollector.Extensions;
using BookCollector.ThirdParty.Goodreads;
using Caliburn.Micro;
using CsvHelper.Configuration;
using Microsoft.Win32;
using ReactiveUI;

namespace BookCollector.Screens.Main
{
    public class MainViewModel : ReactiveObject
    {
        private ILog log = LogManager.GetLog(typeof(MainViewModel));
        private IDataController data_controller;

        private ReactiveList<BookViewModel> _Books = new ReactiveList<BookViewModel>();
        public ReactiveList<BookViewModel> Books
        {
            get { return _Books; }
            private set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private BookViewModel _SelectedBook;
        public BookViewModel SelectedBook
        {
            get { return _SelectedBook; }
            set { this.RaiseAndSetIfChanged(ref _SelectedBook, value); }
        }

        public MainViewModel(IDataController data_controller)
        {
            this.data_controller = data_controller;
        }

        public void Import()
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
                    var books = Mapper.Map<IEnumerable<GoodreadsCsvBook>, IEnumerable<Book>>(csv_books);
                    Books = books.Select(x => new BookViewModel(x)).ToReactiveList();
                }

                if (Books.Count > 0)
                    SelectedBook = Books.First();

                log.Info($"Imported {Books.Count} books from {ofd.FileName}");
            }
        }
    }
}
