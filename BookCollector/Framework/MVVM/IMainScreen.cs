namespace BookCollector.Framework.MVVM
{
    public interface IMainScreen : IScreen
    {
        bool ShowCollectionCommand { get; set; }
        IScreen MenuContent { get; set; }
        IScreen ExtraContent { get; set; }
    }
}
