using System;

namespace BookCollector.Controllers.Misc
{
    public static class StringExtensions
    {
        public static bool IsNotNullAndEqual(string s1, string s2)
        {
            return !string.IsNullOrWhiteSpace(s1) && 
                   !string.IsNullOrWhiteSpace(s2) && 
                   string.Equals(s1, s2, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
