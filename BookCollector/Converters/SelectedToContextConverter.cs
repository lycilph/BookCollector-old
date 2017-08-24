using System;
using System.Globalization;
using System.Windows.Data;

namespace BookCollector.Converters
{
    public class SelectedToContextConverter : IValueConverter
    {
        public string SelectedContext { get; set; } = string.Empty;
        public string UnselectedContext { get; set; } = string.Empty;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return SelectedContext;
            else
                return UnselectedContext;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
