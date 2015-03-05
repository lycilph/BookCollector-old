using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using BookCollector.Controllers;
using Caliburn.Micro;
using NLog;
using Panda.ApplicationCore;
using LogManager = NLog.LogManager;

namespace BookCollector
{
    public class ApplicationTasks
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [Export(ApplicationBootstrapper.STARTUP_TASK_NAME, typeof(BootstrapperTask))]
        public void ApplicationStartup()
        {
            logger.Trace("Application Startup");

            var navigation_controller = IoC.Get<INavigationController>();
            navigation_controller.NavigateToMain();
        }

        [Export(ApplicationBootstrapper.STARTUP_TASK_NAME, typeof(BootstrapperTask))]
        public void ApplyCaliburnOverrides()
        {
            logger.Trace("Apply Caliburn Overrides");

            MessageBinder.SpecialValues.Add("$pressedkey", (context) =>
            {
                // NOTE: IMPORTANT - you MUST add the dictionary key as lowercase as CM
                // does a ToLower on the param string you add in the action message, in fact ideally
                // all your param messages should be lowercase just in case.
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
    }
}
