using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Macro_Commander
{
    class GroupColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string group = value as string;
            if (group == null || group == string.Empty)
                return "Transparent";
            else
            {
                if ((parameter as string) == "Stroke")
                    return "Black";
                byte[] b = res.Statics.GroupsColors[group];
                return new SolidColorBrush(Color.FromRgb(b[0],b[1],b[2]));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
