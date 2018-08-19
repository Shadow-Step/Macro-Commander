using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string Modifier { get; set; }
        public int Id { get; set; }

        //Constructor
        private HotKey()
        {
            
        }

        //Methods
        public static HotKey CreateHotKey(enu.HotKeyStatus status,string key,string modifier = null)
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
                    throw new Exception();
            }
            if(key != null)
            WinWrapper.RegisterKey(temp);
            return temp;
        }
    }
}
