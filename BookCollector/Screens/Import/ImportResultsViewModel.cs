using System;
using System.Collections.Generic;
using System.Linq;
using BookCollector.Utilities;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;

namespace BookCollector.Screens.Import
{
    public class ImportResultsViewModel : ReactiveScreen
    {
        private readonly IImportProcessController import_process_controller;

        private ReactiveList<ImportedBookViewModel> _Books = new ReactiveList<ImportedBookViewModel>();
        public ReactiveList<ImportedBookViewModel> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private bool _IsAllSelected;
        public bool IsAllSelected
        {
            get { return _IsAllSelected; }
            set { this.RaiseAndSetIfChanged(ref _IsAllSelected, value); }
        }

        public ImportResultsViewModel(IImportProcessController import_process_controller)
        {
            this.import_process_controller = import_process_controller;

            this.WhenAnyValue(x => x.IsAllSelected)
                .Subscribe(selected => Books.Apply(b => b.IsSelected = selected));
        }

        public void Ok()
        {
            var books_to_import = Books.Where(b => b.IsSelected).Select(b => b.AssociatedObject).ToList();
            import_process_controller.ImportBooks(books_to_import);
        }

        public void Cancel()
        {
            import_process_controller.Cancel();
        }

        public void Update(List<ImportedBookViewModel> view_models)
        {
            Books.Clear();
            IsAllSelected = false;

            Books = view_models.ToReactiveList();
            Books.Apply(b => b.IsSelected = !b.IsDuplicate);
            IsAllSelected = Books.All(b => b.IsSelected);
        }
    }
}
