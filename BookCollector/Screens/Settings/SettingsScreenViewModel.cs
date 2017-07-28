using MahApps.Metro.Controls;
using Core;

namespace BookCollector.Screens.Settings
{
    public class SettingsScreenViewModel : FlyoutBase, ISettingsScreen
    {
        public SettingsScreenViewModel() : base(Position.Right)
        {
            DisplayName = "Settings";
        }
    }
}
