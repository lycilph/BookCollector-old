namespace BookCollector.Framework.Messages
{
    public class StatusMessage
    {
        public enum MessageKind { CollectionChanged };

        public MessageKind Kind { get; private set; }

        public string Name { get; set; }

        public StatusMessage(MessageKind kind, string name)
        {
            Kind = kind;
            Name = name;
        }
    }
}
