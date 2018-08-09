using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Macro_Commander.src
{
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> can_execute;

        public RelayCommand(Action<object>execute,Func<object,bool>can_execute = null)
        {
            this.execute = execute;
            this.can_execute = can_execute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public bool CanExecute(object parameter)
        {
            return can_execute == null || can_execute(parameter);
        }
        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}
