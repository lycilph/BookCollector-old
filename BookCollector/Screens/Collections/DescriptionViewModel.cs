using BookCollector.Framework.MVVM;
using BookCollector.Models;

namespace BookCollector.Screens.Collections
{
    public class DescriptionViewModel : ItemViewModel<Description>
    {
        public string Name { get { return obj.Name; } }
        public string Text { get { return obj.Text; } }
        public string LastModified { get { return obj.LastModfied.ToShortDateString(); } }
        public string Filename { get { return obj.Filename; } }

        public DescriptionViewModel(Description obj) : base(obj)
        {
        }
    }
}
