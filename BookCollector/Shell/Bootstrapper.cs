using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using AutoMapper;
using BookCollector.Data;
using BookCollector.Initialization;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using Ninject;

namespace BookCollector.Shell
{
    public class Bootstrapper : BootstrapperBase
    {
        private ILog log;
        private IKernel kernel;

        static Bootstrapper()
        {
            LogManager.GetLog = type => new DebugLog(type);
        }

        public Bootstrapper()
        {
            log = LogManager.GetLog(typeof(Bootstrapper));

            log.Info("Initializeing bootstrapper");
            Initialize();
        }

        protected override void Configure()
        {
            log.Info("Configuring bootstrapper");

            RegisterAutomapperMappings();
            SetupDependencyInjectionBindings();
            ApplyViewLocatorOverride();
            ApplyBindingScopeOverride();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            kernel.Dispose();
            base.OnExit(sender, e);
        }

        protected override object GetInstance(Type service, string key)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            return kernel.Get(service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return kernel.GetAll(service);
        }

        protected override void BuildUp(object instance)
        {
            kernel.Inject(instance);
        }

        private void RegisterAutomapperMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
        }

        private void SetupDependencyInjectionBindings()
        {
            kernel = new StandardKernel();

            // Framework bindings
            kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();

            // Application bindings
            kernel.Bind<IDataController>().To<DataController>().InSingletonScope();
        }

        private void ApplyViewLocatorOverride()
        {
            // Can this be replaced by: http://caliburnmicro.com/documentation/custom-conventions ?
            var default_locator = ViewLocator.LocateTypeForModelType;
            ViewLocator.LocateTypeForModelType = (model_type, display_location, context) =>
            {
                var view_type = default_locator(model_type, display_location, context);
                if (view_type == null && context != null)
                {
                    // 1. Split on FullName
                    // 2. Last part -> replace "ViewModel" with Context+"View"
                    // 3. All other parts -> replace "ViewModel" with "View"
                    // 4. Joint string

                    var name_parts = model_type.FullName.Split(new char[] { '.' }).ToList();
                    var transformed_name_parts = name_parts.Select((part, index) => 
                    (index == name_parts.Count - 1 ? 
                    part.Replace("ViewModel", context + "View") : 
                    part.Replace("ViewModel", "View")));
                    var view_name = String.Join(".", transformed_name_parts);
                    view_type = AssemblySource.FindTypeByNames(new List<string> { view_name });
                }
                return view_type;
            };
        }

        private void ApplyBindingScopeOverride()
        {
            var get_named_elements = BindingScope.GetNamedElements;
            BindingScope.GetNamedElements = (o =>
            {
                var elements = get_named_elements(o).ToList();

                if (o is MetroWindow)
                    elements.AddRange(ResolveMetroWindow(o, get_named_elements));

                return elements;
            });
        }

        private static IEnumerable<FrameworkElement> ResolveMetroWindow(DependencyObject o, Func<DependencyObject, IEnumerable<FrameworkElement>> get_named_elements)
        {
            var list = new List<FrameworkElement>();
            var type = o.GetType();

            // Check fields for elements to add
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                             .Where(f => f.DeclaringType == type)
                             .ToList();
            var flyouts = fields.Where(f => f.FieldType == typeof(FlyoutsControl))
                                .Select(f => f.GetValue(o))
                                .Cast<FlyoutsControl>()
                                .ToList();
            var commands = fields.Where(f => f.FieldType == typeof(WindowCommands))
                                 .Select(f => f.GetValue(o))
                                 .Cast<WindowCommands>()
                                 .ToList();
            list.AddRange(flyouts);
            list.AddRange(commands);

            // Check properties for elements to add
            if (!flyouts.Any())
            {
                type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.PropertyType == typeof(FlyoutsControl))
                    .Select(p => p.GetValue(o))
                    .Cast<FlyoutsControl>()
                    .Where(c => c != null)
                    .Apply(c => list.AddRange(get_named_elements(c)));
            }
            if (!commands.Any())
            {
                type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.PropertyType == typeof(WindowCommands))
                    .Select(p => p.GetValue(o))
                    .Cast<WindowCommands>()
                    .Where(c => c != null)
                    .Apply(c => list.AddRange(get_named_elements(c)));
            }

            return list;
        }
    }
}
