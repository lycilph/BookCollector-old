using System;
using System.Collections.Generic;
using System.Linq;

namespace BookSearch.Services.Scoring
{
    public static class Normalizer
    {
        public static IDictionary<string, double> Normalize(this IDictionary<string, double> source)
        {
            var squared_sum = source.Values.Sum(v => v * v);
            var norm = Math.Sqrt(squared_sum);
            return source.ToDictionary(kvp => kvp.Key, kvp => kvp.Value/norm);
        }

        public static List<double> Normalize(this List<double> source)
        {
            var length = Math.Sqrt(source.Sum(v => v * v));
            return length > 0.0 ? source.Select(v => v / length).ToList() : source.ToList();
        }
    }
}
