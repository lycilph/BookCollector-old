using System.Collections.Generic;
using System.Linq;
using Core.Utility;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Collection : DirtyTrackingBase
    {
        private Description _Description = new Description();
        public Description Description
        {
            get { return _Description; }
            set { this.RaiseAndSetIfChanged(ref _Description, value); }
        }

        private ReactiveList<Shelf> _Shelves = new ReactiveList<Shelf>();
        public ReactiveList<Shelf> Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
        }

        [JsonIgnore]
        public Shelf DefaultShelf { get { return Shelves.Single(s => s.IsDefault); } }

        [JsonIgnore]
        public List<Book> Books { get { return DefaultShelf.Books; } }

        public Collection() { }
        public Collection(string default_shelf_name)
        {
            var shelf = new Shelf(default_shelf_name, is_default: true);
            Shelves.Add(shelf);
        }

        public Shelf AddShelf(string name)
        {
            var shelf = new Shelf(name);
            Shelves.Add(shelf);
            return shelf;
        }

        public Shelf AddShelf(Shelf shelf)
        {
            Shelves.Add(shelf);
            return shelf;
        }

        public void RemoveShelf(Shelf shelf)
        {
            shelf.RemoveAllBooks();
            Shelves.Remove(shelf);
        }
    }
}
