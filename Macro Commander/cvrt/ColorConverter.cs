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
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is ActionType action)
            {
                switch (action)
                {
                    case ActionType.MouseLeftButtonClick:
                        return "LimeGreen";
                    case ActionType.MouseRightButtonClick:
                        return "Magenta";
                    case ActionType.MouseMove:
                        return "Aqua";
                }
            }
            throw new Exception();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
