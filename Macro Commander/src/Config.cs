using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro_Commander.src
{
    [Serializable]
    class Config : INotifyWrapper
    {
        private static Config config;
        public static Config ConfigHandle
        {
            get
            {
                return config ?? (config = new Config());
            }
        }
    }
}
