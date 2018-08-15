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
        public Action<object> Action { get; set; }
        public string Key;
        public int Id;
        public HotKeyStatus Status;
        public Macro Macro;

        //Constructor
        public HotKey(string Key, int Id, HotKeyStatus Status)
        {
            this.Key = Key;
            this.Id = Id;
            this.Status = Status;
        }
        public HotKey(string Key,int Id, Macro Macro)
        {
            this.Key = Key;
            this.Id = Id;
            this.Macro = Macro;
        }
        public HotKey(string Key,Action<object>action)
        {

        }
        //Methods
    }
}
