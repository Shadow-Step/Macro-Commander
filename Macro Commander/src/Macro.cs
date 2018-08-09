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
        [NonSerialized]
        private Thread StartThread; //!!!

        //Fields
        private string _name;
        private Action _selectedaction;
        private bool _startedmutex = false;
        
        //Properties
        public ObservableCollection<Action> Actions { get; set; } = null;
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
                _name = value;
                PropChanged("Name");
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
        //Commands
        [NonSerialized]
        private RelayCommand _command_addaction;
        [NonSerialized]
        private RelayCommand _command_delaction;
        [NonSerialized]
        private RelayCommand _command_start;
        [NonSerialized]
        private RelayCommand _command_moveforward;
        [NonSerialized]
        private RelayCommand _command_moveback;

        public RelayCommand CommandAddAction
        {
            get
            {
                return _command_addaction ?? (_command_addaction = new RelayCommand(obj =>
                {
                    if(obj is ActionMeta meta)
                    {
                        Actions.Add(new Action(meta));
                        SelectedAction = Actions.Last();
                    }
                }
                ));
            }
        }
        public RelayCommand CommandDelAction
        {
            get
            {
                return _command_delaction ?? (_command_delaction = new RelayCommand(obj =>
                {
                    if(obj is Action action)
                    {
                        var index = Actions.IndexOf(action);
                        if (index < Actions.Count - 1)
                            SelectedAction = Actions[index + 1];
                        else if (index > 0)
                            SelectedAction = Actions[index - 1];
                        Actions.Remove(action);
                    }
                }));
            }
        }
        public RelayCommand CommandStart
        {
            get
            {
                return _command_start ?? (_command_start = new RelayCommand(obj => Start(obj), obj=> Actions.Count > 0));
            }
        }
        public RelayCommand CommandMoveForward
        {
            get
            {
                return _command_moveforward ?? (_command_moveforward = new RelayCommand(obj =>
                {
                    if(obj is Action action)
                    {
                        var index = Actions.IndexOf(action);
                        if(index < Actions.Count - 1)
                        {
                            Action temp = Actions[index + 1];
                            Actions[index + 1] = Actions[index];
                            Actions[index] = temp;
                            SelectedAction = action;
                        }
                    }
                }
                ));
            }
        }
        public RelayCommand CommandMoveBack
        {
            get
            {
                return _command_moveback ?? (_command_moveback = new RelayCommand(obj =>
                {
                    if (obj is Action action)
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
                ));
            }
        }
        //Methods
        public Macro()
        {
            Actions = new ObservableCollection<Action>();
            Name = DateTime.Now.Millisecond.ToString(); //!!!
        }
        public void Start(object obj)
        {
            if (obj is int time)
            {
                if (StartThread == null || (StartThread != null && StartThread.IsAlive == false))
                {
                    
                    StartThread = new Thread(() => ThreadStart(time));
                    StartThread.Start();
                }
                else
                {
                    StartThread.Abort();
                    StartThread.Join();
                }
            }
        }
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

       
    }
}
