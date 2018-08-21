using System;
using System.Globalization;
using System.Windows.Data;

namespace Macro_Commander
{
    class VisibilityNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse(parameter as string) == 0 ? (value == null ? "Collapsed" : "Visible") : (value == null ? "Visible" : "Collapsed");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
