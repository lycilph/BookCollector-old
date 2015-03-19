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

        public IProgress<List<ImportedBook>> Results { get; private set; }

        [ImportingConstructor]
        public GoodreadsCSVImporter(ISettings settings)
        {
            this.settings = settings;
        }

        public Task Execute(IProgress<string> status, IProgress<List<ImportedBook>> results)
        {
            Status = status;
            Results = results;

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

            return Task.FromResult(0);
        }

        private void ImportInternal(string filename)
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
                var books = Mapper.Map<List<GoodreadsCsvBook>, List<ImportedBook>>(csv_books);

                Status.Report(string.Format("Found {0} books", books.Count));
                Results.Report(books);

                //books.Apply(b =>
                //{
                //    b.Source = "Goodreads CSV";
                //    b.History.Add("Imported on " + DateTime.Now.ToShortDateString());
                //});
            }
        }
    }
}
