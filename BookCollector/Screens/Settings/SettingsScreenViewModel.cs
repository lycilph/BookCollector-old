using MahApps.Metro.Controls;
using Core;
using ReactiveUI;
using MaterialDesignColors;
using System;
using BookCollector.Services;
using Core.Extensions;
using System.Linq;
using System.Reactive.Linq;

namespace BookCollector.Screens.Settings
{
    public class SettingsScreenViewModel : FlyoutBase, ISettingsScreen
    {
        private ISettingsService settings_service;
        private IThemeService theme_service;

        public bool LoadCollectionOnStartup
        {
            get { return settings_service.Settings.LoadCollectionOnStartup; }
            set
            {
                if (value != settings_service.Settings.LoadCollectionOnStartup)
                {
                    this.RaisePropertyChanging();
                    settings_service.Settings.LoadCollectionOnStartup = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private ReactiveList<Swatch> _PrimaryColors;
        public ReactiveList<Swatch> PrimaryColors
        {
            get { return _PrimaryColors; }
            set { this.RaiseAndSetIfChanged(ref _PrimaryColors, value); }
        }

        private ReactiveList<Swatch> _AccentColors;
        public ReactiveList<Swatch> AccentColors
        {
            get { return _AccentColors; }
            set { this.RaiseAndSetIfChanged(ref _AccentColors, value); }
        }

        public Swatch SelectedPrimaryColor
        {
            get { return PrimaryColors.SingleOrDefault(s => s.Name == settings_service.Settings?.PrimaryColor); }
            set
            {
                if (value.Name != settings_service.Settings.PrimaryColor)
                {
                    this.RaisePropertyChanging();
                    settings_service.Settings.PrimaryColor = value.Name;
                    this.RaisePropertyChanged();
                }
            }
        }

        public Swatch SelectedAccentColor
        {
            get { return PrimaryColors.SingleOrDefault(s => s.Name == settings_service.Settings?.AccentColor); }
            set
            {
                if (value.Name != settings_service.Settings.AccentColor)
                {
                    this.RaisePropertyChanging();
                    settings_service.Settings.AccentColor = value.Name;
                    this.RaisePropertyChanged();
                }
            }
        }

        public int ShelfMappingThreshold
        {
            get { return settings_service.Settings.ShelfMappingThreshold; }
            set
            {
                if (value != settings_service.Settings.ShelfMappingThreshold)
                {
                    this.RaisePropertyChanging();
                    settings_service.Settings.ShelfMappingThreshold = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public SettingsScreenViewModel(ISettingsService settings_service, IThemeService theme_service) : base(Position.Right)
        {
            DisplayName = "Settings";
            this.settings_service = settings_service;
            this.theme_service = theme_service;

            Initialize();
        }

        private void Initialize()
        {
            PrimaryColors = theme_service.GetAllPrimaryColors().OrderBy(s => s.Name).ToReactiveList();
            AccentColors = theme_service.GetAllAccentColors().OrderBy(s => s.Name).ToReactiveList();

            this.WhenAnyValue(x => x.SelectedPrimaryColor)
                .Where(swatch => swatch != null)
                .Subscribe(swatch => theme_service.SetPrimary(swatch.Name));

            this.WhenAnyValue(x => x.SelectedAccentColor)
                .Where(swatch => swatch != null)
                .Subscribe(swatch => theme_service.SetAccent(swatch.Name));
        }
    }
}
