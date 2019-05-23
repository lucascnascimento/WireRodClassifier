using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireRodClassifer_1._0.Helper_Classes
{
    public class IsConfiguredEventArgs : EventArgs
    {
        private bool _configurationBool;

        public IsConfiguredEventArgs(bool value)
        {
            ConfigurationBool = value;
        }

        public bool ConfigurationBool { get => _configurationBool; private set => _configurationBool = value; }
    }
}
