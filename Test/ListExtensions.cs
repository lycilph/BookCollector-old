using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public static class ListExtensions
    {
        public static List<double> Normalize(this List<double> source)
        {
            var length = Math.Sqrt(source.Sum(v => v*v));
            return length > 0.0 ? source.Select(v => v/length).ToList() : source.ToList();
        }
    }
}
