using System;
using System.Windows;

namespace Core
{
    public static class ViewManager
    {
        public static Type LocateViewForModel(object view_model)
        {
            var view_model_type = view_model.GetType();
            var view_model_type_name = view_model_type.FullName;
            var view_type_name = view_model_type_name.Replace("ViewModel", "View");
            return AssemblySource.GetType(view_type_name);
        }

        public static UIElement CreateViewForModel(object view_model)
        {
            var view_type = LocateViewForModel(view_model);
            return Activator.CreateInstance(view_type) as UIElement;
        }

        public static void BindViewToModel(UIElement view, object view_model)
        {
            if (view is FrameworkElement fe)
                fe.DataContext = view_model;

            if (view_model is IViewAware va)
                va.AttachView(view);
        }

        public static UIElement CreateAndBindViewForModel(object view_model)
        {
            var view = CreateViewForModel(view_model);
            BindViewToModel(view, view_model);
            return view;
        }
    }
}
