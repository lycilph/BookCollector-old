using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Collection : ReactiveObject
    {
        private const string AllShelfName = "All";

        private string _Id = Guid.NewGuid().ToString().ToUpperInvariant();
        public string Id
        {
            get { return _Id; }
            set { this.RaiseAndSetIfChanged(ref _Id, value); } // This is used when deserializing
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private ReactiveList<Shelf> _Shelves = new ReactiveList<Shelf>();
        public ReactiveList<Shelf> Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
        }

        [JsonIgnore]
        public Shelf All { get; private set; }

        public Collection() : this("[Collection]") { }
        public Collection(string name)
        {
            Name = name;
        }

        public void Initialize()
        {
            All = GetOrCreate(AllShelfName);
        }

        public Shelf GetOrCreate(string name)
        {
            Shelf result = null;

            if (Shelves != null && Shelves.Any())
                result = Shelves.FirstOrDefault(s => s.Name == name);

            if (result == null)
            {
                result = new Shelf(name);
                Add(result);
            }

            return result;
        }

        public void AddRange(IEnumerable<Book> books)
        {
            All.AddRange(books);
        }

        public void Add(Book book)
        {
            All.Add(book);
        }

        public void Add(Shelf shelf)
        {
            Shelves.Add(shelf);
        }

        public void Remove(Shelf shelf)
        {
            Shelves.Remove(shelf);
        }
    }
}
