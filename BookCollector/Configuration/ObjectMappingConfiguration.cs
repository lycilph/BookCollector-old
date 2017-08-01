using System;
using System.Collections.Generic;
using System.Linq;
using BookCollector.Data.Import;
using BookCollector.ThirdParty.Goodreads;
using Core.Mapping;
using NLog;

namespace BookCollector.Configuration
{
    public class ObjectMappingConfiguration
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static void Setup()
        {
            logger.Trace("Adding object mapping between GoodreadsBook and ImportedBook");
            Mapper.Add<GoodreadsBook, ImportedBook>((source, destination) =>
            {
                //ISBN
                destination.ISBN10 = source.ISBN;

                // Handle authors
                destination.Authors = new List<string> { source.Author };
                if (!string.IsNullOrWhiteSpace(source.AdditionalAuthors))
                    destination.Authors.AddRange(source.AdditionalAuthors.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()));

                // Handle shelves
                destination.Shelves = source.Bookshelves.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                      .Select(a => a.Trim())
                                      .Concat(new List<string> { source.ExclusiveShelf })
                                      .Distinct()
                                      .ToList();
            });
        }
    }
}
