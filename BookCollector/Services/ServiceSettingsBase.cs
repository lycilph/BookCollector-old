using ReactiveUI;

namespace BookCollector.Services
{
    public class ServiceSettingsBase<T> : ReactiveObject where T : class 
    {
        public virtual T Encrypt() { return null; }
        public virtual T Decrypt() { return null; }
    }
}
