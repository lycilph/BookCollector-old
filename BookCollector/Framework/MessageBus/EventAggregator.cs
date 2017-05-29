using System;
using System.Collections.Generic;
using System.Linq;
using BookCollector.Extensions;

namespace BookCollector.Framework.MessageBus
{
    // This implementation is inspired by (ie. stolen from) the caliburn.micro EventAggregator.
    // However there is not thread marshaling here
    public class EventAggregator : IEventAggregator
    {
        private readonly List<MessageHandler> handlers = new List<MessageHandler>();

        public void Subscribe(object subscriber)
        {
            if (subscriber == null)
                throw new ArgumentNullException("subscriber");

            if (handlers.Any(x => x.Matches(subscriber)))
                return;

            handlers.Add(new MessageHandler(subscriber));
        }

        public void Unsubscribe(object subscriber)
        {
            if (subscriber == null)
                throw new ArgumentNullException("subscriber");

            var found = handlers.FirstOrDefault(x => x.Matches(subscriber));

            if (found != null)
                handlers.Remove(found);
        }

        public void Publish(object message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var message_type = message.GetType();
            var dead = handlers.Where(h => !h.Handle(message_type, message)).ToList();
            if (dead.Any())
                dead.Apply(h => handlers.Remove(h));
        }
    }
}
