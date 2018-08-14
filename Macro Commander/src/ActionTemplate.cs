using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro_Commander.src
{
    public class ActionTemplate : INotifyWrapper
    {
        //Fields
        private enu.ActionType _actionType;
        private int _pause;
        private string _hotKey;
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
        public string HotKey
        {
            get { return _hotKey; }
            set
            {
                _hotKey = value;
                PropChanged("HotKey");
            }
        }
        //Constructor
        public ActionTemplate(string hotkey, int pause, enu.ActionType actionType)
        {
            HotKey = hotkey;
            Pause = pause;
            ActionType = actionType;
        }
    }
}
