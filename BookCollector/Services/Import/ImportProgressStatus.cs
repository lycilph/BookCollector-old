using System.Collections.Generic;
using BookCollector.Model;

namespace BookCollector.Services.Import
{
    public class ImportProgressStatus
    {
        public string Message { get; set; }
        public IEnumerable<Book> Books { get; set; }

        public ImportProgressStatus(string message) : this(message, null) { }
        public ImportProgressStatus(string message, IEnumerable<Book> books)
        {
            Message = message;
            Books = books;
        }
    }
}
