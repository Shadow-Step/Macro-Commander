using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Macro_Commander
{
    class HotKeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var key = value as src.HotKey;
            return key == null ? null : key.Key;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var key = value as string;
            var param = int.Parse(parameter as string);
            if (key == null)
                throw new Exception();
            return param == 0 ? src.HotKey.CreateHotKey(key, enu.HotKeyStatus.AddAction) : src.HotKey.CreateHotKey(key, enu.HotKeyStatus.ExecuteScenario);
        }
    }
}
