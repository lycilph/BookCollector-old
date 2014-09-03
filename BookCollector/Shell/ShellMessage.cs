using Framework.Docking;

namespace BookCollector.Shell
{
    public class ShellMessage
    {
        public enum MessageKind { Show }

        public MessageKind Kind { get; set; }
        public ILayoutItem Item { get; set; }

        public ShellMessage(MessageKind kind, ILayoutItem item)
        {
            Kind = kind;
            Item = item;
        }

        public static ShellMessage ShowMessage(ILayoutItem item)
        {
            return new ShellMessage(MessageKind.Show, item);
        }
    }
}
