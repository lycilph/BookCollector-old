using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookCollector.Api.Goodreads;
using BookCollector.Services;
using CsvHelper.Configuration;
using Microsoft.Win32;

namespace BookCollector.Api.ImportProvider
{
    [Export(typeof(IImportProvider))]
    public class GoodreadsCSVImporter : IImportProvider
    {
        private readonly ISettings settings;

        public string Name { get { return "Goodreads CSV"; } }

        public IProgress<string> Status { get; private set; }

        [ImportingConstructor]
        public GoodreadsCSVImporter(ISettings settings)
        {
            this.settings = settings;
        }

        public Task<List<ImportedBook>> Execute(IProgress<string> status)
        {
            Status = status;

            var ofd = new OpenFileDialog
            {
                Title = "Please select file to import",
                InitialDirectory = settings.DataFolder,
                DefaultExt = ".csv",
                Filter = "Goodreads CSV files |*.csv"
            };
            var result = ofd.ShowDialog();
            if (result == true)
                return Task.Factory.StartNew(() => ImportInternal(ofd.FileName));

            return Task.FromResult(new List<ImportedBook>());
        }

        private List<ImportedBook> ImportInternal(string filename)
        {
            Status.Report("Starting import of " + filename);

            var configuration = new CsvConfiguration
            {
                IsHeaderCaseSensitive = false,
                IgnoreHeaderWhiteSpace = true,
                TrimFields = true
            };

            using (var sr = new StreamReader(filename))
            using (var csv = new TrimmingCsvReader(sr, configuration))
            {
                var csv_books = csv.GetRecords<GoodreadsCsvBook>().ToList();
                var books = Mapper.Map<List<ImportedBook>>(csv_books);

                Status.Report(string.Format("Found {0} books", books.Count));
                return books;
            }
        }
    }
}
