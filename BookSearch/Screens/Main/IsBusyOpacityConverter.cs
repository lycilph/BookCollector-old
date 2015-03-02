using System;
using System.Globalization;
using System.Windows.Data;

namespace BookSearch.Screens.Main
{
    public class IsBusyOpacityConverter : IValueConverter
    {
        public double BusyOpacity { get; set; }

        public object Convert(object value, Type target_type, object parameter, CultureInfo culture)
        {
            return (bool) value ? BusyOpacity : 1;
        }

        public object ConvertBack(object value, Type target_type, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
