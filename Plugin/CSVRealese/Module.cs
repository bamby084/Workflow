using JdSuite.Common.Module;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Reflection;
using System.ComponentModel.Composition;
using JdSuite.Common.Module.MefMetadata;
using System.Xml.Linq;
using JdSuite.Common.Logging;
using System.Windows;
using JdSuite.Common;
using CSVRealese.Properties;
using System.Drawing;

namespace CSVRealese
{

    [Export(typeof(IModule)), PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportMetadata("ModuleCategory", IModuleCategory.DATA_INPUTS)]
    [ExportMetadata("ModuleType", typeof(CSV))]
    public class CSV : BaseModule
    {

        private OutputNode Node;
        private string TextEncoding = "";

        public CSV()
        {

            Logger.AppName = ModuleName;
            Node = new OutputNode(this, "Schema", "DataLocation");
            AddOutputNode(Node);
            DisplayName = "CSV";
        }

        protected override string ModuleName { get { return "CSVRealese"; } }

        

        protected override Bitmap Icon { get { return Resources.Icon; } }

        protected override string ToolTip { get { return "Provides an CSV for other modules."; } }

        public override object Execute(Workflow workflow)
        {
            var element  = workflow.GetAppState(ModuleName, Guid);
            if (element != null)
            {
                //ReadProgramCSV(element.Element("Root"));
            }
            var msg = "";
            try
            {
                MainWindowClass mainWindowClass = new MainWindowClass();
                mainWindowClass.ShowDialog();
            }
            catch (Exception e)
            {
                msg = $"Critical failure in CSV Converter: {e.Message} ";
                throw;
            }
            return null;
        }
    }
}
