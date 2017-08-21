using BookCollector.Data;
using Core.Utility;

namespace BookCollector.Screens.Collections
{
    public class DescriptionViewModel : ItemViewModel<Description>
    {
        public string Name { get { return Obj.Name; } }
        public string Summary { get { return Obj.Summary; } }

        public DescriptionViewModel(Description obj) : base(obj)
        {
        }
    }
}
