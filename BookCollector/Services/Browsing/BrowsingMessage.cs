using System;

namespace BookCollector.Services.Browsing
{
    public class BrowsingMessage
    {
        public enum MessageKind { LoadStart, LoadEnd }

        public MessageKind Kind { get; set; }
        public Uri Uri { get; set; }

        public static BrowsingMessage LoadStart(Uri uri)
        {
            return new BrowsingMessage {Kind = MessageKind.LoadStart, Uri = uri};
        }

        public static BrowsingMessage LoadEnd(Uri uri)
        {
            return new BrowsingMessage {Kind = MessageKind.LoadEnd, Uri = uri};
        }
    }
}
