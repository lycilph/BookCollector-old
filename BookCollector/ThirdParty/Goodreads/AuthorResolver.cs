using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BookCollector.Data;

namespace BookCollector.ThirdParty.Goodreads
{
    public class AuthorResolver : IValueResolver<GoodreadsCsvBook, Book, List<String>>
    {
        public List<string> Resolve(GoodreadsCsvBook source, Book destination, List<string> destMember, ResolutionContext context)
        {
            var authors = new List<string> { source.Author };
            if (!string.IsNullOrWhiteSpace(source.AdditionalAuthors))
                authors.AddRange(source.AdditionalAuthors.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()));
            return authors;
        }
    }
}
