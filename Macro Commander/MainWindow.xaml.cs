using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using Macro_Commander.src;
using System.Windows.Interop;
using System.Threading;
using System.Drawing;

namespace Macro_Commander
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IntPtr hWnd = IntPtr.Zero;
        HwndSource source;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = ViewModel.viewModel;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            hWnd = new WindowInteropHelper(this).Handle;
            source = HwndSource.FromHwnd(hWnd);
            source.AddHook(MsgListener);
            WinWrapper.hWnd = hWnd;

            WinWrapper.RegisterKey("F1", HotKeyStatus.DoubleClick); // temp!!!
            WinWrapper.RegisterKey("F2", HotKeyStatus.ShortClick); // temp!!!
            WinWrapper.RegisterKey("F3", HotKeyStatus.LongClick); // temp!!!
            WinWrapper.RegisterKey("F4", HotKeyStatus.Pause); // temp!!!
            WinWrapper.RegisterKey("F5", HotKeyStatus.Start); // temp!!!
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WinWrapper.UnregisterKey(0);
        }

        private IntPtr MsgListener(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    var l = lParam.ToInt32();
                    var w = wParam.ToInt32();
                    foreach (var item in WinWrapper.HotKeys)
                    {
                        var value = WinWrapper.KeyDict[item.Key];
                        var longcode = WinWrapper.VirtualKeyCodes[value];
                        if(l == value || l == longcode || w == value || w == longcode)
                        {
                            var x = System.Windows.Forms.Cursor.Position;
                            switch (item.Status)
                            {
                                case HotKeyStatus.ShortClick:
                                    WinWrapper.Click((uint)x.X, (uint)x.Y);
                                    ViewModel.viewModel.SelectedMacro?.CommandAddAction.Execute(new ActionMeta((uint)x.X,(uint)x.Y,100,ActionType.Click,ScreenCapture.CaptureFromScreen(64,48,x.X,x.Y)));
                                    break;
                                case HotKeyStatus.LongClick:
                                    WinWrapper.Click((uint)x.X, (uint)x.Y);
                                    ViewModel.viewModel.SelectedMacro?.CommandAddAction.Execute(new ActionMeta((uint)x.X, (uint)x.Y, 1000, ActionType.Click, ScreenCapture.CaptureFromScreen(64, 48, x.X, x.Y)));
                                    break;
                                case HotKeyStatus.Pause:
                                    ViewModel.viewModel.SelectedMacro?.CommandAddAction.Execute(new ActionMeta((uint)x.X, (uint)x.Y, 3000, ActionType.Pause));
                                    break;
                                case HotKeyStatus.Start:
                                    ViewModel.viewModel.SelectedMacro.CommandStart.Execute(0);
                                    break;
                                case HotKeyStatus.DoubleClick:
                                    ViewModel.viewModel.SelectedMacro.CommandAddAction.Execute(new ActionMeta((uint)x.X, (uint)x.Y, 500, ActionType.DoubleClick, ScreenCapture.CaptureFromScreen(64, 48, x.X, x.Y)));
                                    WinWrapper.Click((uint)x.X, (uint)x.Y);
                                    WinWrapper.Click((uint)x.X, (uint)x.Y);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }
                    }
                    
                    
                    
                    break;
                default:
                    break;
            }

            return IntPtr.Zero;
        }

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var x = 5;
            var p = ViewModel.viewModel.SelectedMacro.SelectedAction;
        }

        private void ListBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Delete)
            {
                ViewModel.viewModel.SelectedMacro.CommandDelAction.Execute(ViewModel.viewModel.SelectedMacro.SelectedAction);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.viewModel.LoadFromFile();
        }
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            ViewModel.viewModel.SaveToFile();
        }
    }
}
