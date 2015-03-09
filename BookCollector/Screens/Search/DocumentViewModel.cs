using BookCollector.Services.DocumentScoring;
using Panda.ApplicationCore.MVVM;

namespace BookCollector.Screens.Search
{
    public class DocumentViewModel : ItemViewModelBase<Document>
    {
        public string Title { get { return AssociatedObject.Book.Title; } }
        public string Author { get { return string.Join(", ", AssociatedObject.Book.Authors); } }
        public string Source { get { return AssociatedObject.Book.Source; } }
        public double Score { get { return AssociatedObject.Score; } }

        public DocumentViewModel(Document obj) : base(obj)
        {
        }
    }
}
