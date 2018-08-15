using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Macro_Commander.enu;

namespace Macro_Commander.src
{
    [Serializable]
    public class Action : INotifyWrapper
    {
        //Fields
        private UInt32 _x = 0;
        private UInt32 _y = 0;
        private int _pause = 0;
        private Bitmap _image;
        private ActionType _action_type;

        //Properties
        public UInt32 X
        {
            get { return _x; }
            set
            {
                _x = value;
                PropChanged("X");
            }
        }
        public UInt32 Y
        {
            get { return _y; }
            set
            {
                _y = value;
                PropChanged("Y");
            }
        }
        public Bitmap Image
        {
            get { return _image; }
            set
            {
                _image = value;
                PropChanged("Image");
            }
        }
        public ActionType ActionType
        {
            get { return _action_type; }
            set
            {
                _action_type = value;
                PropChanged("ActionType");
            }
        }
        public int Pause
        {
            get { return _pause; }
            set
            {
                _pause = value;
                PropChanged("Pause");
            }
        }
        
        //Constructors
        public Action()
        {

        }
        public Action(UInt32 x, UInt32 y)
        {
            X = x;
            Y = y;
        }
        public Action(ActionMeta meta)
        {
            X = meta.X;
            Y = meta.Y;
            Pause = meta.Pause;
            ActionType = meta.ActionType;
            Image = meta.Bitmap;
        }
        public Action(ActionTemplate template, UInt32 x, UInt32 y, Bitmap image) : base()
        {
            ActionType = template.ActionType;
            Pause = template.Pause;
            X = x;
            Y = y;
            Image = image;
        }

        //Methods
        public void Execute()
        {
            switch (ActionType)
            {
                case ActionType.Click:
                    WinWrapper.Click(X, Y);
                    break;
                case ActionType.DoubleClick:
                    WinWrapper.Click(X, Y);
                    WinWrapper.Click(X, Y);
                    break;
                case ActionType.Pause:
                    break;
                default:
                    break;
            }
        }
    }
}
