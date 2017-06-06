using MahApps.Metro.Controls;

namespace BookCollector.Shell
{
    public interface IFlyout
    {
        bool IsOpen { get; set; }
        bool IsPinned { get; set; }
        Position Position { get; set; }

        void Toggle();
        void Show();
        void Hide();
    }
}
