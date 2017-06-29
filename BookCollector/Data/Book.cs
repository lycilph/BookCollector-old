using ReactiveUI;

namespace BookCollector.Data
{
    public class Book : ReactiveObject
    {
        private string _Title = string.Empty;
        public string Title
        {
            get { return _Title; }
            set { this.RaiseAndSetIfChanged(ref _Title, value); }
        }

        private ReactiveList<string> _Authors = new ReactiveList<string>();
        public ReactiveList<string> Authors
        {
            get { return _Authors; }
            set { this.RaiseAndSetIfChanged(ref _Authors, value); }
        }

        private string _ISBN10 = string.Empty;
        public string ISBN10
        {
            get { return _ISBN10; }
            set { this.RaiseAndSetIfChanged(ref _ISBN10, value); }
        }

        private string _ISBN13 = string.Empty;
        public string ISBN13
        {
            get { return _ISBN13; }
            set { this.RaiseAndSetIfChanged(ref _ISBN13, value); }
        }

        private ReactiveList<Shelf> _Shelves = new ReactiveList<Shelf>();
        public ReactiveList<Shelf> Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
        }

        private string _Source = string.Empty;
        public string Source
        {
            get { return _Source; }
            set { this.RaiseAndSetIfChanged(ref _Source, value); }
        }

        private ReactiveList<string> _History = new ReactiveList<string>();
        public ReactiveList<string> History
        {
            get { return _History; }
            set { this.RaiseAndSetIfChanged(ref _History, value); }
        }

        public Book() { }
        public Book(string title)
        {
            Title = title;
        }

        public void Add(Shelf shelf)
        {
            // Add shelf if not already present
            if (!Shelves.Contains(shelf))
                Shelves.Add(shelf);
        }

        public void Remove(Shelf shelf)
        {
            // Remove is tolerant of items not existing in collection
            Shelves.Remove(shelf);
        }

        public bool IsOnShelf(Shelf shelf)
        {
            return Shelves.Contains(shelf);
        }
    }
}
