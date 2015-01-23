using System.Collections.Generic;
using BookCollector.Apis;
using BookCollector.Model;

namespace BookCollector.Screens.Import
{
    public interface IImportProcessController
    {
        void Cancel();
        void SelectController(IImportController import_controller);
        void UpdateProgress(string message);
        void ShowResults(List<ImportedBook> books);
        void ImportBooks(List<ImportedBook> books);
    }
}
