using System;
using System.Linq;

namespace ObjectMapper
{
    class Program
    {
        static void Main(string[] args)
        {
            var goodreads_book = new GoodreadsCsvBook()
            {
                Title = "Goodreads book",
                Author = "Author 1",
                AdditionalAuthors = "Author 2, Author 3",
                ISBN = "1234",
                ISBN13 = "5678"
            };

            var mapper = new MapperInstance();
            mapper.Add<GoodreadsCsvBook, Book>((source, destination) => 
            {
                destination.ISBN10 = source.ISBN;

                // Add author
                destination.Authors.Add(source.Author);
                // Add additional authors
                if (!string.IsNullOrWhiteSpace(source.AdditionalAuthors))
                    destination.Authors.AddRange(source.AdditionalAuthors.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()));
            });

            var book1 = mapper.Map<Book>(goodreads_book);

            var book2 = new Book();
            mapper.Map(goodreads_book, book2);
        }
    }
}
