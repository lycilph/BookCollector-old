namespace BookCollector
{
    public class ApplicationMessage
    {
        public enum MessageKind { ShellInitialized, ShowProfiles }

        public MessageKind Kind { get; set; }

        public static ApplicationMessage ShellInitialized()
        {
            return new ApplicationMessage { Kind = MessageKind.ShellInitialized };
        }

        public static ApplicationMessage ShowProfiles()
        {
            return new ApplicationMessage { Kind = MessageKind.ShowProfiles };
        }
    }
}
