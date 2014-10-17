using System.Collections.Generic;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Data
{
    public class Book : ReactiveObject
    {
        private BookStatus _Status = BookStatus.Invalid;
        [JsonProperty]
        public BookStatus Status
        {
            get { return _Status; }
            set { this.RaiseAndSetIfChanged(ref _Status, value); }
        }

        private string _Id = string.Empty;
        [JsonProperty]
        public string Id
        {
            get { return _Id; }
            set { this.RaiseAndSetIfChanged(ref _Id, value); }
        }

        private string _Title = string.Empty;
        [JsonProperty]
        public string Title
        {
            get { return _Title; }
            set { this.RaiseAndSetIfChanged(ref _Title, value); }
        }

        private string _Author = string.Empty;
        [JsonProperty]
        public string Author
        {
            get { return _Author; }
            set { this.RaiseAndSetIfChanged(ref _Author, value); }
        }

        private string _ISBN = string.Empty;
        [JsonProperty]
        public string ISBN
        {
            get { return _ISBN; }
            set { this.RaiseAndSetIfChanged(ref _ISBN, value); }
        }

        private string _ISBN13 = string.Empty;
        [JsonProperty]
        public string ISBN13
        {
            get { return _ISBN13; }
            set { this.RaiseAndSetIfChanged(ref _ISBN13, value); }
        }

        private string _Image;
        [JsonProperty]
        public string Image
        {
            get { return _Image; }
            set { this.RaiseAndSetIfChanged(ref _Image, value); }
        }

        private string _Description = string.Empty;
        [JsonProperty]
        public string Description
        {
            get { return _Description; }
            set { this.RaiseAndSetIfChanged(ref _Description, value); }
        }

        // Temporary properties (not persisted)
        private ReactiveList<Book> _SimilarBooks = new ReactiveList<Book>();
        public ReactiveList<Book> SimilarBooks
        {
            get { return _SimilarBooks; }
            set { this.RaiseAndSetIfChanged(ref _SimilarBooks, value); }
        }
    }
}
