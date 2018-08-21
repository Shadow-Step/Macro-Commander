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
    public class Macro : INotifyWrapper
    {
        //Fields
        private string _name;
        private Action _selectedaction;
        private bool _startedmutex = false;
        private double _totalExecutionTime;
        private bool _editingMode;
        //Properties
        public ObservableCollection<Action> Actions { get; set; }
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
        //Commands
        public RelayCommand CommandAddAction { get; set; }       
        public RelayCommand CommandDelAction { get; set; }
        public RelayCommand CommandExecute { get; set; }
        public RelayCommand CommandMoveForward { get; set; }
        public RelayCommand CommandMoveBackwards { get; set; }
        //Constructor
        public Macro()
        {
            CommandAddAction = new RelayCommand(AddAction);
            CommandDelAction = new RelayCommand(DelAction);
            CommandExecute = new RelayCommand(Execute);
            CommandMoveForward = new RelayCommand(MoveActionForward);
            CommandMoveBackwards = new RelayCommand(MoveActionBackwards);
            Actions = new ObservableCollection<Action>();
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
            }
        }
        private void DelAction(object param)
        {
            if (param is Action action)
            {
                var index = Actions.IndexOf(action);
                if (index < Actions.Count - 1)
                    SelectedAction = Actions[index + 1];
                else if (index > 0)
                    SelectedAction = Actions[index - 1];
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
                    Actions[index - 1] = Actions[index];
                    Actions[index] = temp;
                    SelectedAction = action;
                }
            }
        }
        
    }
}
