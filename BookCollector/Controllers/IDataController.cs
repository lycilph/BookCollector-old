using BookCollector.Data;

namespace BookCollector.Controllers
{
    public interface IDataController
    {
        User User { get; set; }
        Collection Collection { get; set; }
        void Initialize();
    }
}