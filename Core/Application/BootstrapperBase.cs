using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using NLog;

namespace Core.Application
{
    public class BootstrapperBase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void Initialize()
        {
            logger.Trace("Initializing");

            InitializeApplication();
            InitializeAssemblySource();
            Configure();
        }

        private void InitializeApplication()
        {
            logger.Trace("Hooking up application events");

            System.Windows.Application.Current.Startup += OnStartup;
            System.Windows.Application.Current.DispatcherUnhandledException += OnUnhandledException;
            System.Windows.Application.Current.Exit += OnExit;
        }

        private void InitializeAssemblySource()
        {
            logger.Trace("Initializing AssemblySource");

            AssemblySource.Assemblies.AddRange(SelectAssemblies());
        }

        protected virtual List<Assembly> SelectAssemblies()
        {
            logger.Trace("Adding current assembly to AssemblySource");

            return new List<Assembly> { GetType().Assembly };
        }

        protected virtual void Configure() { }

        protected virtual void OnStartup(object sender, StartupEventArgs e) { }

        protected virtual void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) { }

        protected virtual void OnExit(object sender, ExitEventArgs e) { }

        protected virtual object GetInstance(Type service)
        {
            logger.Trace("Fallback GetInstance - consider overriding this!");

            return Activator.CreateInstance(service);
        }

        protected void DisplayRootViewFor<TViewModel>()
        {
            logger.Trace($"Displaying root view for {typeof(TViewModel).Name}");

            var view_model = GetInstance(typeof(TViewModel));
            var view = ViewManager.CreateAndBindViewForModel(view_model);

            // Show window
            var window = view as Window;
            if (window == null)
                throw new InvalidOperationException($"View for {typeof(TViewModel).FullName} must be of type Window");
            window.Show();
        }
    }
}
