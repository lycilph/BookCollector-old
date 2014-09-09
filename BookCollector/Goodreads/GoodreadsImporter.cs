using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookCollector.Data;
using BookCollector.Utils;
using Caliburn.Micro;
using Microsoft.Win32;

namespace BookCollector.Goodreads
{
    public static class GoodreadsImporter
    {
        public static List<Book> Import()
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".csv",
                Filter = "csv files (.csv)|*.csv"
            };

            return dialog.ShowDialog() == true ? Parse(dialog.FileName) : new List<Book>();
        }

        private static List<Book> Parse(string filename)
        {
            var csv = new TrimmingCsvReader(new StreamReader(filename), new[] { '=', '\"' });
            var imported_books = csv.GetRecords<GoodreadImportedBook>().ToList();

            var books = imported_books.Select(gb => Mapper.MapPublicProperties(gb, new Book())).ToList();
            books.Apply(b => b.Status = BookStatus.Ready);

            return books;
        }
    }
}
