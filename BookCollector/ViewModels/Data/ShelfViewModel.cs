using BookCollector.Data;
using BookCollector.Framework.MVVM;

namespace BookCollector.ViewModels.Data
{
    public class ShelfViewModel : ItemViewModel<Shelf>
    {
        public ShelfViewModel(Shelf obj) : base(obj) { }
    }
}
