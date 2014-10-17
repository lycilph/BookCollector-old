using System.Reflection;
using BookCollector.Data;
using BookCollector.Goodreads;
using Caliburn.Micro;
using Framework.Dialogs;
using Framework.Mvvm;
using ReactiveUI;
using Action = System.Action;

namespace BookCollector.Main
{
    public class BookDetailsViewModel : ItemViewModelBase<Book>, IHaveCloseAction
    {
        private readonly GoodreadsApi api;

        public string Title { get { return AssociatedObject.Title; } }
        public string Author { get { return AssociatedObject.Author; } }
        public string Image { get { return AssociatedObject.Image; } }
        public string Description { get { return AssociatedObject.Description; } }
        public IReactiveDerivedList<SimilarBookViewModel> SimilarBooks { get; set; }

        public Action CloseCallback { get; set; }

        public BookDetailsViewModel(Book book) : base(book)
        {
            api = IoC.Get<GoodreadsApi>();
            FindSimilarBooks();
        }

        private void FindSimilarBooks()
        {
            AssociatedObject.SimilarBooks.Clear();
            SimilarBooks = AssociatedObject.SimilarBooks.CreateDerivedCollection(sb => new SimilarBookViewModel(sb));

            api.FindSimilarBooks(AssociatedObject);
        }

        public void ShowBook(SimilarBookViewModel similar_book)
        {
            AssociatedObject = similar_book.AssociatedObject;

            api.ClearPriorityQueue();
            FindSimilarBooks();
            
            // Raise property changed events for all public properties
            GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     .Apply(p => this.RaisePropertyChanged(p.Name));
        }

        public void Close()
        {
            api.ClearPriorityQueue();
            CloseCallback();
        }
    }
}
