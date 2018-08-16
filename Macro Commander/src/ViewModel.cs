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
                return _viewmodel ?? (_viewmodel = new ViewModel());
            }
        }
        //Fields
        private CancellationTokenSource _tokenSource;
        private ObservableCollection<Macro> _macroList;
        private ObservableCollection<Scenario> _scenarios;
        private ObservableCollection<ActionTemplate> _actionTemplates;
        private Macro _selectedMacro;
        private Scenario _selectedScenario;
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
                _selectedMacro = value;
                PropChanged("SelectedMacro");
            }
        }
        public Scenario SelectedScenario
        {
            get { return _selectedScenario; }
            set
            {
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
        //Commands
        public RelayCommand CommandAddMacro { get; set; }
        public RelayCommand CommandDelMacro { get; set; }
        public RelayCommand CommandSaveToFile { get; set; }
        public RelayCommand CommandLoadFromFile { get; set; }
        public RelayCommand CommandAddScenario { get; set; }
        public RelayCommand CommandDelScenario { get; set; }
        public RelayCommand CommandExecuteScenarioAsync { get; set; }
        //Constructor
        private ViewModel()
        {
            CommandAddMacro = new RelayCommand(AddMacro);
            CommandDelMacro = new RelayCommand(DelMacro, x => MacroList.Count > 0);
            CommandSaveToFile = new RelayCommand(SaveToFile);
            CommandLoadFromFile = new RelayCommand(LoadFromFile);
            CommandAddScenario = new RelayCommand(AddScenario);
            CommandExecuteScenarioAsync = new RelayCommand(ExecuteScenarioAsync,(param)=>SelectedScenario!=null);
            MacroList = new ObservableCollection<Macro>();
            Scenarios = new ObservableCollection<Scenario>();
            ActionTemplates = new ObservableCollection<ActionTemplate>();
            ActionTemplates.Add(new ActionTemplate(HotKey.CreateHotKey("F1",enu.HotKeyStatus.AddAction), 500, enu.ActionType.LeftClick, 1));
            ActionTemplates.Add(new ActionTemplate(HotKey.CreateHotKey("F2", enu.HotKeyStatus.AddAction), 500, enu.ActionType.RightClick, 1));
            ActionTemplates.Add(new ActionTemplate(HotKey.CreateHotKey("F3", enu.HotKeyStatus.AddAction), 500, enu.ActionType.LeftClick, 2));
            ActionTemplates.Add(new ActionTemplate(HotKey.CreateHotKey("F4", enu.HotKeyStatus.AddAction), 3000, enu.ActionType.Pause, 0));
        }
        //Methods
        
       
        //Commands
        private void AddMacro(object param)
        {
            MacroList.Add(new Macro());
            SelectedMacro = MacroList.Last();
        }
        private void DelMacro(object param)
        {
            if (param is Macro macro && macro != null)
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
            else
                throw new Exception();
        }
        private void SaveToFile(object param)
        {
            ViewModelArgs args = ViewModelArgs.CreateFromViewModel(this);
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream("temp.bin", FileMode.OpenOrCreate))
            {
                formatter.Serialize(stream, args);
            }
        }
        private void LoadFromFile(object param)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream("temp.bin", FileMode.OpenOrCreate))
            {
                ViewModelArgs args = (ViewModelArgs)formatter.Deserialize(stream);
                MacroList = args.MacroList;
                Scenarios = args.Scenarios;
                ActionTemplates = args.ActionTemplates;
                SelectedMacro = args.SelectedMacro;
                SelectedScenario = args.SelectedScenario;
            }
        }
        private void AddScenario(object param)
        {
            Scenarios.Add(new Scenario());
            SelectedScenario = Scenarios.Last();
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
                catch (Exception)
                {
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
