using System.Collections.Generic;

namespace BookCollector.Data
{
    public interface IDataController
    {
        List<Book> Books { get; }
    }
}