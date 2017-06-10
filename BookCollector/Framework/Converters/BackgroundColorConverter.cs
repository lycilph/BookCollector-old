using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BookCollector.Framework.Converters
{
    public class BackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var selected = (bool)value;
            var proxy = (BindingProxy)parameter;
            if (selected && proxy != null)
                return proxy.Data;
            else
                return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
