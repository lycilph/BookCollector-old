using BookCollector.Screens;
using Caliburn.Micro;

namespace BookCollector.Shell
{
    public class ShellMessage
    {
        public enum MessageKind { Back, Show, Text, Busy }

        public MessageKind Kind { get; set; }
        public IShellScreen ViewModel { get; set; }
        public string Message { get; set; }
        public bool State { get; set; }
        
        public static ShellMessage Back()
        {
            return new ShellMessage {Kind = MessageKind.Back};
        }

        public static ShellMessage Show(IShellScreen view_model)
        {
            return new ShellMessage {Kind = MessageKind.Show, ViewModel = view_model};
        }

        public static ShellMessage Show(string screen)
        {
            var view_model = IoC.Get<IShellScreen>(screen);
            return Show(view_model);
        }

        public static ShellMessage Text(string message)
        {
            return new ShellMessage {Kind = MessageKind.Text, Message = message};
        }

        public static ShellMessage Busy(bool state)
        {
            return new ShellMessage {Kind = MessageKind.Busy, State = state};
        }
    }
}
