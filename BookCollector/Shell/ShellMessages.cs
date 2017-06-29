namespace BookCollector.Shell
{
    public class ShellMessages
    {
        public enum MessageKind { ShellLoaded, ShellClosing };

        public MessageKind Kind { get; private set; }

        public ShellMessages(MessageKind kind)
        {
            Kind = kind;
        }

        public static ShellMessages ShellLoaded()
        {
            return new ShellMessages(MessageKind.ShellLoaded);
        }

        public static ShellMessages ShellClosing()
        {
            return new ShellMessages(MessageKind.ShellClosing);
        }
    }
}
