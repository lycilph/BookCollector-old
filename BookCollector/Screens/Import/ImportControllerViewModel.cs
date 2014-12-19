using Caliburn.Micro;
using Framework.Core.MVVM;

namespace BookCollector.Screens.Import
{
    public class ImportControllerViewModel : ItemViewModelBase<IImportController>
    {
        public string DisplayName { get { return AssociatedObject.Name; } }

        public ImportControllerViewModel(IImportController obj) : base(obj)
        {
        }

        public void Select()
        {
            var event_aggregator = IoC.Get<IEventAggregator>();
            event_aggregator.PublishOnUIThread(ImportMessage.Select(AssociatedObject));
        }
    }
}
