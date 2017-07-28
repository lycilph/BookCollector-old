using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BookCollector.Converters
{
    public class MarginConverter : IValueConverter
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
