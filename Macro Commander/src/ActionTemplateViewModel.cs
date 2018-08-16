using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro_Commander.src
{
    public class ActionTemplateViewModel : INotifyWrapper
    {
        //Fields

        //Properties
        public enu.ActionType ActionType { get; set; }
        public HotKey HotKey { get; set; }
        public int Pause { get; set; }
        public int Times { get; set; }
        //Constructor

        //Methods

    }
}
