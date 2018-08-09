using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Macro_Commander.src
{
    public class ActionMeta
    {
        public UInt32 X;
        public UInt32 Y;
        public int RestTime;
        public ActionType ActionType;
        public Bitmap Bitmap;

        public ActionMeta(UInt32 x, UInt32 y, int RestTime, ActionType type, Bitmap bitmap = null)
        {
            this.X = x;
            this.Y = y;
            this.RestTime = RestTime;
            this.ActionType = type;
            this.Bitmap = bitmap;
        }
    }
}
