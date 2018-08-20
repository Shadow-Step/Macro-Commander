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
        private int _pause;
        private HotKey _hotKey;
        private int _times;
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
        public int Pause
        {
            get { return +_pause; }
            set
            {
                _pause = value;
                PropChanged("Pause");
            }
        }
        public int Times
        {
            get { return _times; }
            set
            {
                _times = value;
                PropChanged("Times");
            }
        }
        public HotKey HotKey
        {
            get { return _hotKey; }
            set
            {
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
            Times = 0;
            HotKey = HotKey.CreateHotKey(enu.HotKeyStatus.AddAction, null);
            PlaceHolder = false;
            ActionType = enu.ActionType.MouseLeftButtonClick;
        }
        public ActionTemplate(HotKey hotkey, int pause, enu.ActionType actionType,int times)
        {
            HotKey = hotkey;
            Pause = pause;
            ActionType = actionType;
            Times = times;
        }
        
        //Methods
    }
}
