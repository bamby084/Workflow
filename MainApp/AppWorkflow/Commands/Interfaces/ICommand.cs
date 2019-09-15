using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Commands
{
    internal interface ICommand
    {
        string GetHelpEntry();

        bool Parse(string arg);
    }
}
