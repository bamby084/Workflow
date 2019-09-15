using JdSuite.Common.Module;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel.Composition;
using JdSuite.Common;
using JdSuite.Common.Module.MefMetadata;
using System.Xml.Linq;
using JdSuite.Common.Logging;
using System.Drawing;
using ScriptingApp.Properties;

namespace ScriptingApp
{
    [Export(typeof(IModule)), PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportMetadata("ModuleCategory", IModuleCategory.DATA_MANIPULATION)]
    [ExportMetadata("ModuleType", typeof(Scripting))]
    public class Scripting : BaseModule
    {
        private OutputNode outNode;
        private InputNode inNode;

        protected override string ModuleName { get { return "Script"; } }

       

        protected override Bitmap Icon { get { return Resources.Icon; } }

        protected override string ToolTip { get { return "Provides an Scripting support."; } }

        public Scripting()
        {
            Logger.AppName = ModuleName;
            outNode = new OutputNode(this, "Schema", "DataLocation");
            AddOutputNode(outNode);
            inNode = new InputNode(this, "Schema", "DataLocation");
            AddInputNode(inNode);
            DisplayName = "Script";
        }

        public override object Execute(Workflow workflow)
        {
            var element = workflow.GetAppState(ModuleName, Guid);
            if (element != null)
            {
                ReadProgramXml(element.Element("Root"));
            }

            var msg = "";
            bool failed = false;

            try
            {
                frmMain main = new frmMain();
                main.ShowDialog();
            }
            catch (Exception e)
            {
                msg = $"Critical failure in XML Converter: {e.Message}";
            }

            //if (Program.form.isCancel)
            //{
            //    return null;
            //}

            //Schema = Program.form.Schema;
            //InputPath = Program.form.InputPath;
            //TextEncoding = Program.form.SelectedEncoding;
            //InputIsSchema = Program.form.isReadingFromSchema;

            //if (msg.Length > 0)
            //{
            //    failed = true;
            //}
            //else if (Schema == null || InputPath == null)
            //{
            //    msg += "Procedure not completed.";
            //    if (Schema == null)
            //    {
            //        msg += " Schema not available. Please generate a schema before closing the window.";
            //    }
            //    if (InputPath == null)
            //    {
            //        msg += " Input path not available. Input file was not added before closing the window.";
            //    }
            //    failed = true;
            //}
            //else
            //{
            //    if (!File.Exists(InputPath))
            //    {
            //        msg += string.Format("Input file does not exist at path: {0}.", InputPath);
            //        failed = true;
            //    }
            //}

            //if (failed)
            //{
            //    if (Node.Connected())
            //    {
            //        msg = "Disconnecting module: " + msg;
            //        Node.Disconnect();
            //    }
            //    Logger.Log(Severity.WARN, LogCategory.MODULE, msg);
            //    MessageBox.Show(msg);
            //    InputPath = "";
            //    return null;
            //}

            XElement state = GenerateProgramXml();

            workflow.SetAppState(ModuleName, state, Guid);

            RequestStateUpdate?.Invoke(workflow);
            return null;
        }

        public void ReadProgramXml(XElement element)
        {
            //TextEncoding = element.Attribute("TextEncoding").Value;
            //InputIsSchema = element.Attribute("InputType").Value == "schema" ? true : false;
            //InputPath = element.Attribute("InputPath").Value;
            //Schema = new XDocument(element.Element("Schema").FirstNode);
        }

        public XElement GenerateProgramXml()
        {
            return new XElement("Root"
            //,new XAttribute("TextEncoding", TextEncoding),
            //new XAttribute("InputType", InputIsSchema ? "schema" : "data"),
            //new XAttribute("InputPath", InputPath),
            //new XElement("Schema", Schema.Root)
            );
        }
    }
}
