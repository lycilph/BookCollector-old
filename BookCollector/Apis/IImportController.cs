using BookCollector.Model;

namespace BookCollector.Apis
{
    public interface IImportController
    {
        string ApiName { get; }
        void Start(ProfileDescription profile);
    }
}
