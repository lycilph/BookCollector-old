namespace BookCollector.Shell
{
    public interface IShellScreen
    {
        string DisplayName { get; set; }
        void Activate();
    }
}