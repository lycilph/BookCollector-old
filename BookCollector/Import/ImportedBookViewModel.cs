using BookCollector.Services.Import;
using Framework.Core.MVVM;
using ReactiveUI;

namespace BookCollector.Import
{
    public class ImportedBookViewModel : ItemViewModelBase<ImportedBook>
    {
        public string Title { get { return AssociatedObject.Book.Title; } }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { this.RaiseAndSetIfChanged(ref _IsSelected, value); }
        }

        public ImportedBookViewModel(ImportedBook obj) : base(obj)
        {
        }
    }
}
