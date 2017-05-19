using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace BookCollector.ThirdParty.Goodreads
{
    public class TrimmingCsvReader : CsvReader
    {
        public TrimmingCsvReader(TextReader reader, CsvConfiguration configuration) : base(reader, configuration) { }

        public override string GetField(int index)
        {
            var field = base.GetField(index);
            return field.TrimStart('=').Replace("\"", "");
        }
    }
}
