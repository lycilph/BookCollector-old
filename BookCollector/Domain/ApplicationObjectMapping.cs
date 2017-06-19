using System;
using System.Linq;
using BookCollector.Data;
using BookCollector.Domain.Goodreads;
using BookCollector.Framework.Logging;
using BookCollector.Framework.Mapping;

namespace BookCollector.Domain
{
    public class ApplicationObjectMapping
    {
        public static void Setup()
        {
            var log = LogManager.GetCurrentClassLogger();
            log.Info("Configuring Mapper");

            Mapper.Add<GoodreadsBook, Book>((source, destination) =>
            {
                destination.ISBN10 = source.ISBN;

                // Add author
                destination.Authors.Add(source.Author);
                // Add additional authors
                if (!string.IsNullOrWhiteSpace(source.AdditionalAuthors))
                    destination.Authors.AddRange(source.AdditionalAuthors.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()));
            });
        }
    }
}
