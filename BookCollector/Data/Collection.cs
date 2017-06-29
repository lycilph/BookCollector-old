﻿using System.Collections.Generic;
using System.Linq;
using ReactiveUI;

namespace BookCollector.Data
{
    public class Collection : ReactiveObject
    {
        private Description _Description = new Description();
        public Description Description
        {
            get { return _Description; }
            set { this.RaiseAndSetIfChanged(ref _Description, value); }
        }

        private Shelf _DefaultShelf;
        public Shelf DefaultShelf
        {
            get { return _DefaultShelf; }
            private set { this.RaiseAndSetIfChanged(ref _DefaultShelf, value); }
        }

        private ReactiveList<Shelf> _Shelves = new ReactiveList<Shelf>();
        public ReactiveList<Shelf> Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
        }

        public List<Book> Books { get { return DefaultShelf.Books.ToList(); } }

        public Collection() { }
        public Collection(Description description, Shelf default_shelf)
        {
            Description = description;
            SetDefaultShelf(default_shelf);
        }

        public void SetDefaultShelf(Shelf shelf)
        {
            DefaultShelf = shelf;
            Add(shelf);
        }

        public void Add(Shelf shelf)
        {
            if (!Shelves.Contains(shelf))
                Shelves.Add(shelf);
        }

        public void Removed(Shelf shelf)
        {
            throw new System.NotImplementedException();
        }
    }
}
