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
        };
        public static Dictionary<int, int> VirtualKeyCodes { get; set; } = new Dictionary<int, int>()
        {
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
        private static UInt32 ABSOLUTE_FLAG = 0x8000;
        private static UInt32 MOUSE_MOVE = 0x0001;
        private static UInt32 MOUSE_BUTTONDOWN = 0x0002;
        private static UInt32 MOUSE_BUTTONUP = 0x0004;
        private static UInt32 WHEEL_ROTATE = 0x0800;

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


        public static void Click(uint x, uint y)
        {
            mouse_event(ABSOLUTE_FLAG | MOUSE_MOVE, (uint)(x  * (ABSOLUTE / SCREEN_WIDTH)) + 1, (uint)(y * (ABSOLUTE / SCREEN_HEIGHT)) + 1, 0, IntPtr.Zero);
            mouse_event(MOUSE_BUTTONDOWN, 0, 0, 0, IntPtr.Zero);
            Thread.Sleep(CLICK_SENSITIVITY);
            mouse_event(MOUSE_BUTTONUP, 0, 0, 0, IntPtr.Zero);
        }

        public static void RegisterKey(string key)
        {
            RegisterHotKey(hWnd, 0, 0, KeyDict[key]);
        }
        public static void RegisterKey(string Key,HotKeyStatus status)
        {
            var x = RegisterHotKey(hWnd, 0, 0, KeyDict[Key]);
            HotKeys.Add(new HotKey(Key, 0, status));
        }
        public static void UnregisterKey(int id)
        {
            if (hWnd != null)
                UnregisterHotKey(hWnd, id);
            else
                throw new Exception("hWnd is null");

        }
    }
}
