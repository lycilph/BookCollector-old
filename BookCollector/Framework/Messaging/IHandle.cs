namespace BookCollector.Framework.Messaging
{
    public interface IHandle { }

    public interface IHandle<T> : IHandle
    {
        void Handle(T message);
    }
}
