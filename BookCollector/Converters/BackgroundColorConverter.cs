using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Core.Utility;

namespace BookCollector.Converters
{
    public class BackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var selected = (bool)value;
            if (selected)
            {
                if (parameter is BindingProxy proxy)
                    return proxy.Data;
                else
                    return parameter;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
