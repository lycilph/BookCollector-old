using BookCollector.Model;
using Omu.ValueInjecter;

namespace BookCollector.Services.Goodreads
{
    public class GoodreadsBook
    {
        public string Title { get; set; }
        public string Isbn { get; set; }
        public string Isbn13 { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Link { get; set; }

        public Book ToBook()
        {
            var book = new Book();
            book.InjectFrom(this);
            return book;
        }
    }
}
