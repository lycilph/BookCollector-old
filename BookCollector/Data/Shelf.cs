using System.Collections.Generic;
using System.Diagnostics;
using Core.Extensions;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Data
{
    [DebuggerDisplay("Name = {Name}, Books = {Books.Count}")]
    [JsonObject(MemberSerialization.OptOut)]
    public class Shelf : ReactiveObject
    {
        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        public bool IsDefault { get; set; } = false;

        public List<Book> Books { get; set; } = new List<Book>();

        public Shelf() { }
        public Shelf(string name, bool is_default = false)
        {
            Name = name;
            IsDefault = is_default;
        }

        public void Add(List<Book> books_to_add)
        {
            books_to_add.Apply(Add);
        }

        public void Add(Book book)
        {
            // Add to list of books on shelf
            Books.Add(book);

            // Add a link from the book to this shelf
            book.Shelves.Add(this);
        }
    }
}
