using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Threading;

namespace BookCollector.Framework.Messaging
{
    public class MessageHandler
    {
        private readonly WeakReference reference;
        private readonly Dictionary<Type, MethodInfo> handler_methods = new Dictionary<Type, MethodInfo>();

        public bool IsDead
        {
            get { return reference.Target == null; }
        }

        public MessageHandler(object subscriber)
        {
            reference = new WeakReference(subscriber);

            var interfaces = subscriber.GetType()
                                       .GetInterfaces()
                                       .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IHandle<>));

            foreach (var i in interfaces)
            {
                var type = i.GetGenericArguments()[0];
                var method = i.GetMethod("Handle", new Type[] { type });

                if (method != null)
                    handler_methods[type] = method;
            }
        }

        public bool Handle(Type message_type, object message)
        {
            var target = reference.Target;
            if (target == null)
                return false;

            foreach (var pair in handler_methods)
            {
                if (pair.Key.IsAssignableFrom(message_type))
                    pair.Value.Invoke(target, new[] { message });
            }

            return true;
        }

        public bool Matches(object instance)
        {
            return reference.Target == instance;
        }
    }
}
