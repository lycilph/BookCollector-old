﻿using System;
using System.Linq;
using BookCollector.Domain.ThirdParty.Goodreads;
using BookCollector.Framework.Mapping;
using BookCollector.Models;

namespace BookCollector.Domain.Configuration
{
    public class ApplicationObjectMapping
    {
        public static void Setup()
        {
            Mapper.Add<GoodreadsCsvBook, Book>((source, destination) =>
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