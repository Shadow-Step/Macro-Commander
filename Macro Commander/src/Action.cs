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
        private double _pause = 0;
        private Bitmap _image;
        private ActionType _action_type;
        private int _clicks;
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
        public double Pause
        {
            get { return _pause; }
            set
            {
                _pause = value;
                PropChanged("Pause");
            }
        }
        public int Clicks
        {
            get { return _clicks; }
            set
            {
                _clicks = value;
                PropChanged("Clicks");
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
            Image = meta.Bitmap;
            Pause = meta.Template.Pause;
            ActionType = meta.Template.ActionType;
            Clicks = meta.Template.Clicks;
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
                case ActionType.MouseLeftButtonClick:
                    for (int i = 0; i < Clicks; i++)
                        WinWrapper.MouseLeftButtonClick(X, Y);
                    break;
                case ActionType.MouseRightButtonClick:
                    for (int i = 0; i < Clicks; i++)
                        WinWrapper.MouseRightButtonClick(X, Y);
                    break;
                case ActionType.MouseMove:
                    WinWrapper.MouseMove(X,Y);
                    break;
                default:
                    break;
            }
        }
    }
}
