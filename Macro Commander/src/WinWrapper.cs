using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Macro_Commander.enu;

namespace Macro_Commander.src
{
    public static class WinWrapper
    {
        public static List<HotKey> HotKeys = new List<HotKey>();

        public static int CLICK_SENSITIVITY = 50;
        public static Dictionary<string, int> KeyDict { get; set; } = new Dictionary<string, int>()
        {
            {"A",0x0041 },
            {"B",0x0042 },
            {"C",0x0043 },
            {"D",0x0044 },
            {"E",0x0045 },
            {"F",0x0046 },
            {"G",0x0047 },
            {"H",0x0048 },
            {"I",0x0049 },
            {"J",0x004A },
            {"K",0x004B },
            {"L",0x004C },
            {"M",0x004D },
            {"N",0x004E },
            {"O",0x004F },
            {"P",0x0050 },
            {"Q",0x0051 },
            {"R",0x0052 },
            {"S",0x0053 },
            {"T",0x0054 },
            {"U",0x0055 },
            {"V",0x0056 },
            {"W",0x0057 },
            {"X",0x0058 },
            {"Y",0x0059 },
            {"Z",0x005A },
            {"F1", 0x0070},
            {"F2", 0x0071},
            {"F3", 0x0072},
            {"F4", 0x0073},
            {"F5", 0x0074},
            {"F6", 0x0075},
            {"F7", 0x0076},
            {"F8", 0x0077},
            {"F9", 0x0078},
            {"F10", 0x0079},
            {"F11", 0x007A},
            {"ALT", 0x0001 },
            {"CTRL", 0x0002 },
            {"SHIFT", 0x0004 },
        };
        public static Dictionary<int, int> VirtualKeyCodes { get; set; } = new Dictionary<int, int>()
        {
            {0x0041,0x00410000},
            {0x0042,0x00420000},
            {0x0043,0x00430000},
            {0x0044,0x00440000},
            {0x0045,0x00450000},
            {0x0046,0x00460000},
            {0x0047,0x00470000},
            {0x0048,0x00480000},
            {0x0049,0x00490000},
            {0x004A,0x004A0000},
            {0x004B,0x004B0000},
            {0x004C,0x004C0000},
            {0x004D,0x004D0000},
            {0x004E,0x004E0000},
            {0x004F,0x004F0000},
            {0x0050,0x00500000},
            {0x0051,0x00510000},
            {0x0052,0x00520000},
            {0x0053,0x00530000},
            {0x0054,0x00540000},
            {0x0055,0x00550000},
            {0x0056,0x00560000},
            {0x0057,0x00570000},
            {0x0058,0x00580000},
            {0x0059,0x00590000},
            {0x005A,0x005A0000},
            {0x0070,0x00700000},
            {0x0071,0x00710000},
            {0x0072,0x00720000},
            {0x0073,0x00730000},
            {0x0074,0x00740000},
            {0x0075,0x00750000},
            {0x0076,0x00760000},
            {0x0077,0x00770000},
            {0x0078,0x00780000},
            {0x0079,0x00790000},
            {0x007A,0x007A0000},
            {0x007B,0x007B0000}
        };

        private const double ABSOLUTE = 65535;
        private static UInt32 MOUSEEVENTF_ABSOLUTE = 0x8000;
        private static UInt32 MOUSEEVENTF_MOVE = 0x0001;
        private static UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private static UInt32 MOUSEEVENTF_LEFTUP = 0x0004;
        private static UInt32 MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private static UInt32 MOUSEEVENTF_RIGHTUP = 0x0010;

        private const double SCREEN_WIDTH = 1360;
        private const double SCREEN_HEIGHT = 768;


        public static IntPtr hWnd { get; set; }

        [DllImport("user32.dll")]
        public static extern void mouse_event(UInt32 dwFlags, UInt32 dx, UInt32 dy, int dwData, IntPtr dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(int hwndParent, int hwndEnfant, int lpClasse, string title);
        [DllImport("user32.dll")]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, int vk);
        [DllImport("user32.dll")]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref Point point);


        public static void MouseLeftButtonClick(uint x, uint y)
        {
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, (uint)(x  * (ABSOLUTE / SCREEN_WIDTH)) + 1, (uint)(y * (ABSOLUTE / SCREEN_HEIGHT)) + 1, 0, IntPtr.Zero);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, IntPtr.Zero);
            Thread.Sleep(CLICK_SENSITIVITY);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, IntPtr.Zero);
        }
        public static void MouseRightButtonClick(uint x, uint y)
        {
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, (uint)(x * (ABSOLUTE / SCREEN_WIDTH)) + 1, (uint)(y * (ABSOLUTE / SCREEN_HEIGHT)) + 1, 0, IntPtr.Zero);
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, IntPtr.Zero);
            Thread.Sleep(CLICK_SENSITIVITY);
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, IntPtr.Zero);
        }
        public static void MouseMove(uint x,uint y)
        {
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, (uint)(x * (ABSOLUTE / SCREEN_WIDTH)) + 1, (uint)(y * (ABSOLUTE / SCREEN_HEIGHT)) + 1, 0, IntPtr.Zero);
        }

        public static void RegisterKey(HotKey key)
        {
            if (key == null || key.Key == null)
                return;
            var exist = from k in HotKeys where k.Key == key.Key select k;
            if(exist.Count() > 0)
            {
                UnregisterKey(exist.First());
            }
            var result = RegisterHotKey(hWnd, key.Id, key.StringModifier == null ? 0 : (uint)KeyDict[key.StringModifier] , KeyDict[key.Key]);
            HotKeys.Add(key);
        }
        public static void UnregisterKey(HotKey key)
        {
            if (hWnd != null)
            {
                if (key == null)
                    return;
                var x = UnregisterHotKey(hWnd, key.Id);
                HotKeys.Remove(key);
                HotKey.IdSet.Remove(key.Id);
            }
            else
                throw new Exception("hWnd is null");

        }
        public static void UnregisterAll()
        {
            if (hWnd == null)
                throw new Exception();
            foreach (var item in HotKeys)
            {
                UnregisterHotKey(hWnd, item.Id);
            }
            HotKeys.Clear();
        }
    }
}
