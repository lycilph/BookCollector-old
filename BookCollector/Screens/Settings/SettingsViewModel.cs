using System;
using System.Linq;
using System.Reactive.Linq;
using BookCollector.Framework.Extensions;
using BookCollector.Framework.Mapping;
using BookCollector.Models;
using BookCollector.Shell;
using MahApps.Metro.Controls;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace BookCollector.Screens.Settings
{
    public class SettingsViewModel : FlyoutBase
    {
        private SwatchesProvider swatches_provider = new SwatchesProvider();
        private PaletteHelper palette_helper = new PaletteHelper();
        private ISettingsModel settings_model;

        private bool _LoadCollectionOnStart = true;
        public bool LoadCollectionOnStart
        {
            get { return _LoadCollectionOnStart; }
            set { this.RaiseAndSetIfChanged(ref _LoadCollectionOnStart, value); }
        }

        private ReactiveList<Swatch> _PrimaryColors;
        public ReactiveList<Swatch> PrimaryColors
        {
            get { return _PrimaryColors; }
            set { this.RaiseAndSetIfChanged(ref _PrimaryColors, value); }
        }

        private Swatch _SelectedPrimaryColor;
        public Swatch SelectedPrimaryColor
        {
            get { return _SelectedPrimaryColor; }
            set { this.RaiseAndSetIfChanged(ref _SelectedPrimaryColor, value); }
        }

        private ReactiveList<Swatch> _AccentColors;
        public ReactiveList<Swatch> AccentColors
        {
            get { return _AccentColors; }
            set { this.RaiseAndSetIfChanged(ref _AccentColors, value); }
        }

        private Swatch _SelectedAccentColor;
        public Swatch SelectedAccentColor
        {
            get { return _SelectedAccentColor; }
            set { this.RaiseAndSetIfChanged(ref _SelectedAccentColor, value); }
        }
        
        public SettingsViewModel(ISettingsModel settings_model) : base(ScreenNames.SettingsName, Position.Right)
        {
            this.settings_model = settings_model;

            PrimaryColors = swatches_provider.Swatches.ToReactiveList();
            AccentColors = swatches_provider.Swatches.Where(s => s.IsAccented).ToReactiveList();

            this.WhenAnyValue(x => x.SelectedPrimaryColor)
                .Where(swatch => swatch != null)
                .Subscribe(swatch => palette_helper.ReplacePrimaryColor(swatch));

            this.WhenAnyValue(x => x.SelectedAccentColor)
                .Where(swatch => swatch != null)
                .Subscribe(swatch => palette_helper.ReplaceAccentColor(swatch));

            this.WhenAnyValue(x => x.IsOpen)
                .Skip(1)
                .Subscribe(is_open =>
                {
                    if (is_open)
                        Activate();
                    else
                        Deactivate();
                });
        }

        public void Activate()
        {
            Mapper.Map(settings_model, this);

            SelectedPrimaryColor = PrimaryColors.Single(s => s.Name == settings_model.PrimaryColorName);
            SelectedAccentColor = AccentColors.Single(s => s.Name == settings_model.AccentColorName);
        }

        public void Deactivate()
        {
            Mapper.Map(this, settings_model);
        }
    }
}
