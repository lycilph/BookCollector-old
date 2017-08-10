using ReactiveUI;

namespace Core.Utility
{
    public class ItemViewModel<T> : ReactiveObject
    {
        public T Obj { get; protected set; }

        public ItemViewModel(T obj)
        {
            Obj = obj;
        }
    }
}
