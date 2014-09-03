using System.ComponentModel.Composition;
using Framework.Window;
using MahApps.Metro.Controls;

namespace BookCollector.Settings
{
    [Export("Settings", typeof(IFlyout))]
    public class SettingsViewModel : FlyoutBase
    {
        public SettingsViewModel() : base("Settings", Position.Right)
        {
        }
    }
}
