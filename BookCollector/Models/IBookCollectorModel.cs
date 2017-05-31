using System.Collections.Generic;
using BookCollector.Data;

namespace BookCollector.Models
{
    public interface IBookCollectorModel
    {
        Collection CurrentCollection { get; set; }

        void AddToCurrentCollection(List<Book> books);
        void LoadAndSetCurrentCollection(string path);
        void SaveCurrentCollection();
    }
}