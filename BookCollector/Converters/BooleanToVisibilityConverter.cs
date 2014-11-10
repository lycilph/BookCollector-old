using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BookCollector.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public Visibility TrueState { get; set; }
        public Visibility FalseState { get; set; }

        public BooleanToVisibilityConverter()
        {
            TrueState = Visibility.Visible;
            FalseState = Visibility.Collapsed;
        }
      
        public object Convert(object value, Type target_type, object parameter, CultureInfo culture)
        {
            return (bool) value ? TrueState : FalseState;
        }

        public object ConvertBack(object value, Type target_type, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
