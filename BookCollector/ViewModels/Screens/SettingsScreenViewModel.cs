using System;
using System.Linq;
using System.Reactive.Linq;
using BookCollector.Data;
using BookCollector.Domain;
using BookCollector.Framework.Extensions;
using BookCollector.Services;
using BookCollector.Shell;
using MahApps.Metro.Controls;
using MaterialDesignColors;
using ReactiveUI;

namespace BookCollector.ViewModels.Screens
{
    public class SettingsScreenViewModel : FlyoutBase
    {
        private IApplicationModel application_model;

        public bool LoadCollectionOnStartup
        {
            get { return application_model.Settings.LoadCollectionOnStartup; }
            set
            {
                if (value != application_model.Settings.LoadCollectionOnStartup)
                {
                    this.RaisePropertyChanging();
                    application_model.Settings.LoadCollectionOnStartup = value;
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

        public SettingsScreenViewModel(IApplicationModel application_model, IThemeService theme_service) : base(Constants.SettingsScreenDisplayName, Position.Right)
        {
            this.application_model = application_model;

            PrimaryColors = theme_service.GetAllPrimaryColors().ToReactiveList();
            AccentColors = theme_service.GetAllAccentColors().ToReactiveList();

            this.WhenAnyValue(x => x.SelectedPrimaryColor)
                .Where(swatch => swatch != null)
                .Subscribe(swatch => 
                {
                    theme_service.SetPrimary(swatch.Name);
                    this.application_model.Settings.PrimaryColor = swatch.Name;
                });

            this.WhenAnyValue(x => x.SelectedAccentColor)
                .Where(swatch => swatch != null)
                .Subscribe(swatch =>
                {
                    theme_service.SetAccent(swatch.Name);
                    this.application_model.Settings.AccentColor = swatch.Name;
                });
        }

        public override void Activate()
        {
            base.Activate();

            SelectedPrimaryColor = PrimaryColors.Single(s => s.Name == application_model.Settings.PrimaryColor);
            SelectedAccentColor = AccentColors.Single(s => s.Name == application_model.Settings.AccentColor);
        }
    }
}
