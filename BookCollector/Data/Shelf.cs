using System.Collections.Generic;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Shelf : ReactiveObject
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private ReactiveList<Book> _Books = new ReactiveList<Book>();
        public ReactiveList<Book> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        [JsonIgnore]
        public int Count
        {
            get { return Books.Count; }
        }

        public Shelf() : this("[Name]") { }
        public Shelf(string name)
        {
            Name = name;
        }

        public void AddRange(IEnumerable<Book> books)
        {
            Books.AddRange(books);
        }

        public void Add(Book book)
        {
            Books.Add(book);
        }
    }
}
