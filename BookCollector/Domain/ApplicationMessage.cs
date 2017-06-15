namespace BookCollector.Domain
{
    public class ApplicationMessage
    {
        public enum MessageKind
        {
            // Shell messages
            ShellLoaded, ShellClosing,
            // Navigation and controll messages
            NavigateTo
        };

        public MessageKind Kind { get; private set; }
        public string Text { get; set; }

        public ApplicationMessage(MessageKind kind) : this(kind, string.Empty) { }
        public ApplicationMessage(MessageKind kind, string text)
        {
            Kind = kind;
            Text = text;
        }

        public static ApplicationMessage ShellLoaded()
        {
            return new ApplicationMessage(MessageKind.ShellLoaded);
        }

        public static ApplicationMessage ShellClosing()
        {
            return new ApplicationMessage(MessageKind.ShellClosing);
        }

        public static ApplicationMessage NavigateTo(string screen)
        {
            return new ApplicationMessage(MessageKind.NavigateTo, screen);
        }
    }
}
