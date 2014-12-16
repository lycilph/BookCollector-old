using System.Collections.Generic;
using BookCollector.Services.Import;

namespace BookCollector.Import
{
    public class ImportMessage
    {
        public enum MessageKind { Selection, Results }

        public MessageKind Kind { get; set; }
        public IImportController ImportController { get; set; }
        public List<ImportedBook> ImportedBooks { get; set; }

        public static ImportMessage Select(IImportController import_controller)
        {
            return new ImportMessage {Kind = MessageKind.Selection, ImportController = import_controller};
        }

        public static ImportMessage Results(List<ImportedBook> imported_books)
        {
            return new ImportMessage {Kind = MessageKind.Results, ImportedBooks = imported_books};
        }
    }
}
