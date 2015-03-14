using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using BookCollector.Controllers;
using BookCollector.Services;
using Caliburn.Micro;
using NLog;
using Panda.ApplicationCore.Bootstrapper;
using LogManager = NLog.LogManager;

namespace BookCollector
{
    public class ApplicationTasks
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [Export(PandaBootstrapper.STARTUP_TASK_NAME, typeof(BootstrapperTask))]
        public void ApplicationStartup()
        {
            logger.Trace("Application Startup");

            var settings = IoC.Get<ISettings>();
            settings.Load();

            var data_controller = IoC.Get<IDataController>();
            data_controller.Load();

            // This is needed to initialize the statusbar correctly
            var status_controller = IoC.Get<IStatusController>();
            status_controller.ClearStatusText();

            var navigation_controller = IoC.Get<INavigationController>();
            navigation_controller.NavigateToMain();
            navigation_controller.NavigateToSelection();
        }

        [Export(PandaBootstrapper.STARTUP_TASK_NAME, typeof(BootstrapperTask))]
        public void ApplyCaliburnOverrides()
        {
            logger.Trace("Apply Caliburn Overrides");

            var irregular_nouns = new Dictionary<string, string>
            {
                {"Shelves", "Shelf"}
            };
            var default_singularize = ConventionManager.Singularize;
            ConventionManager.Singularize = str => irregular_nouns.ContainsKey(str) ? irregular_nouns[str] : default_singularize(str);

            MessageBinder.SpecialValues.Add("$pressedkey", context =>
            {
                // NOTE: IMPORTANT - you MUST add the dictionary key as lowercase as CM
                // does a ToLower on the param string you add in the action message
                var key_args = context.EventArgs as KeyEventArgs;

                if (key_args != null)
                    return key_args.Key;

                return null;
            });

            var default_locator = ViewLocator.LocateTypeForModelType;
            ViewLocator.LocateTypeForModelType = (model_type, display_location, context) =>
            {
                var view_type = default_locator(model_type, display_location, context);
                if (view_type == null && context != null)
                {
                    var base_name = model_type.FullName.Replace("ViewModel", "");
                    var view_name = base_name + context + "View";
                    view_type = AssemblySource.FindTypeByNames(new List<string> { view_name });
                }
                return view_type;
            };
        }


        [Export(PandaBootstrapper.SHUTDOWN_TASK_NAME, typeof(BootstrapperTask))]
        public void ApplicationShutdown()
        {
            logger.Trace("Application Shutdown");

            var settings = IoC.Get<ISettings>();
            settings.Save();

            var data_controller = IoC.Get<IDataController>();
            data_controller.Save();
        }
    }
}
