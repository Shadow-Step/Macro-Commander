#define DEBUGLOG
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
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                Logger.GetLogger().CatchException("MainWindow", "Constructor", e.Message);
                throw;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                hWnd = new WindowInteropHelper(this).Handle;
                source = HwndSource.FromHwnd(hWnd);
                source.AddHook(MsgListener);
                WinWrapper.hWnd = hWnd;
                this.DataContext = ViewModel.viewModel;
            }
            catch (Exception ex)
            {
                Logger.GetLogger().CatchException("MainWindow", "Window_Loaded", ex.Message);
                throw;
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                WinWrapper.UnregisterAll();
            }
            catch (Exception ex)
            {
                Logger.GetLogger().CatchException("MainWindow", "Window_Closing",ex.Message);
                throw;
            }
            
        }

        //Methods

        //Global message listener
        private IntPtr MsgListener(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            try
            {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    var l = lParam.ToInt32();
                    var w = wParam.ToInt32();
#if DEBUGLOG
                    short code = 0;
                    string command = "null";
#endif
                    foreach (var key in WinWrapper.HotKeys)
                    {
                        int shortcode = WinWrapper.KeyDict[key.Key]; 
                        int longcode = WinWrapper.VirtualKeyCodes[shortcode] + (key.StringModifier == null ? 0 : WinWrapper.KeyDict[key.StringModifier]);
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
                                            ViewModel.viewModel.SelectedMacro?.CommandAddAction.Execute(new ActionMeta((uint)pos.X,(uint)pos.Y, template, ScreenCapture.CaptureFromScreen(64, 64, pos.X, pos.Y)));
#if DEBUGLOG
                                            command = "CommandAddAction";
                                            code = 1;
#endif
                                        }
                                    }
                                    break;
                                case HotKeyStatus.ExecuteScenario:
                                    foreach (var scen in ViewModel.viewModel.Scenarios)
                                    {
                                        if (scen.HotKey.Key == key.Key)
                                        {
                                            if(ViewModel.viewModel.ExecutionStarted)
                                                    ViewModel.viewModel.CommandExecuteScenarioAsync.Execute(null);
                                            ViewModel.viewModel.SelectedScenario = scen;
                                            ViewModel.viewModel.CommandExecuteScenarioAsync.Execute(null);
#if DEBUGLOG
                                            command = "CommandExecuteScenarioAsync";
                                            code = 1;
#endif
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }

                        }
                    }
#if DEBUGLOG
                    Logger.GetLogger().WriteToLog($"MsgListener: MessageReceived: Msg{{{WM_HOTKEY}}}, l{{{l}}}, w{{{w}}} : Command{{{command}}}, Code{{{code}}}");
#endif
                    break;
                default:
                    break;
            }
            }
            catch (Exception ex)
            {
                Logger.GetLogger().CatchException("MainWindow", "MsgListener", ex.Message);
                throw;
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
                ViewModel.viewModel.CommandAddItemToList.Execute("ActionTemplate");
            }
            else
                ViewModel.viewModel.SelectedTemplate.EditingMode = true;
        }
                
        //Menu
        private void MenuNewClick(object sender, RoutedEventArgs e)
        {
            if(System.Windows.MessageBox.Show("message") == MessageBoxResult.OK)
            {
                ViewModel.viewModel.CommandNewProject.Execute(null);
            }
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

        //Windows
        private void ShowHelpWindow(object sender, RoutedEventArgs e)
        {
            win.HelpWindow window = new win.HelpWindow();
            window.Show();
        }

        //Keyboard and Focus events
        private void MacrosListBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (ViewModel.viewModel.SelectedMacro != null && ViewModel.viewModel.SelectedMacro.EditingMode == false)
                    ViewModel.viewModel.CommandRemoveItemFromList.Execute(ViewModel.viewModel.SelectedMacro);
            }
        }

        private void HotKeyTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    KeyList[0] = e.Key;
                    break;
                default:
                    KeyList[1] = e.Key;
                    break;
            }
        }
        private void HotKeyTextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (KeyList[1] == Key.None)
                return;
            if (!WinWrapper.KeyDict.ContainsKey(e.Key.ToString()))
            {
                System.Windows.MessageBox.Show($"Unsupported HotKey - {e.Key.ToString()}");
                return;
            }
            if (WinWrapper.HotKeys.Count(x => x.Key == e.Key.ToString()) > 0)
            {
                System.Windows.MessageBox.Show($"HotKey already registered - {e.Key.ToString()}");
                return;
            }

            Keyboard.ClearFocus();
            switch ((sender as System.Windows.Controls.TextBox).Name)
            {
                case "ScenarioHotKeyTextBox":
                    if (ViewModel.viewModel.SelectedScenario != null)
                        ViewModel.viewModel.SelectedScenario.HotKey = HotKey.CreateHotKey(HotKeyStatus.ExecuteScenario, KeyList[1].ToString(), KeyList[0]);
                    break;
                case "TemplateHotKeyTextBox":
                    if (ViewModel.viewModel.SelectedTemplate != null)
                        ViewModel.viewModel.SelectedTemplate.HotKey = HotKey.CreateHotKey(HotKeyStatus.AddAction, KeyList[1].ToString(), KeyList[0]);
                    break;
                default:
                    Logger.GetLogger().CatchException("MainWindow", "HotKeyTextBox_KeyUp", "Unknown TextBox.Name");
                    throw new Exception();
            }
            KeyList[0] = KeyList[1] = Key.None;
        }

        private void EditNameTextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if(ViewModel.viewModel.SelectedScenario != null && ViewModel.viewModel.SelectedScenario.EditingMode)
                    ViewModel.viewModel.SelectedScenario.Name = (sender as System.Windows.Controls.TextBox).Text;
                else if(ViewModel.viewModel.SelectedMacro != null && ViewModel.viewModel.SelectedMacro.EditingMode)
                    ViewModel.viewModel.SelectedMacro.Name = (sender as System.Windows.Controls.TextBox).Text;
            }
        }
        private void EditNameTextBox_LostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            if (ViewModel.viewModel.SelectedScenario != null && ViewModel.viewModel.SelectedScenario.EditingMode)
                ViewModel.viewModel.SelectedScenario.Name = (sender as System.Windows.Controls.TextBox).Text;
            else if (ViewModel.viewModel.SelectedMacro != null && ViewModel.viewModel.SelectedMacro.EditingMode)
                ViewModel.viewModel.SelectedMacro.Name = (sender as System.Windows.Controls.TextBox).Text;
        }
        private void EditNameTextBox_Initialized(object sender, EventArgs e)
        {
            var box = sender as System.Windows.Controls.TextBox;
            box.Focus();
            box.SelectAll();
        }
        
       
        //Other events
        private void AcceptActionTemplateChanges(object sender, RoutedEventArgs e)
        {
            ViewModel.viewModel.CommandEditItem.Execute(ViewModel.viewModel.SelectedTemplate);
        }

    }
}
