using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using BookCollector.Domain;
using BookCollector.Domain.ThirdParty.Goodreads;
using BookCollector.Framework.Extensions;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Messaging;
using BookCollector.Framework.MVVM;
using BookCollector.Models;
using CsvHelper.Configuration;
using Microsoft.Win32;
using ReactiveUI;

namespace BookCollector.Screens.Books
{
    public class BooksViewModel : MainScreenBase
    {
        private ILog log = LogManager.GetCurrentClassLogger();

        private ReactiveList<BookViewModel> _Books;
        public ReactiveList<BookViewModel> Books
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

        private ReactiveCommand _WebCommand;
        public ReactiveCommand WebCommand
        {
            get { return _WebCommand; }
            set { this.RaiseAndSetIfChanged(ref _WebCommand, value); }
        }

        private ReactiveCommand _ImportCommand;
        public ReactiveCommand ImportCommand
        {
            get { return _ImportCommand; }
            set { this.RaiseAndSetIfChanged(ref _ImportCommand, value); }
        }

        private ReactiveCommand _ChangeCollectionCommand;
        public ReactiveCommand ChangeCollectionCommand
        {
            get { return _ChangeCollectionCommand; }
            set { this.RaiseAndSetIfChanged(ref _ChangeCollectionCommand, value); }
        }

        public BooksViewModel(IEventAggregator event_aggregator, SearchViewModel search_view_model, MenuViewModel menu_view_model)
        {
            DisplayName = ScreenNames.BooksName;
            ShowCollectionCommand = true;
            ExtraContent = search_view_model;
            MenuContent = menu_view_model;

            WebCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateToMessage(ScreenNames.WebName)));
            ChangeCollectionCommand = ReactiveCommand.Create(() => event_aggregator.Publish(ApplicationMessage.NavigateToMessage(ScreenNames.CollectionsName)));
            ImportCommand = ReactiveCommand.Create(Import);
        }

        private void Import()
        {
            log.Info("Importing books");

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
                    var books = Mapper.Map<IEnumerable<GoodreadsCsvBook>, IEnumerable<Book>>(csv_books).ToReactiveList();
                    Books = books.Select(b => new BookViewModel(b)).ToReactiveList();
                }

                if (Books.Count > 0)
                    SelectedBook = Books.First();

                log.Info($"Imported {Books.Count} books from {ofd.FileName}");
            }
        }
    }
}
