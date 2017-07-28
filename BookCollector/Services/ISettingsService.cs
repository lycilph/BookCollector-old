using BookCollector.Data;

namespace BookCollector.Services
{
    public interface ISettingsService
    {
        Settings Settings { get; }

        void Exit();
        void Initialize();
    }
}