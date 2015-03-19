using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace BookCollector.Api.Goodreads
{
    public class AuthorResolver : ValueResolver<GoodreadsCsvBook, List<String>>
    {
        protected override List<string> ResolveCore(GoodreadsCsvBook source)
        {
            var authors = new List<string> { source.Author };
            if (!string.IsNullOrWhiteSpace(source.AdditionalAuthors))
                authors.AddRange(source.AdditionalAuthors.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()));
            return authors;
        }
    }
}
