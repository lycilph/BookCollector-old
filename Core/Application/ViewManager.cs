﻿using System;
using System.Windows;
using Core.Shell;

namespace Core.Application
{
    public static class ViewManager
    {
        public static Type LocateViewForModel(object view_model, string context = "")
        {
            var view_model_type = view_model.GetType();
            var view_model_type_name = view_model_type.FullName;
            var view_type_name = view_model_type_name.Replace("ViewModel", context+"View");
            return AssemblySource.GetType(view_type_name);
        }

        public static UIElement CreateViewForModel(object view_model, string context = "")
        {
            var view_type = LocateViewForModel(view_model, context);
            return Activator.CreateInstance(view_type) as UIElement;
        }

        public static void BindViewToModel(UIElement view, object view_model)
        {
            if (view is FrameworkElement fe)
                fe.DataContext = view_model;

            if (view_model is IViewAware va)
                va.AttachView(view);
        }

        public static UIElement CreateAndBindViewForModel(object view_model, string context = "")
        {
            var view = CreateViewForModel(view_model, context);
            BindViewToModel(view, view_model);
            return view;
        }
    }
}
