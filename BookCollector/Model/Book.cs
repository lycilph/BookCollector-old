using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ReactiveUI;

namespace BookCollector.Model
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Book : ReactiveObject
    {
        public string Id { get; set; }

        public string Title { get; set; }
        public List<string> Authors { get; set; }
        public List<string> Narrators { get; set; }
        public string Description { get; set; }
        public string Asin { get; set; }
        public string ISBN10 { get; set; }
        public string ISBN13 { get; set; }
        public string ImportSource { get; set; }

        // The image properties is updated by the Downloader
        private string _Image;
        public string Image
        {
            get { return _Image; }
            set { this.RaiseAndSetIfChanged(ref _Image, value); }
        }
        private string _SmallImage;
        public string SmallImage
        {
            get { return _SmallImage; }
            set { this.RaiseAndSetIfChanged(ref _SmallImage, value); }
        }

        public Book()
        {
            Id = Guid.NewGuid().ToString();
        }

        public bool IsDuplicate(Book book)
        {
            return (!string.IsNullOrWhiteSpace(ISBN10) && !string.IsNullOrWhiteSpace(book.ISBN10) && String.Equals(ISBN10, book.ISBN10, StringComparison.InvariantCultureIgnoreCase)) ||
                   (!string.IsNullOrWhiteSpace(ISBN13) && !string.IsNullOrWhiteSpace(book.ISBN13) && String.Equals(ISBN13, book.ISBN13, StringComparison.InvariantCultureIgnoreCase)) ||
                   (!string.IsNullOrWhiteSpace(Asin) && !string.IsNullOrWhiteSpace(book.Asin) && String.Equals(Asin, book.Asin, StringComparison.InvariantCultureIgnoreCase)) ||
                   (!string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(book.Title) && String.Equals(Title, book.Title, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
