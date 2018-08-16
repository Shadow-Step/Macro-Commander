﻿using System;
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
using Macro_Commander.enu;

namespace Macro_Commander
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Fields
        IntPtr hWnd = IntPtr.Zero;
        HwndSource source;

        //Window
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

        //Methods

        //Global message listener
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
                        if (l == value || l == longcode || w == value || w == longcode)
                        {
                            var x = System.Windows.Forms.Cursor.Position;
                            switch (item.Status)
                            {
                                case HotKeyStatus.ShortClick:
                                    //WinWrapper.Click((uint)x.X, (uint)x.Y);
                                    ViewModel.viewModel.SelectedMacro?.CommandAddAction.Execute(new ActionMeta((uint)x.X, (uint)x.Y, 100, ActionType.LeftClick, 1, ScreenCapture.CaptureFromScreen(64, 48, x.X, x.Y)));
                                    break;
                                case HotKeyStatus.LongClick:
                                    WinWrapper.Click((uint)x.X, (uint)x.Y);
                                    ViewModel.viewModel.SelectedMacro?.CommandAddAction.Execute(new ActionMeta((uint)x.X, (uint)x.Y, 1000, ActionType.LeftClick, 1, ScreenCapture.CaptureFromScreen(64, 48, x.X, x.Y)));
                                    break;
                                case HotKeyStatus.Pause:
                                    ViewModel.viewModel.SelectedMacro?.CommandAddAction.Execute(new ActionMeta((uint)x.X, (uint)x.Y, 3000, ActionType.Pause, 0, ScreenCapture.CaptureFromScreen(64, 48, 0, 0, enu.CaptureMode.EmptyImage)));
                                    break;
                                case HotKeyStatus.Start:
                                    if(ViewModel.viewModel.CommandExecuteScenarioAsync.CanExecute(null))
                                    ViewModel.viewModel.CommandExecuteScenarioAsync.Execute(null);
                                    break;
                                case HotKeyStatus.DoubleClick:
                                    ViewModel.viewModel.SelectedMacro.CommandAddAction.Execute(new ActionMeta((uint)x.X, (uint)x.Y, 500, ActionType.LeftClick, 2, ScreenCapture.CaptureFromScreen(64, 48, x.X, x.Y)));
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

        private void AvailableMacros_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            Macro item = row.Item as Macro;
            if(ViewModel.viewModel.SelectedScenario.CommandAddMacro.CanExecute(item))
            ViewModel.viewModel.SelectedScenario.CommandAddMacro.Execute(item);
        }
        private void ScenarioMacros_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            Macro item = row.Item as Macro;
            if (ViewModel.viewModel.SelectedScenario.CommandDelMacro.CanExecute(item))
                ViewModel.viewModel.SelectedScenario.CommandDelMacro.Execute(item);
        }
    }
}
