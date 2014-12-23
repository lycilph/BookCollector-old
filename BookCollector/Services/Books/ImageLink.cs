namespace BookCollector.Services.Books
{
    public class ImageLink
    {
        public string Url { get; set; }
        public string Property { get; set; }

        public ImageLink() : this(string.Empty, string.Empty) {}
        public ImageLink(string url, string property)
        {
            Url = url;
            Property = property;
        }
    }
}
