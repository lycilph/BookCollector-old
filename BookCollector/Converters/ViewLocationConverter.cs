using System;
using System.Globalization;
using System.Windows.Data;
using Core;

namespace BookCollector.Converters
{
    public class ViewLocationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            // Locate and Create view
            var view = ViewManager.CreateAndBindViewForModel(value);
            if (view == null)
                throw new ArgumentException($"Could not find view for {value.GetType().Name}");

            return view;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
