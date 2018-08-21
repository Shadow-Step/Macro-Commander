using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro_Commander.src;
using Macro_Commander.enu;

namespace Macro_Commander.res
{
    public static class Statics
    {
        public static Dictionary<ActionType, string> ActionDictionary = new Dictionary<ActionType, string>()
        {
            {ActionType.MouseLeftButtonClick,"LeftClick"},
            {ActionType.MouseRightButtonClick, "RightClick"},
            {ActionType.MouseMove, "MouseMove"}
        };
        public static List<enu.ActionType> ActionTypes = new List<ActionType>() { ActionType.MouseLeftButtonClick, ActionType.MouseRightButtonClick, ActionType.MouseMove };
        public static List<enu.ExecutionMode> ExecutionModes { get; set; } = new List<ExecutionMode>() { enu.ExecutionMode.Loop, enu.ExecutionMode.Single };
        
    }
}
