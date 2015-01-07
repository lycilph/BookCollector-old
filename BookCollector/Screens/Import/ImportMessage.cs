﻿using System.Collections.Generic;
using BookCollector.Apis;
using BookCollector.Model;

namespace BookCollector.Screens.Import
{
    public class ImportMessage
    {
        public enum MessageKind { Select, Information, Results }

        public MessageKind Kind { get; set; }
        public IImportController ImportController { get; set; }
        public List<ImportedBook> ImportedBooks { get; set; }
        public string Text { get; set; }

        public static ImportMessage Select(IImportController import_controller)
        {
            return new ImportMessage { Kind = MessageKind.Select, ImportController = import_controller };
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
