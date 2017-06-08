namespace BookCollector.Domain
{
    public class ApplicationMessage
    {
        public enum MessageKind { ShellLoaded, ShellClosing, ToggleMainMenu, NavigateTo, CollectionChanged };

        public MessageKind Kind { get; private set; }
        public string Text { get; set; }

        public ApplicationMessage(MessageKind kind) : this(kind, string.Empty) { }
        public ApplicationMessage(MessageKind kind, string text)
        {
            Kind = kind;
            Text = text;
        }

        public static ApplicationMessage ShellLoadedMessage()
        {
            return new ApplicationMessage(MessageKind.ShellLoaded);
        }

        public static ApplicationMessage ShellClosingMessage()
        {
            return new ApplicationMessage(MessageKind.ShellClosing);
        }

        public static ApplicationMessage ToggleMainMenuMessage()
        {
            return new ApplicationMessage(MessageKind.ToggleMainMenu);
        }

        public static ApplicationMessage NavigateToMessage(string screen_name)
        {
            return new ApplicationMessage(MessageKind.NavigateTo, screen_name);
        }

        public static ApplicationMessage CollectionChangedMessage(string filename)
        {
            return new ApplicationMessage(MessageKind.CollectionChanged, filename);
        }
    }
}
