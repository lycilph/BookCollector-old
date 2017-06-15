using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BookCollector.Framework.Converters
{
    public class ShellMarginConverter : IValueConverter
    {
        public Thickness TrueValueMargin { get; set; }
        public Thickness FalseValueMargin { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return TrueValueMargin;
            else
                return FalseValueMargin;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
