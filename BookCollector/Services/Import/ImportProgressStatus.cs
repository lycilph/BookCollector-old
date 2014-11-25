using System.Collections.Generic;

namespace BookCollector.Services.Import
{
    public class ImportProgressStatus
    {
        public string Message { get; set; }
        public IEnumerable<ImportedBook> Books { get; set; }

        public ImportProgressStatus(string message) : this(message, null) { }
        public ImportProgressStatus(string message, IEnumerable<ImportedBook> books)
        {
            Message = message;
            Books = books;
        }
    }
}
