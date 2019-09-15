using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AppWorkflow.Commands
{
    internal class PageDefinitionFile : ICommand
    {
        public Controls.WorkflowCanvas workflow;
        private bool fileExists = false;

        public string GetHelpEntry()
        {
            var header = "ERROR: Invalid pgdef command";
            if (!fileExists)
            {
                header += ": File does not exist.";
            }
            header += '\n';
            var body = "";
            return header + body;
        }

        public bool Parse(string arg)
        {
            if (!File.Exists(arg))
            {
                return false;
            }
            fileExists = true;

            using (FileStream fs = new FileStream(arg, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Controls.WorkflowScrollViewer));
                workflow = ((Controls.WorkflowScrollViewer)serializer.Deserialize(fs)).Content as Controls.WorkflowCanvas;
            }

            // Call update layout to hook up the modules
            workflow.UpdateLayout();
            return true;
        }
    }
}
