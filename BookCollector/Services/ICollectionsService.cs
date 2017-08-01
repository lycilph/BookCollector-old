using BookCollector.Data;

namespace BookCollector.Services
{
    public interface ICollectionsService
    {
        Collection Current { get; set; }

        void Initialize();
    }
}