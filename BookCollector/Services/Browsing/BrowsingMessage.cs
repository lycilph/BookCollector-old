namespace BookCollector.Services.Browsing
{
    public class BrowsingMessage
    {
        public enum MessageKind { LoadStart, LoadEnd }

        public MessageKind Kind { get; set; }
        public string Url { get; set; }

        public static BrowsingMessage LoadStart(string url)
        {
            return new BrowsingMessage {Kind = MessageKind.LoadStart, Url = url};
        }

        public static BrowsingMessage LoadEnd(string url)
        {
            return new BrowsingMessage { Kind = MessageKind.LoadEnd, Url = url };
        }
    }
}
