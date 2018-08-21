#define DEBUGLOG
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Macro_Commander.src
{
    [Serializable]
    class ViewModel : INotifyWrapper
    {
        //Singleton
        private static ViewModel _viewmodel;
        public static ViewModel viewModel
        {
            get
            {
                try
                {
                    return _viewmodel ?? (_viewmodel = new ViewModel());
                }
                catch (Exception e)
                {
                    Logger.GetLogger().CatchException("ViewModel","Property viewModel",e.Message);
                    throw;
                }
                
            }
        }
        //Fields
        private CancellationTokenSource _tokenSource;
        private ObservableCollection<Macro> _macroList;
        private ObservableCollection<Scenario> _scenarios;
        private ObservableCollection<ActionTemplate> _actionTemplates;
        private Macro _selectedMacro;
        private Scenario _selectedScenario;
        private ActionTemplate _selectedTemplate;
        private string _projectPath;
        private bool _executionStarted;
        
        //Properties
        public ObservableCollection<Macro> MacroList
        {
            get { return _macroList; }
            set
            {
                _macroList = value;
                PropChanged("MacroList");
            }
        }
        public ObservableCollection<Scenario> Scenarios
        {
            get { return _scenarios; }
            set
            {
                _scenarios = value;
                PropChanged("Scenarios");
            }
        }
        public ObservableCollection<ActionTemplate> ActionTemplates
        {
            get { return _actionTemplates; }
            set
            {
                _actionTemplates = value;
                PropChanged("ActionTemplates");
            }
        }
        public Macro SelectedMacro
        {
            get { return _selectedMacro; }
            set
            {
                if (_selectedMacro != null && _selectedMacro.EditingMode)
                    _selectedMacro.EditingMode = false;
                _selectedMacro = value;
                PropChanged("SelectedMacro");
            }
        }
        public Scenario SelectedScenario
        {
            get { return _selectedScenario; }
            set
            {
                if (_selectedScenario != null && _selectedScenario.EditingMode)
                    _selectedScenario.EditingMode = false;
                _selectedScenario = value;
                PropChanged("SelectedScenario");
            }
        }
        public string ProjectPath
        {
            get { return _projectPath; }
            set
            {
                _projectPath = value;
                PropChanged("ProjectPath");
            }
        }
        public bool ExecutionStarted
        {
            get { return _executionStarted; }
            set
            {
                _executionStarted = value;
                PropChanged("ExecutionStarted");
            }
        }
        public ActionTemplate SelectedTemplate
        {
            get { return _selectedTemplate; }
            set
            {
                if(SelectedTemplate != null && SelectedTemplate.EditingMode == true)
                _selectedTemplate.EditingMode = false;
                _selectedTemplate = value;
                PropChanged("SelectedTemplate");
            }
        }
        
        //Commands
        public RelayCommand CommandSaveToFile { get; set; }
        public RelayCommand CommandLoadFromFile { get; set; }
        public RelayCommand CommandExecuteScenarioAsync { get; set; }
        public RelayCommand CommandEditItem { get; set; }
        public RelayCommand CommandAddItemToList { get; set; }
        public RelayCommand CommandRemoveItemFromList { get; set; }
        //Constructor
        private ViewModel()
        {
            try
            {
                CommandSaveToFile = new RelayCommand(SaveToFile);
                CommandLoadFromFile = new RelayCommand(LoadFromFile);
                CommandExecuteScenarioAsync = new RelayCommand(ExecuteScenarioAsync, (param) => SelectedScenario != null);
                CommandEditItem = new RelayCommand(EditItem, x => x != null);
                CommandAddItemToList = new RelayCommand(AddItemToList);
                CommandRemoveItemFromList = new RelayCommand(RemoveItemFromList, x => x != null);
                MacroList = new ObservableCollection<Macro>();
                Scenarios = new ObservableCollection<Scenario>();
                ActionTemplates = new ObservableCollection<ActionTemplate>();
                ActionTemplates.Add(new ActionTemplate(HotKey.CreateHotKey(enu.HotKeyStatus.AddAction, "F1"), 500, enu.ActionType.MouseLeftButtonClick, 1));
                ActionTemplates.Add(new ActionTemplate(HotKey.CreateHotKey(enu.HotKeyStatus.AddAction, "F2"), 500, enu.ActionType.MouseRightButtonClick, 1));
                ActionTemplates.Add(new ActionTemplate(HotKey.CreateHotKey(enu.HotKeyStatus.AddAction, "F3"), 500, enu.ActionType.MouseLeftButtonClick, 2));
                ActionTemplates.Add(new ActionTemplate(HotKey.CreateHotKey(enu.HotKeyStatus.AddAction, "F4"), 3000, enu.ActionType.MouseMove, 0));
                ActionTemplates.Add(new ActionTemplate(HotKey.CreateHotKey(enu.HotKeyStatus.AddAction, null), 3000, enu.ActionType.MouseMove, 0));
                ActionTemplates.Last().PlaceHolder = true;
            }
            catch (Exception e)
            {
                Logger.GetLogger().CatchException("ViewModel", "Constructor", e.Message);
                throw;
            }
            
        }
        //Methods

        //Commands
        private void AddItemToList(object param)
        {
            var item = param as string;
            switch (param)
            {
                case "Scenario":
                    Scenarios.Add(new Scenario());
                    SelectedScenario = Scenarios.Last();
                    CommandEditItem.Execute(SelectedScenario);
                    break;
                case "Macro":
                    MacroList.Add(new Macro());
                    SelectedMacro = MacroList.Last();
                    CommandEditItem.Execute(SelectedMacro);
                    break;
                case "ActionTemplate":
                    ActionTemplate newTemplate = new ActionTemplate();
                    ActionTemplates.Insert(ActionTemplates.Count - 1, newTemplate);
                    SelectedTemplate = newTemplate;
                    CommandEditItem.Execute(SelectedTemplate);
                    break;
                default:
                    Logger.GetLogger().CatchException("ViewModel", "AddItemToList", $"Unknown param{{{item}}}");
                    throw new Exception();
            }
        }
        private void RemoveItemFromList(object param)
        {
            if(param is Scenario scenario)
            {
                var index = Scenarios.IndexOf(scenario);
                Scenarios.Remove(scenario);
                if (Scenarios.Count > 0)
                {
                    if (index < Scenarios.Count)
                        SelectedScenario = Scenarios[index];
                    else
                        SelectedScenario = Scenarios[index - 1];
                }
            }
            else if(param is Macro macro)
            {
                var index = MacroList.IndexOf(macro);
                MacroList.Remove(macro);
                if (MacroList.Count > 0)
                {
                    if (index < MacroList.Count)
                        SelectedMacro = MacroList[index];
                    else
                        SelectedMacro = MacroList[index - 1];
                }
            }
            else if(param is ActionTemplate template)
            {
                if (template.PlaceHolder == true)
                    return;
                var index = ActionTemplates.IndexOf(template);
                ActionTemplates.Remove(template);
                template.HotKey = null;
            }
            else
            {
                Logger.GetLogger().CatchException("ViewModel", "RemoveItemFromList", "Unknown param");
                throw new Exception();
            }
        }
        private void EditItem(object param)
        {
            if (param is Macro macro)
                macro.EditingMode = !macro.EditingMode;
            else if (param is Scenario scenario)
                scenario.EditingMode = !scenario.EditingMode;
            else if (param is ActionTemplate template)
            {
                if(template.PlaceHolder == false)
                template.EditingMode = !template.EditingMode;
            }
            else
            {
                Logger.GetLogger().CatchException("ViewModel", "EditItem", "Unknown param");
                throw new Exception();
            }
        }

        private void SaveToFile(object param)
        {
            var path = param as string;
            try
            {
                ProjectPath = path ?? throw new Exception("null path");
                SelectedMacro.EditingMode = false;
                SelectedScenario.EditingMode = false;
                SelectedTemplate.EditingMode = false;
                ViewModelArgs args = ViewModelArgs.CreateFromViewModel(this);
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(stream, args);
                }
            }
            catch (Exception e)
            {
                Logger.GetLogger().CatchException("ViewModel", "SaveToFile", e.Message);
                throw;
            }
#if DEBUGLOG
            Logger.GetLogger().WriteToLog($"ViewModel: SaveToFile: Path{{{path}}} : Code{{{1}}}");
#endif
        }
        private void LoadFromFile(object param)
        {
            var path = param as string;
            try
            {
                ProjectPath = path ?? throw new Exception("null path");
                SelectedTemplate.EditingMode = false;
                SelectedTemplate = null;
                BinaryFormatter formatter = new BinaryFormatter();
                WinWrapper.UnregisterAll();
                using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    ViewModelArgs args = (ViewModelArgs)formatter.Deserialize(stream);
                    MacroList = args.MacroList;
                    Scenarios = args.Scenarios;
                    foreach (var action in Scenarios)
                    {
                        WinWrapper.RegisterKey(action.HotKey);
                    }
                    ActionTemplates = args.ActionTemplates;
                    foreach (var action in ActionTemplates)
                    {
                        WinWrapper.RegisterKey(action.HotKey);
                    }
                    SelectedMacro = args.SelectedMacro;
                    SelectedScenario = args.SelectedScenario;
                }
            }
            catch (Exception e)
            {
                Logger.GetLogger().CatchException("ViewModel", "LoadFromFile", e.Message);
                throw;
            }
#if DEBUGLOG
            Logger.GetLogger().WriteToLog($"ViewModel: LoadFromFile: Path{{{path}}} : Code{{{1}}}");
#endif
        }
        private async void ExecuteScenarioAsync(object param)
        {
            if (!ExecutionStarted)
            {
                try
                {
                    ExecutionStarted = true;
                    _tokenSource = new CancellationTokenSource();
                    var token = _tokenSource.Token;
                    await Task.Factory.StartNew(() =>
                    {
                        if (SelectedScenario.DelayedLaunch)
                        {
                            var startTime = DateTime.Now.AddSeconds(SelectedScenario.DelayTime);
                            do
                            {
                                if (token.IsCancellationRequested)
                                    return;
                            } while (DateTime.Now < startTime);
                        }
                        do
                        {
                            foreach (var macros in SelectedScenario.MacroList)
                            {
                                foreach (var action in macros.Actions)
                                {
                                    action.Execute();
                                    var SleepTime = DateTime.Now.AddMilliseconds(action.Pause);
                                    while (DateTime.Now < SleepTime)
                                    {
                                        if (token.IsCancellationRequested)
                                            return;
                                    }
                                }
                            }
                            if(SelectedScenario.ExecutionMode == enu.ExecutionMode.Loop)
                            {
                                var endTime = DateTime.Now.AddSeconds(SelectedScenario.LoopTime);
                                do
                                {
                                    if (token.IsCancellationRequested)
                                        return;
                                } while (DateTime.Now < endTime);
                            }
                        } while (SelectedScenario.ExecutionMode == enu.ExecutionMode.Loop);
                        
                    }, token);
                }
                catch (Exception e)
                {
                    Logger.GetLogger().CatchException("ViewModel", "ExecuteScenarioAsync", e.Message);
                    throw new Exception();
                }
                finally
                {
                    ExecutionStarted = false;
                }

            }
            else
            {
                _tokenSource?.Cancel();
            }
        }
    }
}
