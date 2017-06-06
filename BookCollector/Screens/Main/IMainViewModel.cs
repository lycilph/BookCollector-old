using BookCollector.Framework.MVVM;

namespace BookCollector.Screens.Main
{
    public interface IMainViewModel : IScreen
    {
        void ToggleMenu();
        void Show(IMainScreen content);
    }
}