using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookCollector.Data;
using Caliburn.Micro;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace BookCollector.Services
{
    public class Importer
    {
        private class CustomCsvReader : CsvReader
        {
            public CustomCsvReader(TextReader reader) : base(reader) { }
            public CustomCsvReader(TextReader reader, CsvConfiguration configuration) : base(reader, configuration) { }
            public CustomCsvReader(ICsvParser parser) : base(parser) { }

            public override string GetField(int index)
            {
                var field = base.GetField(index);
                return field.TrimStart('=').TrimStart('=').Replace("\"", "");
            }
        }

        private class ListTypeConverter : ITypeConverter
        {
            public string ConvertToString(TypeConverterOptions options, object value)
            {
                throw new NotImplementedException();
            }

            public object ConvertFromString(TypeConverterOptions options, string text)
            {
                return new List<string> {text};
            }

            public bool CanConvertFrom(Type type)
            {
                return type == typeof (string);
            }

            public bool CanConvertTo(Type type)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class BookMap : CsvClassMap<Book>
        {
            public BookMap()
            {
                Map(m => m.Title);
                Map(m => m.Authors).Name("Author").TypeConverter(new ListTypeConverter());
                Map(m => m.ISBN10).Name("isbn");
                Map(m => m.ISBN13);
            }
        }

        public static List<Book> Read(string filename)
        {
            var configuration = new CsvConfiguration
            {
                IsHeaderCaseSensitive = false,
                TrimFields = true
            };
            configuration.RegisterClassMap(new BookMap());

            using (var sr = new StreamReader(filename))
            using (var csv = new CustomCsvReader(sr, configuration))
            {
                var books = csv.GetRecords<Book>().ToList();
                books.Apply(b => b.ImportSource = "Goodreads CSV");
                return books;
            }
        }
    }
}
