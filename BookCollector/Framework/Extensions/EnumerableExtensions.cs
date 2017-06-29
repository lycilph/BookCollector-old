using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;

namespace BookCollector.Framework.Extensions
{
    public static class EnumerableExtensions
    {
        public static void Apply<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static IEnumerable<T> AddTo<T>(this IEnumerable<T> source, T element)
        {
            var list = source.ToList();
            list.Add(element);
            return list;
        }

        public static ReactiveList<T> ToReactiveList<T>(this IEnumerable<T> source, bool enable_change_tracking = false)
        {
            return new ReactiveList<T>(source) { ChangeTrackingEnabled = enable_change_tracking };
        }
    }
}
