using BookCollector.Data;
using BookCollector.Framework.MVVM;
using ReactiveUI;

namespace BookCollector.ViewModels.Data
{
    public class DescriptionViewModel : ItemViewModel<Description>
    {
        public string Name
        {
            get { return Obj.Name; }
            set
            {
                if (value != Obj.Name)
                {
                    this.RaisePropertyChanging();
                    Obj.Name = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string Text
        {
            get { return Obj.Text; }
            set
            {
                if (value != Obj.Text)
                {
                    this.RaisePropertyChanging();
                    Obj.Text = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string Details { get; set; }
        public string LastModifiedDate { get { return Obj.LastModifiedDate.ToShortDateString(); } }
        public string Filename { get { return Obj.Filename; } }

        public DescriptionViewModel(Description obj) : base(obj)
        {
            Details = $"Collection consists of {obj.BooksCount} books in {obj.ShelfCount} shelves";
        }
    }
}
