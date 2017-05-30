﻿using System.IO;
using System.Linq;

namespace BookCollector.Extensions
{
    public static class StringExtensions
    {
        public static string MakeFilenameSafe(this string filename, char replace = '_')
        {
            var invalid_chars = Path.GetInvalidFileNameChars();
            return new string(filename.Select(c => invalid_chars.Contains(c) ? replace : c).ToArray());
        }
    }
}
