using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Shelf : ReactiveObject
    {
        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private string _Description = string.Empty;
        public string Description
        {
            get { return _Description; }
            set { this.RaiseAndSetIfChanged(ref _Description, value); }
        }

        private bool _IsLocked = false;
        public bool IsLocked
        {
            get { return _IsLocked; }
            set { this.RaiseAndSetIfChanged(ref _IsLocked, value); }
        }

        private ReactiveList<Book> _Books = new ReactiveList<Book>();
        public ReactiveList<Book> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        public Shelf() { }
        public Shelf(string name, string description = "", bool is_locked = false)
        {
            Name = name;
            Description = description;
            IsLocked = is_locked;
        }

        public void Add(Book book)
        {
            // Add book to shelf if not already present
            if (!Books.Contains(book))
                Books.Add(book);

            // Add this shelf to the book
            book.Add(this);
        }

        public void Remove(Book book)
        {
            // Remove is tolerant of items not existing in collection
            Books.Remove(book);

            // Remove this shelf from the book
            book.Remove(this);
        }
    }
}
