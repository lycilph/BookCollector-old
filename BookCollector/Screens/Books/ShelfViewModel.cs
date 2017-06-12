using BookCollector.Data;
using BookCollector.Framework.MVVM;

namespace BookCollector.Screens.Books
{
    public class ShelfViewModel : ItemViewModel<Shelf>
    {
        public string Name { get { return obj.Name; } }

        public ShelfViewModel(Shelf obj) : base(obj) { }
    }
}
