using Caliburn.Micro;

namespace BookCollector.Shell
{
    public class ShellMessage
    {
        public enum MessageKind { Back, Show, Exit }

        public MessageKind Kind { get; set; }
        public IScreen ViewModel { get; set; }

        public ShellMessage(MessageKind kind, IScreen view_model)
        {
            Kind = kind;
            ViewModel = view_model;
        }

        public static ShellMessage BackMessage()
        {
            return new ShellMessage(MessageKind.Back, null);
        }

        public static ShellMessage ShowMessage(IScreen view_model)
        {
            return new ShellMessage(MessageKind.Show, view_model);
        }

        public static ShellMessage ExitMessage()
        {
            return new ShellMessage(MessageKind.Exit, null);
        }
    }
}
