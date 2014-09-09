using BookCollector.Data;
using Caliburn.Micro;
using Framework.Mvvm;

namespace BookCollector.Start
{
    public class InfoViewModel : ItemViewModelBase<Info>
    {
        public string DisplayName { get { return AssociatedObject.DisplayName; } }
        public string Text { get { return AssociatedObject.Text; } }
        public string Updated { get { return AssociatedObject.LastUpdated.ToShortDateString(); } }

        public InfoViewModel(Info info) : base(info) { }

        public void Remove()
        {
            var repository = IoC.Get<InfoRepository>();
            repository.Remove(AssociatedObject);
        }
    }
}
