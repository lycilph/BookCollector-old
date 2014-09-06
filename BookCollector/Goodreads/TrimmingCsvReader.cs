using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace BookCollector.Goodreads
{
    public class TrimmingCsvReader : CsvReader
    {
        private readonly char[] trim_chars;

        public TrimmingCsvReader(TextReader reader, params char[] trim_chars) : base(reader)
        {
            this.trim_chars = trim_chars;
        }

        public TrimmingCsvReader(TextReader reader, CsvConfiguration configuration) : base(reader, configuration)
        {
        }

        public TrimmingCsvReader(ICsvParser parser) : base(parser)
        {
        }

        public override string GetField(int index)
        {
            return base.GetField(index).Trim(trim_chars);
        }
    }
}
