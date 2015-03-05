﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using NLog;
using Panda.ApplicationCore;
using LogManager = NLog.LogManager;

namespace BookSearch
{
    public class ApplicationTasks
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [Export(ApplicationBootstrapper.STARTUP_TASK_NAME, typeof(BootstrapperTask))]
        public void ApplyMessageBinderSpecialValues()
        {
            logger.Trace("Applying MessageBinder SpecialValues");

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
        }


        [Export(ApplicationBootstrapper.STARTUP_TASK_NAME, typeof(BootstrapperTask))]
        public void ApplyStringFormat()
        {
            logger.Trace("Applying String Format");

            var aps = ConventionManager.ApplyStringFormat;
            ConventionManager.ApplyStringFormat = (binding, convention, property_info) =>
            {
                if (property_info.Name == "Score" && typeof (double).IsAssignableFrom(property_info.PropertyType))
                {
                    binding.StringFormat = "{0:F2}";
                    return;
                }

                aps(binding, convention, property_info);
            };

            var default_locator = ViewLocator.LocateTypeForModelType;
            ViewLocator.LocateTypeForModelType = (model_type, display_location, context) =>
            {
                var view_type = default_locator(model_type, display_location, context);
                if (view_type == null && context != null)
                {
                    var base_name = model_type.FullName.Replace("ViewModel", "");
                    var view_name = base_name + context + "View";
                    view_type = AssemblySource.FindTypeByNames(new List<string> {view_name});
                }
                return view_type;
            };
        }
    }
}