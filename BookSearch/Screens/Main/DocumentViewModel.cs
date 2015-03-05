using BookSearch.Services.Scoring;
using Panda.ApplicationCore.MVVM;

namespace BookSearch.Screens.Main
{
    public class DocumentViewModel : ItemViewModelBase<Document>
    {
        public string Title { get { return AssociatedObject.Book.Title; } }
        public string Author { get { return AssociatedObject.Book.Author; } }
        public string Source { get { return AssociatedObject.Book.Source; } }
        public double Score { get { return AssociatedObject.Score; } }

        public DocumentViewModel(Document obj) : base(obj)
        {
        }
    }
}
