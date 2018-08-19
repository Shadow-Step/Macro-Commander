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
using Macro_Commander.enu;

namespace Macro_Commander
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Key[] KeyList = new Key[2] { Key.None,Key.None};
        //Fields
        IntPtr hWnd = IntPtr.Zero;
        HwndSource source;

        //Window
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            hWnd = new WindowInteropHelper(this).Handle;
            source = HwndSource.FromHwnd(hWnd);
            source.AddHook(MsgListener);
            WinWrapper.hWnd = hWnd;
            this.DataContext = ViewModel.viewModel;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WinWrapper.UnregisterAll();
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
                    foreach (var key in WinWrapper.HotKeys)
                    {
                        int shortcode = WinWrapper.KeyDict[key.Key];
                        int longcode = WinWrapper.VirtualKeyCodes[shortcode];
                        if(l == shortcode || w == shortcode || l == longcode || w == longcode)
                        {
                            switch (key.KeyStatus)
                            {
                                case HotKeyStatus.AddAction:
                                    foreach (var template in ViewModel.viewModel.ActionTemplates)
                                    {
                                        if(template.HotKey == key)
                                        {
                                            var pos = System.Windows.Forms.Cursor.Position;
                                            ViewModel.viewModel.SelectedMacro?.CommandAddAction.Execute(new ActionMeta((uint)pos.X,(uint)pos.Y, template, ScreenCapture.CaptureFromScreen(64, 48, pos.X, pos.Y)));
                                        }
                                    }
                                    break;
                                case HotKeyStatus.ExecuteScenario:
                                    foreach (var scen in ViewModel.viewModel.Scenarios)
                                    {
                                        if (scen.HotKey.Key == key.Key)
                                        {
                                            ViewModel.viewModel.SelectedScenario = scen;
                                            ViewModel.viewModel.CommandExecuteScenarioAsync.Execute(null);
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            return IntPtr.Zero;
        }
        
        //Events
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
        private void TemplateItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (((sender as ListBoxItem).Content as ActionTemplate).PlaceHolder)
            {
                ViewModel.viewModel.CommandAddTemplate.Execute(null);
            }
            else
                ViewModel.viewModel.SelectedTemplate.EditingMode = true;
        }

        private void ScenarioHotKeyInit(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Keyboard.ClearFocus();
            if(ViewModel.viewModel.SelectedScenario != null)
            ViewModel.viewModel.SelectedScenario.HotKey = HotKey.CreateHotKey(HotKeyStatus.ExecuteScenario, e.Key.ToString());
        }
        private void ActionTemplateHotKeyInit(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Keyboard.ClearFocus();
            if (ViewModel.viewModel.SelectedTemplate != null)
                ViewModel.viewModel.SelectedTemplate.HotKey = KeyList[1] == Key.None ? 
                    HotKey.CreateHotKey(HotKeyStatus.AddAction, KeyList[0].ToString()) : 
                    HotKey.CreateHotKey(HotKeyStatus.AddAction, KeyList[1].ToString(), KeyList[0].ToString());
        }
        private void ActionTemplateHotKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (KeyList[0] == Key.None)
                KeyList[0] = e.Key;
            else
                KeyList[1] = e.Key;
        }
        private void AcceptActionTemplateChanges(object sender, RoutedEventArgs e)
        {
            ViewModel.viewModel.CommandStartStopEditTemplate.Execute(null);
        }

        private void MenuNewClick(object sender, RoutedEventArgs e)
        {

        }
        private void MenuOpenClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Filter = "Project file (*.mcp)|*mcp";
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ViewModel.viewModel.CommandLoadFromFile.Execute(fileDialog.FileName);
            }

        }
        private void MenuSaveClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.viewModel.ProjectPath == null)
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.RestoreDirectory = true;
                saveFile.Filter = "Project file (*.mcp)|*mcp";
                saveFile.DefaultExt = ".mcp";
                if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ViewModel.viewModel.CommandSaveToFile.Execute(saveFile.FileName);
                }
            }
            else
            {
                ViewModel.viewModel.CommandSaveToFile.Execute(ViewModel.viewModel.ProjectPath);
            }

        }
        private void MenuSaveAsClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.RestoreDirectory = true;
            saveFile.Filter = "Project file (*.mcp)|*mcp";
            saveFile.DefaultExt = ".mcp";
            if (saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ViewModel.viewModel.CommandSaveToFile.Execute(saveFile.FileName);
            }
        }
    }
}
