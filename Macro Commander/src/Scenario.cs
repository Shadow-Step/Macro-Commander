using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro_Commander.src
{
    [Serializable]
    internal class Scenario
    {
        //Fields

        //Properties
        public ObservableCollection<Macro> MacroList { get; set; }
        public double DelayedLaunch { get; set; }

        //Constructors
        public Scenario()
        {
            MacroList = new ObservableCollection<Macro>();
        }

        //Methods
    }
}
