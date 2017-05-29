namespace BookCollector.Framework.MessageBus
{
    public interface IHandle { }

    public interface IHandle<T> : IHandle
    { 
        void Handle(T message);
    }
}
