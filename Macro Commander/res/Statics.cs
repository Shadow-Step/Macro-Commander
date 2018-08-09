using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro_Commander.src;

namespace Macro_Commander.res
{
    public static class Statics
    {
        public static Dictionary<ActionType, string> ActionDictionary = new Dictionary<ActionType, string>()
        {
            {ActionType.Click,"!!!Click"},
            {ActionType.DoubleClick, "!!!DoubleClick"},
            {ActionType.Pause, "!!!Pause"}
        };
        
    }
}
