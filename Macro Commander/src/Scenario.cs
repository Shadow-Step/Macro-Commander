using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Macro_Commander.src
{
    [Serializable]
    internal class Scenario : INotifyWrapper
    {
        //Fields
        private string _name;
        private Macro _selectedMacro;
        private Macro _previewMacro;
        private int _selectedIndex;
        private enu.ExecutionMode _executionMode;
        private bool _delayedLaunch;
        private double _delayTime;
        private double _loopTime;
        private HotKey _hotKey;
        //Properties
        public ObservableCollection<Macro> MacroList { get; set; }
        public Macro SelectedMacro
        {
            get { return _selectedMacro; }
            set
            {
                _selectedMacro = value;
                PreviewMacro = value;
                PropChanged("SelectedMacro");
            }
        }
        public Macro PreviewMacro
        {
            get { return _previewMacro; }
            set
            {
                _previewMacro = value;
                PropChanged("PreviewMacro");
            }
        }
        public enu.ExecutionMode ExecutionMode
        {
            get { return _executionMode; }
            set
            {
                _executionMode = value;
                PropChanged("ExecutionMode");
            }
        }
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                if (value == -1)
                    SelectedMacro = null;
                else
                    SelectedMacro = MacroList[value];
                PropChanged("SelectedIndex");
            }
        }
        public bool DelayedLaunch
        {
            get { return _delayedLaunch; }
            set
            {
                _delayedLaunch = value;
                PropChanged("DelayedLaunch");
            }
        }
        public double DelayTime
        {
            get { return _delayTime; }
            set
            {
                _delayTime = value;
                PropChanged("DelayTime");
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                PropChanged("Name");
            }
        }
        public double LoopTime
        {
            get { return _loopTime; }
            set
            {
                _loopTime = value;
                PropChanged("LoopTime");
            }
        }
        public HotKey HotKey
        {
            get { return _hotKey; }
            set
            {
                WinWrapper.UnregisterKey(_hotKey);
                _hotKey = value;
                PropChanged("HotKey");
            }
        }
        //Commands
        public RelayCommand CommandAddMacro { get; set; }
        public RelayCommand CommandDelMacro { get; set; }
        public RelayCommand CommandMoveMacroUp { get; set; }
        public RelayCommand CommandMoveMacroDown { get; set; }
        public RelayCommand CommandExecute { get; set; }
        //Constructors
        public Scenario()
        {
            MacroList = new ObservableCollection<Macro>();
            CommandAddMacro = new RelayCommand(AddMacro);
            CommandDelMacro = new RelayCommand(DelMacro);
            CommandMoveMacroUp = new RelayCommand(MoveMacroUp);
            CommandMoveMacroDown = new RelayCommand(MoveMacroDown);
            ExecutionMode = enu.ExecutionMode.Single;
        }

        //Commands
        private void AddMacro(object param)
        {
            Macro macro = param as Macro;
            if (macro == null)
                throw new Exception();
            MacroList.Add(macro);
        }
        private void DelMacro(object param)
        {
            if (param is Macro macro && macro != null)
            {
                var index = MacroList.IndexOf(macro);
                MacroList.Remove(macro);
            }
            else
                throw new Exception();
        }
        private void MoveMacroUp(object param)
        {
            if (param is int index)
            {
                if (index > 0)
                {
                    Macro temp = MacroList[index - 1];
                    MacroList[index - 1] = MacroList[index];
                    MacroList[index] = temp;
                    SelectedIndex = index - 1;
                }
            }
        }
        private void MoveMacroDown(object param)
        {
            if (param is int index)
            {
                if (index == -1)
                    return;
                if (index < MacroList.Count - 1)
                {
                    Macro temp = MacroList[index + 1];
                    MacroList[index + 1] = MacroList[index];
                    MacroList[index] = temp;
                    SelectedIndex = index + 1;
                }
            }
        }
        private void Execute(object param)
        {
            foreach (var item in MacroList)
            {
                item.CommandExecute.Execute(null);
            }
        }
    }
}
