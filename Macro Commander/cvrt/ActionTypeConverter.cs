using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Macro_Commander.src;
using Macro_Commander.enu;

namespace Macro_Commander
{
    class ActionTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is ActionType)
            {
                if (Macro_Commander.res.Statics.ActionDictionary.ContainsKey((ActionType)value))
                    return Macro_Commander.res.Statics.ActionDictionary[(ActionType)value];
                else
                    throw new Exception();
            }
            throw new Exception();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string action))
                throw new Exception();
            else
                return res.Statics.ActionDictionary.First(x => x.Value == action).Key;
            throw new Exception();
        }
    }
}
