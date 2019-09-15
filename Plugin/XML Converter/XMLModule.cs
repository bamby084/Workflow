using JdSuite.Common.Logging;
using JdSuite.Common.Module;
using JdSuite.Common.Module.MefMetadata;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Forms;
using JdSuite.Common;
using System.Xml.Linq;
using System;
using JdSuite.Common.Logging.Enums;
using System.Drawing;
using XML_Converter.Properties;

namespace XML_Converter
{
    [Export(typeof(IModule)), PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportMetadata("ModuleCategory", IModuleCategory.DATA_INPUTS)]
    [ExportMetadata("ModuleType", typeof(XmlInput))]
    public class XmlInput : BaseModule
    {
        NLog.ILogger logger = NLog.LogManager.GetLogger("XMlModule");


        private OutputNode _outputNode;

       

        protected override Bitmap Icon { get { return Resources.Icon; } }

        protected override string ToolTip { get { return "Provides an XML Schema for other modules."; } }

        protected override string ModuleName { get { return "XML_Converter"; } }

        public XmlInput()
        {
            Logger.AppName = ModuleName;
            _outputNode = new OutputNode(this, "Schema", "DataLocation");
            AddOutputNode(_outputNode);
            DisplayName = "XML Input";
        }

        public override object Execute(Workflow workflow)
        {
            logger.Trace("Executing xmlinput module");

            Logger.Log(Severity.DEBUG, LogCategory.MODULE, "Test");
            if (this._outputNode.State == null)
                this._outputNode.State = new ModuleState();
            var msg = "";
            bool failed = false;

            try
            {
                Program.Main(this._outputNode.State);
            }
            catch (Exception e)
            {
                msg = $"Critical failure in XML Converter: {e.Message}";
            }

            if (Program.form.isCancel)
            {
                return null;
            }

            if (msg.Length > 0)
            {
                failed = true;
            }
            else if (this._outputNode.State.Schema == null || this._outputNode.State.InputPath == null)
            {
                msg += "Procedure not completed.";
                if (this._outputNode.State.Schema == null)
                {
                    msg += " Schema not available. Please generate a schema before closing the window.";
                }
                if (this._outputNode.State.InputPath == null)
                {
                    msg += " Input path not available. Input file was not added before closing the window.";
                }
                failed = true;
            }
            else
            {
                if (!File.Exists(this._outputNode.State.InputPath))
                {
                    msg += string.Format("Input file does not exist at path: {0}.", this._outputNode.State.InputPath);
                    failed = true;
                }
            }

            if (failed)
            {
                if (_outputNode.IsConnected())
                {
                    msg = "Disconnecting module: " + msg;
                    _outputNode.Disconnect();
                }
                Logger.Log(Severity.WARN, LogCategory.MODULE, msg);
                MessageBox.Show(msg);
                this._outputNode.State.InputPath = "";
                return null;
            }

            logger.Info("Calling   RequestStateUpdate?.Invoke(workflow)");
            RequestStateUpdate?.Invoke(workflow);
            return null;
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
    }
}