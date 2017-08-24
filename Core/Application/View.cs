using System;
using System.Windows;
using System.Windows.Controls;

namespace Core.Application
{
    public static class View
    {
        public static string GetContext(DependencyObject obj)
        {
            return (string)obj.GetValue(ContextProperty);
        }

        public static void SetContext(DependencyObject obj, string value)
        {
            obj.SetValue(ContextProperty, value);
        }

        public static readonly DependencyProperty ContextProperty =
            DependencyProperty.RegisterAttached("Context", typeof(string), typeof(View), new UIPropertyMetadata(string.Empty, OnContextChanged));

        private static void OnContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bound = GetBind(d);
            if (bound == null)
                return;

            var context = e.NewValue as string;
            Bind(d as ContentControl, bound, context);
        }

        public static object GetBind(DependencyObject obj)
        {
            return obj.GetValue(BindProperty);
        }

        public static void SetBind(DependencyObject obj, object value)
        {
            obj.SetValue(BindProperty, value);
        }

        public static readonly DependencyProperty BindProperty = DependencyProperty.RegisterAttached("Bind", typeof(object), typeof(View), new UIPropertyMetadata(null, OnBindChanged));

        private static void OnBindChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var context = GetContext(d);

            Bind(d as ContentControl, e.NewValue, context);
        }

        private static void Bind(ContentControl control, object value, string context)
        {
            if (control == null)
                return;
            
            object vm;
            if (Boolean.TryParse(value as string, out var use_datacontext) && use_datacontext)
                vm = control.DataContext;
            else
                vm = value;

            if (vm == null)
                return;

            var view = ViewManager.CreateAndBindViewForModel(vm, context);
            control.Content = view ?? throw new ArgumentException($"Could not find view for {vm.GetType().Name}");
        }
    }
}
