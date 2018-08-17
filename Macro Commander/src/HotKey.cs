using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro_Commander.enu;

namespace Macro_Commander.src
{
    public class HotKey
    {
        private static int ID;
        //Fields

        //Properties
        public enu.HotKeyStatus KeyStatus { get; set; }
        public string Key { get; set; }
        public int Id { get; set; }

        //Constructor
        private HotKey()
        {
            
        }

        //Methods
        public static HotKey CreateHotKey(string key, enu.HotKeyStatus status)
        {
            HotKey temp = new HotKey();
            temp.KeyStatus = status;
            temp.Key = key;
            temp.Id = ID++;
            if(key != null)
            WinWrapper.RegisterKey(temp);
            return temp;
        }
    }
}
