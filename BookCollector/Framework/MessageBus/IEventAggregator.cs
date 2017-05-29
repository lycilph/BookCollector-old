namespace BookCollector.Framework.MessageBus
{
    public interface IEventAggregator
    {
        void Publish(object message);
        void Subscribe(object subscriber);
        void Unsubscribe(object subscriber);
    }
}