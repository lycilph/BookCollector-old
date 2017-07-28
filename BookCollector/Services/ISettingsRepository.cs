using BookCollector.Data;

namespace BookCollector.Services
{
    public interface ISettingsRepository
    {
        Settings LoadOrCreate();
        void Save(Settings settings);
    }
}