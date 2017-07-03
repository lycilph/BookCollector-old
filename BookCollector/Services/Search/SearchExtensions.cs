using System;
using System.Collections.Generic;
using System.Linq;

namespace BookCollector.Services.Search
{
    public static class SearchExtensions
    {
        public static IDictionary<string, double> Normalize(this IDictionary<string, double> source)
        {
            var squared_sum = source.Values.Sum(v => v * v);
            var factor = 1 / Math.Sqrt(squared_sum);
            return source.ToDictionary(kvp => kvp.Key, kvp => kvp.Value * factor);
        }

        public static List<double> Normalize(this IEnumerable<double> vector)
        {
            var length = Math.Sqrt(vector.Sum(v => v * v));
            return length > 0.0 ? vector.Select(v => v / length).ToList() : vector.ToList();
        }
    }
}
