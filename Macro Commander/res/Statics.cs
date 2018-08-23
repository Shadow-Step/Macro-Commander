using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macro_Commander.src;
using Macro_Commander.enu;
using System.Windows.Media;

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
        public static Dictionary<string, byte[]> GroupsColors { get; set; } = new Dictionary<string, byte[]>();
        public static List<byte[]> FixedColors { get; set; } = new List<byte[]>()
        {
            GetBytesFromColor(Colors.Magenta),
            GetBytesFromColor(Colors.Cyan),
            GetBytesFromColor(Colors.Blue),
            GetBytesFromColor(Colors.Yellow)
        };
        public static void AddGroupToColorDictionary(string group_name)
        {
            
            if (FixedColors.Count > 0)
            {
                GroupsColors.Add(group_name, FixedColors.Last());
                FixedColors.Remove(FixedColors.Last());
            }
            else
            {
                Random rand = new Random();
                byte[] buffer = new byte[3];
                rand.NextBytes(buffer);
                GroupsColors.Add(group_name, buffer);
            }
            
        }
        public static void RemoveGroupFromColorDictionary(string group_name)
        {
            var color = GroupsColors[group_name];
            GroupsColors.Remove(group_name);
            FixedColors.Add(color);
        }
        public static byte[] GetBytesFromColor(Color color)
        {
            byte[] buffer = new byte[3];
            buffer[0] = color.R;
            buffer[1] = color.G;
            buffer[2] = color.B;
            return buffer;
        }
    }
}
