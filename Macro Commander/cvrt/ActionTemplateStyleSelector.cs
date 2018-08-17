using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Macro_Commander
{
    class ActionTemplateStyleSelector : StyleSelector
    {
        public Style TemplatePlaceHolderStyle { get; set; }
        public Style TemplateOriginStyle { get; set; }
        public override Style SelectStyle(object item, DependencyObject container)
        {
            return (item as src.ActionTemplate).PlaceHolder == true ? TemplatePlaceHolderStyle : TemplateOriginStyle;
        }
    }
}
