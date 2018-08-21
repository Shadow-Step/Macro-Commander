#define DEBUGLOG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Macro_Commander.enu;

namespace Macro_Commander.src
{
    [Serializable]
    public class HotKey
    {
        [NonSerialized]
        public static SortedSet<int> IdSet = new SortedSet<int>();
        //Fields

        //Properties
        public enu.HotKeyStatus KeyStatus { get; set; }
        public string Key { get; set; }
        public Key Modifier { get; set; }
        public int Id { get; set; }
        public string StringModifier
        {
            get
            {
                switch(Modifier)
                {
                    case System.Windows.Input.Key.LeftAlt:
                    case System.Windows.Input.Key.RightAlt:
                        return "ALT";
                    case System.Windows.Input.Key.LeftCtrl:
                    case System.Windows.Input.Key.RightCtrl:
                        return "CTRL";
                    case System.Windows.Input.Key.LeftShift:
                    case System.Windows.Input.Key.RightShift:
                        return "SHIFT";
                    default:
                        return null;
                }
            }
        }
        //Constructor
        private HotKey()
        {
            
        }

        //Methods
        public static HotKey CreateHotKey(enu.HotKeyStatus status,string key, System.Windows.Input.Key modifier = System.Windows.Input.Key.None)
        {
            HotKey temp = new HotKey();
            temp.KeyStatus = status;
            temp.Key = key;
            temp.Modifier = modifier;
            for (int i = 0; i < 1000; i++)
            {
                if (!IdSet.Contains(i))
                {
                    IdSet.Add(i);
                    temp.Id = i;
                    break;
                }
                if (i == 999)
                {
#if DEBUGLOG
                    Logger.GetLogger().WriteToLog($"HotKey: CreateHotKey : Exception{{ID limit}} : Code{{{0}}}");
#endif
                    throw new Exception();
                }
            }
            if (key != null)
            WinWrapper.RegisterKey(temp);
            return temp;
        }
    }
}
