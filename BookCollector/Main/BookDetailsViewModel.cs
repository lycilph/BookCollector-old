using System.Threading;
using System.Threading.Tasks;
using BookCollector.Data;
using BookCollector.Goodreads;
using Caliburn.Micro;
using Framework.Dialogs;
using Framework.Mvvm;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;
using Action = System.Action;

namespace BookCollector.Main
{
    public class BookDetailsViewModel : ItemViewModelBase<Book>, IHaveCloseAction
    {
        private CancellationToken token;

        public string Title { get { return AssociatedObject.Title; } }
        public string Author { get { return AssociatedObject.Author; } }
        public string Image { get { return AssociatedObject.Image; } }
        public string Description { get { return AssociatedObject.Description; } }
        public IReactiveDerivedList<SimilarBookViewModel> SimilarBooks { get; set; }

        public Task<MessageDialogResult> Task { get; set; }
        public Action CloseCallback { get; set; }

        public BookDetailsViewModel(Book book) : base(book)
        {
            if (book.SimilarBooks.IsEmpty)
            {
                var api = IoC.Get<GoodreadsApi>();
                api.PriorityQueue.Enqueue(AssociatedObject);    
            }

            SimilarBooks = AssociatedObject.SimilarBooks.CreateDerivedCollection(sb => new SimilarBookViewModel(sb));
        }

        public async void ShowBook(SimilarBookViewModel similar_book)
        {
            CloseCallback();
            await Task;
            var vm = new BookDetailsViewModel(similar_book.AssociatedObject);
            await DialogController.ShowAsync(vm, DialogButtonOptions.None);
        }
    }
}
