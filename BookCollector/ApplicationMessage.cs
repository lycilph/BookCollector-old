namespace BookCollector
{
    public class ApplicationMessage
    {
        public enum MessageKind { ShellInitialized }

        public MessageKind Kind { get; set; }

        public static ApplicationMessage ShellInitialized()
        {
            return new ApplicationMessage { Kind = MessageKind.ShellInitialized };
        }
    }
}
