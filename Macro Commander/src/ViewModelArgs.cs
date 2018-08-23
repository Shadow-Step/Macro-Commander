using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Macro_Commander.src
{
    [Serializable]
    internal class ViewModelArgs
    {
        //Fields
        public ObservableCollection<Macro> MacroList;
        public ObservableCollection<Scenario> Scenarios;
        public ObservableCollection<ActionTemplate> ActionTemplates;
        public Macro SelectedMacro;
        public Scenario SelectedScenario;
        public Dictionary<string, byte[]> GroupColors;
        public List<byte[]> FixedColors;
        //Constructor
        private ViewModelArgs() { }

        //Factory
        public static ViewModelArgs CreateFromViewModel(ViewModel viewModel)
        {
            ViewModelArgs args = new ViewModelArgs
            {
                MacroList = viewModel.MacroList,
                Scenarios = viewModel.Scenarios,
                ActionTemplates = viewModel.ActionTemplates,
                SelectedMacro = viewModel.SelectedMacro,
                SelectedScenario = viewModel.SelectedScenario,
                GroupColors = res.Statics.GroupsColors,
                FixedColors = res.Statics.FixedColors
            };
            return args;
        }
    }
}
