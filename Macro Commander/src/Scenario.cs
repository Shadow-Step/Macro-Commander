using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro_Commander.src
{
    internal class Scenario
    {
        //Fields

        //Properties
        public ObservableCollection<Macro> MacrosList { get; set; }
        public double DelayedLaunch { get; set; }

        //Constructors
        public Scenario()
        {
            MacrosList = new ObservableCollection<Macro>();
        }

        //Methods
    }
}
