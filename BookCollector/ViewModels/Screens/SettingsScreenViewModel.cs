using System;
using System.Linq;
using System.Reactive.Linq;
using BookCollector.Domain;
using BookCollector.Framework.Extensions;
using BookCollector.Framework.Logging;
using BookCollector.Models;
using BookCollector.Services;
using BookCollector.Shell;
using MahApps.Metro.Controls;
using MaterialDesignColors;
using ReactiveUI;

namespace BookCollector.ViewModels.Screens
{
    public class SettingsScreenViewModel : FlyoutBase
    {
        private ILog log = LogManager.GetCurrentClassLogger();
        private ISettingsModel settings_model;
        private IThemeService theme_service;

        private bool _LoadCollectionOnStartup;
        public bool LoadCollectionOnStartup
        {
            get { return _LoadCollectionOnStartup; }
            set { this.RaiseAndSetIfChanged(ref _LoadCollectionOnStartup, value); }
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

        public SettingsScreenViewModel(ISettingsModel settings_model, IThemeService theme_service) : base(Constants.SettingsScreenDisplayName, Position.Right)
        {
            DisplayName = Constants.SettingsScreenDisplayName;
            this.settings_model = settings_model;
            this.theme_service = theme_service;

            PrimaryColors = theme_service.GetAllPrimaryColors().ToReactiveList();
            AccentColors = theme_service.GetAllAccentColors().ToReactiveList();

            this.WhenAnyValue(x => x.SelectedPrimaryColor)
                .Where(swatch => swatch != null)
                .Subscribe(swatch =>
                {
                    theme_service.SetPrimary(swatch.Name);
                    this.settings_model.Settings.PrimaryColor = swatch.Name;
                });

            this.WhenAnyValue(x => x.SelectedAccentColor)
                .Where(swatch => swatch != null)
                .Subscribe(swatch =>
                {
                    theme_service.SetAccent(swatch.Name);
                    this.settings_model.Settings.AccentColor = swatch.Name;
                });
        }

        public override void Activate()
        {
            log.Info("Activating");
            base.Activate();

            LoadCollectionOnStartup = settings_model.Settings.LoadCollectionOnStartup;
            SelectedPrimaryColor = PrimaryColors.Single(s => s.Name == settings_model.Settings.PrimaryColor);
            SelectedAccentColor = AccentColors.Single(s => s.Name == settings_model.Settings.AccentColor);
        }

        public override void Deactivate()
        {
            log.Info("Deactivating");
            base.Deactivate();

            if (settings_model == null)
            {
                log.Info("Not ready yet");
                return;
            }

            settings_model.Settings.LoadCollectionOnStartup = LoadCollectionOnStartup;
        }
    }
}
