using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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
        private ObservableCollection<Macro> _macroList;
        private ObservableCollection<Scenario> _scenarios;
        private ObservableCollection<ActionTemplate> _actionTemplates;
        private Macro _selectedMacro;
        private Scenario _selectedScenario;
        private string _projectPath;
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
        //Commands
        public RelayCommand CommandAddMacro { get; set; }
        public RelayCommand CommandDelMacro { get; set; }
        public RelayCommand CommandSaveToFile { get; set; }
        public RelayCommand CommandLoadFromFile { get; set; }
        //Constructor
        private ViewModel()
        {
            CommandAddMacro = new RelayCommand(AddMacro);
            CommandDelMacro = new RelayCommand(DelMacro, x => MacroList.Count > 0);
            CommandSaveToFile = new RelayCommand(SaveToFile);
            CommandLoadFromFile = new RelayCommand(LoadFromFile);
            MacroList = new ObservableCollection<Macro>();
            Scenarios = new ObservableCollection<Scenario>();
            ActionTemplates = new ObservableCollection<ActionTemplate>();
            ActionTemplates.Add(new ActionTemplate("F1", 500, enu.ActionType.Click));
            ActionTemplates.Add(new ActionTemplate("F2", 500, enu.ActionType.DoubleClick));
            ActionTemplates.Add(new ActionTemplate("F3", 3000, enu.ActionType.Pause));
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
    }
}
