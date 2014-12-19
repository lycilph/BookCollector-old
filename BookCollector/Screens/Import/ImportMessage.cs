using System.Collections.Generic;
using BookCollector.Services.Repository;

namespace BookCollector.Screens.Import
{
    public class ImportMessage
    {
        public enum MessageKind { Selection, Information, Results }

        public MessageKind Kind { get; set; }
        public IImportController ImportController { get; set; }
        public List<ImportedBook> ImportedBooks { get; set; }
        public string Text { get; set; }

        public static ImportMessage Select(IImportController import_controller)
        {
            return new ImportMessage { Kind = MessageKind.Selection, ImportController = import_controller };
        }

        public static ImportMessage Information(string text)
        {
            return new ImportMessage { Kind = MessageKind.Information, Text = text };
        }

        public static ImportMessage Results(List<ImportedBook> imported_books)
        {
            return new ImportMessage { Kind = MessageKind.Results, ImportedBooks = imported_books };
        }
    }
}
