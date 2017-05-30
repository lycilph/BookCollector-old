using BookCollector.Data;

namespace BookCollector.Models
{
    public interface IBookCollectorModel
    {
        Collection CurrentCollection { get; set; }

        void LoadAndSetCurrentCollection(string path);
    }
}