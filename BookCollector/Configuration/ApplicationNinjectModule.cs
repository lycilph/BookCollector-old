﻿using BookCollector.Screens.Books;
using BookCollector.Screens.Collections;
using BookCollector.Screens.Import;
using BookCollector.Screens.Settings;
using BookCollector.Screens.Shell;
using BookCollector.Screens.Web;
using BookCollector.Services;
using Core;
using MaterialDesignThemes.Wpf;
using Ninject.Modules;
using NLog;

namespace BookCollector.Configuration
{
    public class ApplicationNinjectModule : NinjectModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override void Load()
        {
            logger.Trace("Loading dependencies for application");

            Bind<IApplicationController>().To<ApplicationController>().InSingletonScope();
            Bind<ISnackbarMessageQueue>().To<SnackbarMessageQueue>().InSingletonScope();
            Bind<INavigationService>().To<NavigationService>().InSingletonScope();
            Bind<ISettingsRepository>().To<SettingsRepository>().InSingletonScope();

            Bind<IShellViewModel>().To<ShellViewModel>().InSingletonScope();

            Bind<IScreen>().To<CollectionsScreenViewModel>();
            Bind<IScreen>().To<BooksScreenViewModel>();
            Bind<IScreen>().To<ImportScreenViewModel>();
            Bind<IScreen>().To<SettingsScreenViewModel>();
            Bind<IScreen>().To<WebScreenViewModel>();
            Bind<IScreen>().To<SearchScreenViewModel>();
            Bind<IScreen>().To<NavigationScreenViewModel>();
        }
    }
}
