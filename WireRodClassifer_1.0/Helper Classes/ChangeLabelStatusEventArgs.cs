using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireRodClassifer_1._0.Helper_Classes
{
    public class ChangeLabelStatusEventArgs : EventArgs
    {
        private string _status;

        public ChangeLabelStatusEventArgs(string status)
        {
            Status = status;
        }

        public string Status { get => _status; private set => _status = value; }
    }
}
