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
        private Macro _selectedMacro;
        private Scenario _selectedScenario;
        private string _projectPath;
        //Properties
        public ObservableCollection<Macro> MacroList { get; set; }
        public ObservableCollection<Scenario> Scenarios { get; set; }
        public ObservableCollection<Action> ActionTemplates { get; set; }
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
            MacroList = new ObservableCollection<Macro>();
            Scenarios = new ObservableCollection<Scenario>();
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
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream("temp.bin", FileMode.OpenOrCreate))
            {
                formatter.Serialize(stream, MacroList);
            }
        }
        private void LoadFromFile(object param)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream("temp.bin", FileMode.OpenOrCreate))
            {
                MacroList = (ObservableCollection<Macro>)formatter.Deserialize(stream);
                SelectedMacro = MacroList.First();
            }
        }
    }
}
