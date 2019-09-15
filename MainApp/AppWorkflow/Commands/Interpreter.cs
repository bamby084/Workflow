using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Commands
{
    internal class Interpreter
    {
        public Interpreter(string[] args)
        {
            if (args.Length == 0)
            {
                // TODO: Print help
                return;
            }

            // Commands are not delimited by whitespace, but instead by "-"
            string task = "";
            foreach (var str in args)
            {
                task += str + " ";
            }

            string[] commands = task.Split('-');
            if (commands.Length != 5)
            { // first element is empty string
              // TODO: Print help
                return;
            }

            // clean up raw commands
            foreach (var cmd in commands)
            {
                cmd.Trim();
            }

            // Load Workflow from PageDefinitionFile argument
            var pgdef = new Commands.PageDefinitionFile();
            if (!pgdef.Parse(GetCommandFrom("pgdef", commands)))
            {
                Print(pgdef.GetHelpEntry());
                return;
            }

            // Parse -dfc to see what type of input content we have
            var dfc = new Commands.DifferentMessageContent();
            if (!dfc.Parse(GetCommandFrom("dfc", commands)))
            {
                Print(dfc.GetHelpEntry());
                return;
            }

            // TODO: We need to first find a valid workflow chain
        }

        private static string GetCommandFrom(string cmdSwitch, string[] commands)
        {
            foreach (var cmd in commands)
            {
                if (cmd.Contains(cmdSwitch))
                {
                    return cmd.Split(' ')[1];
                }
            }

            return null;
        }

        private void Print(string str)
        {
            Console.WriteLine(str);
        }
    }
}
