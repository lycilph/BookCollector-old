using BookCollector.Data;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.ViewModels.Common
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

        public string Details { get; set; }
        public string LastModifiedDate { get { return obj.LastModifiedDate.ToShortDateString(); } }
        public string Filename { get { return obj.Filename; } }

        public DescriptionViewModel(Description obj) : base(obj)
        {
            Details = $"Collection consists of {obj.BooksCount} books in {obj.ShelfCount} shelves";
        }
    }
}
