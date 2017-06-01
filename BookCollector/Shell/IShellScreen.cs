namespace BookCollector.Shell
{
    public interface IShellScreen
    {
        string DisplayName { get; set; }
        IFlyout Tools { get; }
        
        void Activate();
    }
}