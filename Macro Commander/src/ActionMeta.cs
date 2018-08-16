using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Macro_Commander.enu;

namespace Macro_Commander.src
{
    public class ActionMeta
    {
        public UInt32 X;
        public UInt32 Y;
        public Bitmap Bitmap;
        public ActionTemplate Template;

        public ActionMeta(UInt32 x, UInt32 y, ActionTemplate template, Bitmap bitmap = null)
        {
            this.X = x;
            this.Y = y;
            this.Bitmap = bitmap;
            this.Template = template;
            
        }
    }
}
