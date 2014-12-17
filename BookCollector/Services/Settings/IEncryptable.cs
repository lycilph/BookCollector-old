namespace BookCollector.Services.Settings
{
    public interface IEncryptable<out T> where T : class
    {
        T Encrypt(string key);
        T Decrypt(string key);
    }
}
