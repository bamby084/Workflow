using Designer.Properties;
using JdSuite.Common;
using JdSuite.Common.Logging;
using JdSuite.Common.Logging.Enums;
using JdSuite.Common.Module;
using JdSuite.Common.Module.MefMetadata;
using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Designer
{
    [Export(typeof(IModule)), PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportMetadata("ModuleCategory", IModuleCategory.DESIGNS)]
    [ExportMetadata("ModuleType", typeof(Module))]
    public class Module : BaseModule
    {
        private InputNode Node;
        private ModuleState State;

        protected override string ModuleName { get { return "Designer"; } }

      

        protected override Bitmap Icon { get { return Resources.Icon; } }

        protected override string ToolTip { get { return "Design a document based on an input schema."; } }

        public Module()
        {
            Node = new InputNode(this, "Schema", "DataLocation");
            Node.OnDisconnect += delegate
            {
                State = null;
            };
            AddInputNode(Node);
            DisplayName = "Designer";
        }

        private void OnExecuteFailed(string msg, Severity severity)
        {
            Logger.Log(
                severity,
                LogCategory.MODULE,
                msg
            );
            MessageBox.Show(msg, "", MessageBoxButton.OK, MessageBoxImage.Information);
            Node.Disconnect();
        }

        public override object Execute(Workflow workflow)
        {
            RetrieveEndpointData();
            if (State == null)
            {
                return null;
            }

            if (State.Schema == null)
            {
                var msg = "Provided schema is empty.";
                OnExecuteFailed(msg, Severity.INFO);
                return null;
            }

            if (!Path.GetExtension(State.InputPath).ToLower().Contains("xml"))
            {
                var msg = $"Provided data file at {State.InputPath} is not an XML file.";
                OnExecuteFailed(msg, Severity.INFO);
                return null;
            }
            else if (!File.Exists(State.InputPath))
            {
                var msg = "Could not find XML data file at path " + State.InputPath;
                OnExecuteFailed(msg, Severity.ERROR);
                return null;
            }

            var dataFile = XDocument.Load(State.InputPath);
            if (State.InputIsSchema || IsSchemaFile(dataFile))
            {
                var msg = $"Provided file at {State.InputPath} is not an XML data file.";
                OnExecuteFailed(msg, Severity.INFO);
                return null;
            }

            try
            {
                var window = MainWindow.Instance;
                window.Load(workflow, Guid, State.Schema, dataFile);
                window.Show();
                RequestStateUpdate?.Invoke(window.Workflow);
            }
            catch (Exception e)
            {
                string msg = $"Critical failure in Designer: {e.Message}";
                OnExecuteFailed(msg, Severity.ERROR);
            }

            return null;
        }

        private bool IsSchemaFile(XDocument doc)
        {
            if (doc.Root != null && doc.Root.Name.LocalName.Contains("xs"))
            {
                return true;
            }
            return false;
        }

        public override int GetMajorVersion()
        {
            return 1;
        }

        public override int GetMinorVersion()
        {
            return 0;
        }

        public override int GetPatchVersion()
        {
            return 0;
        }

        private void RetrieveEndpointData()
        {
            try
            {
                State = ((BaseOutputNode)Node.GetConnector())?.State;
                if (State == null)
                {
                    return;
                }
            }
            catch (Exception e)
            {
                Node.Disconnect();
                var msg = $"Connection exception: {e.Message}";
                Logger.Log(Severity.ERROR, LogCategory.MODULE, msg);
                MessageBox.Show(msg, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}