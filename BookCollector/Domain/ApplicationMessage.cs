namespace BookCollector.Domain
{
    public class ApplicationMessage
    {
        public enum MessageKind
        {
            NavigateTo,
            CollectionChanged, 
            SearchTextChanged
        };

        public MessageKind Kind { get; private set; }
        public string Text { get; set; }

        public ApplicationMessage(MessageKind kind) : this(kind, string.Empty) { }
        public ApplicationMessage(MessageKind kind, string text)
        {
            Kind = kind;
            Text = text;
        }

        public static ApplicationMessage NavigateTo(string screen)
        {
            return new ApplicationMessage(MessageKind.NavigateTo, screen);
        }

        public static ApplicationMessage CollectionChanged()
        {
            return new ApplicationMessage(MessageKind.CollectionChanged);
        }

        public static ApplicationMessage SearchTextChanged(string text)
        {
            return new ApplicationMessage(MessageKind.SearchTextChanged, text);
        }
    }
}
