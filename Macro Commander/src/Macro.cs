using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Macro_Commander.src
{
    [Serializable]
    public class Macro : INotifyWrapper , IDataErrorInfo
    {
        //Fields
        private string _name;
        private Action _selectedaction;
        private bool _startedmutex = false;
        private double _totalExecutionTime;
        private bool _editingMode;
        private string _params;
        //Properties
        public ObservableCollection<Action> Actions { get; set; }
        public Dictionary<string, List<Action>> ActionsGroups { get; set; }
        public Action SelectedAction
        {
            get { return _selectedaction; }
            set
            {
                _selectedaction = value;
                PropChanged("SelectedAction");
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == string.Empty)
                    _name = "None";
                else
                    _name = value;
                PropChanged();
                EditingMode = false;
            }
        }
        public bool StartedMutex
        {
            get { return _startedmutex; }
            set
            {
                _startedmutex = value;
                PropChanged("StartedMutex");
            }
        }
        public double TotalExecutionTime
        {
            get
            {
                return (double)Actions.Sum(x=>x.Pause) / 1000;
            }
            set
            {
                _totalExecutionTime = value;
                PropChanged();
            }

        }
        public bool EditingMode
        {
            get { return _editingMode; }
            set
            {
                _editingMode = value;
                PropChanged("EditingMode");
            }
        }
        public string Params
        {
            get { return _params; }
            set
            {
                _params = value;
                PropChanged();
            }
        }
        //Commands
        public RelayCommand CommandAddAction { get; set; }       
        public RelayCommand CommandDelAction { get; set; }
        public RelayCommand CommandExecute { get; set; }
        public RelayCommand CommandMoveForward { get; set; }
        public RelayCommand CommandMoveBackwards { get; set; }

        //Validation check
        public string Error => throw new NotImplementedException();
        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case "Params":
                        if (Params != null && Params != string.Empty && Regex.IsMatch(Params, ParamReader.PARAMS_PATTERN) == false)
                            error = "Syntax error";
                        break;
                }
                return error;
            }
        }

        //Constructor
        public Macro()
        {
            CommandAddAction = new RelayCommand(AddAction);
            CommandDelAction = new RelayCommand(DelAction);
            CommandExecute = new RelayCommand(Execute);
            CommandMoveForward = new RelayCommand(MoveActionForward);
            CommandMoveBackwards = new RelayCommand(MoveActionBackwards);
            Actions = new ObservableCollection<Action>();
            ActionsGroups = new Dictionary<string, List<Action>>();
            Name = "New Macro";
        }

        //Methods
        public void ThreadStart(int time)
        {
            try
            {
                StartedMutex = true;
                Thread.Sleep(time * 1000);
                foreach (var action in Actions)
                {
                    action.Execute();
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                StartedMutex = false;
            }
            
        }
        private void ActionGroupChange(string group, Action action)
        {
            if(action.Group != string.Empty)
            {
                var list = ActionsGroups[action.Group];
                list.Remove(action);
                if (list.Count == 0)
                {
                    ActionsGroups[action.Group] = null;
                    ActionsGroups.Remove(action.Group);
                }
                else
                    list.Sort((x, y) => x.Index.CompareTo(y.Index));
            }
            if(group != string.Empty)
            {
                if (!ActionsGroups.ContainsKey(group))
                    ActionsGroups.Add(group, new List<Action>());
                var list = ActionsGroups[group];
                list.Add(action);
                list.Sort((x, y) => x.Index.CompareTo(y.Index));
            }
        }
        //Commands
        private void Execute(object param)
        {
            foreach (var item in Actions)
            {
                item.Execute();
            }
        }
        private void AddAction(object param)
        {
            if (param is ActionMeta meta)
            {
                Actions.Add(new Action(meta));
                SelectedAction = Actions.Last();
                SelectedAction.Index = Actions.Count - 1;
                SelectedAction.GroupChangeEvent += ActionGroupChange;
            }
        }
        private void DelAction(object param)
        {
            if (param is Action action)
            {
                action.GroupChangeEvent -= ActionGroupChange;
                var index = Actions.IndexOf(action);
                if (index < Actions.Count - 1)
                    SelectedAction = Actions[index + 1];
                else if (index > 0)
                    SelectedAction = Actions[index - 1];
                for (int i = index; i < Actions.Count; i++)
                    Actions[i].Index--;
                Actions.Remove(action);
                
            }
        }
        private void StartMacro(object param)
        {

        }
        private void MoveActionForward(object param)
        {
            if (param is Action action)
            {
                var index = Actions.IndexOf(action);
                if (index < Actions.Count - 1)
                {
                    Action temp = Actions[index + 1];
                    temp.Index--;
                    action.Index++;
                    Actions[index + 1] = Actions[index];
                    Actions[index] = temp;
                    SelectedAction = action;
                }
            }
        }
        private void MoveActionBackwards(object param)
        {
            if (param is Action action)
            {
                var index = Actions.IndexOf(action);
                if (index > 0)
                {
                    
                    Action temp = Actions[index - 1];
                    action.Index--;
                    temp.Index++;
                    Actions[index - 1] = Actions[index];
                    Actions[index] = temp;
                    SelectedAction = action;
                }
            }
        }
        
    }
}
