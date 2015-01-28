using System.Collections.Generic;
using Newtonsoft.Json;

namespace BookCollector.Model
{
    public class DownloadQueueItem
    {
        public string BookId { get; set; }
        [JsonIgnore]
        public Book Book { get; set; }
        public List<ImageLink> ImageLinks { get; set; }

        public DownloadQueueItem() { }

        public DownloadQueueItem(Book book, List<ImageLink> image_links)
        {
            Book = book;
            BookId = book.Id;
            ImageLinks = image_links;
        }
    }
}
