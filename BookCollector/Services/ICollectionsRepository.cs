using BookCollector.Data;

namespace BookCollector.Services
{
    public interface ICollectionsRepository
    {
        Collection Load(string filename);
        void Save(Collection collection);
        bool Exists(string path);
    }
}