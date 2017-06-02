using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace Mockups
{
    public partial class SettingsView
    {
        private SwatchesProvider swatches_provider = new SwatchesProvider();
        private PaletteHelper palette_helper = new PaletteHelper();

        public ObservableCollection<Swatch> Primary
        {
            get { return (ObservableCollection<Swatch>)GetValue(PrimaryProperty); }
            set { SetValue(PrimaryProperty, value); }
        }
        public static readonly DependencyProperty PrimaryProperty =
            DependencyProperty.Register("Primary", typeof(ObservableCollection<Swatch>), typeof(SettingsView), new PropertyMetadata(null));

        public ObservableCollection<Swatch> Accent
        {
            get { return (ObservableCollection<Swatch>)GetValue(AccentProperty); }
            set { SetValue(AccentProperty, value); }
        }
        public static readonly DependencyProperty AccentProperty =
            DependencyProperty.Register("Accent", typeof(ObservableCollection<Swatch>), typeof(SettingsView), new PropertyMetadata(null));

        public SettingsView()
        {
            InitializeComponent();
            DataContext = this;

            var palette = palette_helper.QueryPalette();

            Primary = new ObservableCollection<Swatch>(swatches_provider.Swatches);
            PrimayComboBox.SelectedItem = Primary.Single(s => s.Name == palette.PrimarySwatch.Name);

            Accent = new ObservableCollection<Swatch>(swatches_provider.Swatches.Where(s => s.IsAccented));
            AccentComboBox.SelectedItem = Accent.Single(s => s.Name == palette.AccentSwatch.Name);
        }

        private void PrimaryChangedClick(object sender, SelectionChangedEventArgs e)
        {
            palette_helper.ReplacePrimaryColor(PrimayComboBox.SelectedItem as Swatch);
        }

        private void AccentChangedClick(object sender, SelectionChangedEventArgs e)
        {
            palette_helper.ReplaceAccentColor(AccentComboBox.SelectedItem as Swatch);
        }
    }
}
