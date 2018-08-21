#define DEBUGLOG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro_Commander.src
{
    [Serializable]
    public class ActionTemplate : INotifyWrapper
    {
        //Fields
        private enu.ActionType _actionType;
        private double _pause;
        private HotKey _hotKey;
        private int _clicks;
        private bool _placeHolder;
        private bool _editingMode;
        //Properties
        public enu.ActionType ActionType
        {
            get { return _actionType; }
            set
            {
                _actionType = value;
                PropChanged("ActionType");
            }
        }
        public double Pause
        {
            get { return +_pause; }
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
        public HotKey HotKey
        {
            get { return _hotKey; }
            set
            {
                if(_hotKey != null)
                WinWrapper.UnregisterKey(_hotKey);
                _hotKey = value;
                PropChanged("HotKey");
            }
        }
        public bool PlaceHolder
        {
            get { return _placeHolder; }
            set
            {
                _placeHolder = value;
                PropChanged("PlaceHolder");
            }
        }
        public bool EditingMode
        {
            get { return _editingMode; }
            set
            {
                _editingMode = value;
                PropChanged("EditingMode");
            }
        }
        //Constructor
        public ActionTemplate()
        {
            Pause = 0;
            Clicks = 0;
            PlaceHolder = false;
            ActionType = enu.ActionType.MouseLeftButtonClick;
        }
        public ActionTemplate(HotKey hotkey, double pause, enu.ActionType actionType,int clicks)
        {
            HotKey = hotkey;
            Pause = pause;
            ActionType = actionType;
            Clicks = clicks;
        }
        //Factory
        public static ActionTemplate GetPlaceHolder()
        {
            ActionTemplate temp = new ActionTemplate()
            {
                HotKey = null,
                PlaceHolder = true,
                ActionType = enu.ActionType.MouseMove,
                Clicks = 0,
                Pause = 0
            };
            return temp;
        }
        //Methods
    }
}
