using System.Collections.Generic;
using BookCollector.Data;

namespace BookCollector.Domain
{
    public interface IImporter
    {
        List<Book> Import(string filename);
    }
}