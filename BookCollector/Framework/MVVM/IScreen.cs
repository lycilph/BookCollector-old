namespace BookCollector.Framework.MVVM
{
    public interface IScreen
    {
        string DisplayName { get; set; }

        void Activate();
        void Deactivate();
    }
}
