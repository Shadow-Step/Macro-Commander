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
        //Fields
        private static ViewModel _viewmodel;
        private Macro _selectedmacro;

        //Properties
        public static ViewModel viewModel
        {
            get
            {
                return _viewmodel ?? (_viewmodel = new ViewModel());
            }
        }
        public ObservableCollection<Macro> MacroList { get; set; } = new ObservableCollection<Macro>();
        public Macro SelectedMacro
        {
            get { return _selectedmacro; }
            set
            {
                _selectedmacro = value;
                PropChanged("SelectedMacro");
            }
        }
        //Commands
        private RelayCommand command_addmacro;
        private RelayCommand command_delmacro;

        public RelayCommand CommandAddMacro
        {
            get
            {
                return command_addmacro ?? (command_addmacro = new RelayCommand(obj =>
                {
                    MacroList.Add(new Macro());
                    SelectedMacro = MacroList.Last();
                }
          ));
            }
        }
        public RelayCommand CommandDelMacro
        {
            get
            {
                return command_delmacro ?? (command_delmacro = new RelayCommand(
                    obj =>
                    {
                        if (obj is Macro macro && macro != null)
                        {
                            var index = MacroList.IndexOf(macro);
                            MacroList.Remove(macro);
                            if(MacroList.Count > 0)
                            {
                                if (index < MacroList.Count)
                                    SelectedMacro = MacroList[index];
                                else
                                    SelectedMacro = MacroList[index - 1];
                            }
                            
                        }
                        else
                            throw new Exception();
                    },
                    obj =>
                    {
                        return MacroList.Count > 0;
                    }
                    ));
            }
        }
        //Methods
        public void PlayMacro()
        {

        }
        public void SaveToFile()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream("temp.bin",FileMode.OpenOrCreate))
            {
                formatter.Serialize(stream, MacroList);
            }
        }
        public void LoadFromFile()
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
