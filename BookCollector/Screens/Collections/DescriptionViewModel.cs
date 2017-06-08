using BookCollector.Framework.MVVM;
using BookCollector.Models;
using ReactiveUI;

namespace BookCollector.Screens.Collections
{
    public class DescriptionViewModel : ItemViewModel<Description>
    {
        public string Name
        {
            get { return obj.Name; }
            set
            {
                if (value != obj.Name)
                {
                    this.RaisePropertyChanging();
                    obj.Name = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string Text
        {
            get { return obj.Text; }
            set
            {
                if (value != obj.Text)
                {
                    this.RaisePropertyChanging();
                    obj.Text = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string LastModified { get { return obj.LastModfied.ToShortDateString(); } }
        public string Filename { get { return obj.Filename; } }

        public DescriptionViewModel(Description obj) : base(obj)
        {
        }
    }
}
