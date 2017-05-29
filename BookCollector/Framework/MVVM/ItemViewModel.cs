using ReactiveUI;

namespace BookCollector.Framework.MVVM
{
    public class ItemViewModel<T> : ReactiveObject
    {
        protected readonly T obj;

        public ItemViewModel(T obj)
        {
            this.obj = obj;
        }

        public bool Matches(T obj)
        {
            return this.obj.Equals(obj);
        }

        public virtual T Unwrap()
        {
            return obj;
        }
    }
}
