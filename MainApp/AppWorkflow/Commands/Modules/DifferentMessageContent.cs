using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AppWorkflow.Commands
{
    internal class DifferentMessageContent : ICommand
    {
        public string extension = "";
        public string path = "";

        public string GetHelpEntry()
        {
            throw new NotImplementedException();
        }

        public bool Parse(string arg)
        {
            if (!File.Exists(arg))
            {
                return false;
            }

            var extensionStart = arg.LastIndexOf('.') + 1;
            extension = arg.Substring(extensionStart);

            path = arg;
            return true;
        }
    }
}
