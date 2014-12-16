using Caliburn.Micro;

namespace BookCollector.Shell
{
    public class ShellMessage
    {
        public enum MessageKind { Back, Show, Exit, Text, Busy }

        public MessageKind Kind { get; set; }
        public IScreen ViewModel { get; set; }
        public string Text { get; set; }
        public bool Busy { get; set; }
        
        public static ShellMessage BackMessage()
        {
            return new ShellMessage {Kind = MessageKind.Back};
        }

        public static ShellMessage ShowMessage(IScreen view_model)
        {
            return new ShellMessage {Kind = MessageKind.Show, ViewModel = view_model};
        }

        public static ShellMessage ExitMessage()
        {
            return new ShellMessage {Kind = MessageKind.Exit};
        }

        public static ShellMessage TextMessage(string text)
        {
            return new ShellMessage {Kind = MessageKind.Text, Text = text};
        }

        public static ShellMessage BusyMessage(bool busy)
        {
            return new ShellMessage {Kind = MessageKind.Busy, Busy = busy};
        }
    }
}
