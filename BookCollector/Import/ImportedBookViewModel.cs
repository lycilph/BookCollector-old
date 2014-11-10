using BookCollector.Model;
using Framework.Core.MVVM;

namespace BookCollector.Import
{
    public class ImportedBookViewModel : ItemViewModelBase<Book>
    {
        public string Title { get { return AssociatedObject.Title; } }

        public ImportedBookViewModel(Book obj) : base(obj)
        {
        }
    }
}
