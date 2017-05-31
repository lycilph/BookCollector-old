namespace BookCollector.Application.Messages
{
    public class ApplicationMessage
    {
        public enum MessageKind { ShellLoaded, ShellClosing };

        public MessageKind Kind { get; private set; }

        public ApplicationMessage(MessageKind kind)
        {
            Kind = kind;
        }
    }
}
