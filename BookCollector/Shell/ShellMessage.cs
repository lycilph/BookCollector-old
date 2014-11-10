using Caliburn.Micro;

namespace BookCollector.Shell
{
    public class ShellMessage
    {
        public enum MessageKind { Back, Show, Exit, Text }

        public MessageKind Kind { get; set; }
        public IScreen ViewModel { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        
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

        public static ShellMessage TextMessage(string text1, string text2)
        {
            return new ShellMessage {Kind = MessageKind.Text, Text1 = text1, Text2 = text2};
        }
    }
}
