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
using System.Drawing;
using DataOutput.Properties;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using JdSuite.Common.TreeListView;
using System.Xml;

namespace DataOutput
{

    [Export(typeof(IModule)), PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportMetadata("ModuleCategory", IModuleCategory.OUTPUTS)]
    [ExportMetadata("ModuleType", typeof(DataOutputModule))]
    public class DataOutputModule : BaseModule
    {
        NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(DataOutputModule));

        private static List<DataOutputModule> Instances = new List<DataOutputModule>();

        #region Properties
        private InputNode inNode;

        public int MarkedMenu { get; private set; } = 99999;

        public Workflow WorkFlow { get; private set; }

        public DataOutputModule()
        {
            DisplayName = "DataOutput";

            logger.Info("creating object");
            Logger.AppName = ModuleName;

            //outNode = new OutputNode(this, "Schema", "DataLocation");
            // AddOutputNode(outNode);

            inNode = new InputNode(this, "Schema", "DataLocation");
            AddInputNode(inNode);

            Instances.Add(this);
        }

        protected override string ModuleName { get { return "DataOutputRelease"; } }



        protected override Bitmap Icon { get { return Resources.icon_20; } }

        protected override string ToolTip { get { return "Provides DataOutput for other modules."; } }

        #endregion Properties


        public override void OpenWindow(Workflow workflow)
        {
            ShowWindow();
        }

        public override bool Run(int Command)
        {
            if (MarkedMenu != Command)
                return false;
            ShowWindow();
            return true;
        }

        public override object Execute(Workflow workflow)
        {
            logger.Info($"Entered {DisplayName}");

            if (workflow.Command == (int)MarkedMenu && workflow.Command == (int)Key.F5)
            {
                this.WorkFlow = workflow;
                ShowWindow();
            }
            else if (workflow.Command == (int)MarkedMenu && workflow.Command == (int)Key.F6)
            {
                this.WorkFlow = workflow;
                ShowWindow();
            }
            else if (workflow.Command == (int)Commands.DoubleClick)
            {
                this.WorkFlow = workflow;
                ShowWindow();
            }

            /*
            var element = workflow.GetAppState(ModuleName, Guid);
            if (element != null)
            {
                //ReadProgramCSV(element.Element("Root"));
            }*/

            return null;
        }

        public override void SetContextMenuItems(ContextMenu ctxMenu)
        {
            base.SetContextMenuItems(ctxMenu);

            MenuItem item = new MenuItem();
            item.Click += F6_Click;
            item.Header = "Proof Data (F6)";

            ctxMenu.Items.Add(item);

            item = new MenuItem();
            item.Click += F5_Click;
            item.Header = "Production (F5)";

            ctxMenu.Items.Add(item);

        }

        private void F5_Click(object sender, RoutedEventArgs e)
        {
            SetKey((int)Key.F5);
        }

        private void F6_Click(object sender, RoutedEventArgs e)
        {
            SetKey((int)Key.F6);
        }

        public void SetKey(int iKey)
        {
            logger.Info($"{DisplayName} Entered");
            if (iKey == (int)Key.F5)
            {
                MarkedMenu = iKey;
                ModulePageBorder.BorderBrush = System.Windows.Media.Brushes.Blue;

                var obj = Instances.FirstOrDefault(x => x.MarkedMenu == iKey);


                if (obj != null && obj != this)
                {
                    obj.SetKey((int)Key.F6);
                }


            }
            else if (iKey == (int)Key.F6)
            {
                MarkedMenu = iKey;
                ModulePageBorder.BorderBrush = System.Windows.Media.Brushes.Yellow;

                var obj = Instances.FirstOrDefault(x => x.MarkedMenu == iKey);

                if (obj != null && obj != this)
                {
                    obj.SetKey((int)Key.F5);
                }


            }
        }



        /// <summary>
        /// View Data menu workflow
        /// </summary>
        private void ShowWindow()
        {
            if (!CanProcess())
                return;


            // ModulePageBorder.BorderBrush = System.Windows.Media.Brushes.Blue;

            ModuleState state = null;
            try
            {
                logger.Info($"Calling {InputModule.DisplayName}.Run() on input module");

                bool status = InputModule.Run(this.MarkedMenu);

                logger.Info($"{InputModule.DisplayName}.Run() Status={status}");

                var workFlow = InputModule.GetState(InputModule);
                state = ((OutputNode)this.inNode.Connector).State;



                if (!status)
                {
                    MessageService.ShowError("Module Execution Error", $"{InputModule.DisplayName} execution error");
                    return;
                }


                if (state.Schema == null)
                {
                    MessageService.ShowError("Data Connection Error", $"In Module {InputModule.DisplayName} Sate has invalid Schema");
                    return;
                }

                if (!File.Exists(state.InputPath))
                {
                    MessageService.ShowError("Data Connection Error", $"In Module {InputModule.DisplayName} output file [{state.InputPath}] does not exists");
                    return;
                }

               // ModulePageBorder.BorderBrush = System.Windows.Media.Brushes.Green;

                MainWindowClass mainWindowClass = new MainWindowClass();
                mainWindowClass.Schema = state.Schema;
                mainWindowClass.DataFile = state.InputPath;
                mainWindowClass.TextBlockModuleTitle.Text = "Output Module [" + DisplayName + "]";



                logger.Info("Loading data from xml file {0}", state.InputPath);

                mainWindowClass.ShowDialog();

                mainWindowClass.Close();

            }
            catch (Exception ex)
            {
                if (state == null)
                {
                    logger.Error(ex, $"Data loading error in module class");
                }
                else
                {
                    logger.Error(ex, $"XML Loading Error {state.InputPath}");
                }
            }
        }

        /// <summary>
        /// Not done data menu workflow
        /// </summary>
        private void RunF5()
        {
            if (!CanProcess())
                return;

           // ModulePageBorder.BorderBrush = System.Windows.Media.Brushes.Green;
            var msg = "";
            try
            {
                MainWindowClass mainWindowClass = new MainWindowClass();
                mainWindowClass.ShowDialog();
            }
            catch (Exception e)
            {
                msg = $"Critical failure in DataOutput Converter: {e.Message} ";
                throw;
            }
        }

        private bool CanProcess()
        {
            if (!IsConnected())
            {
                MessageService.ShowError("Data Connection Error", "Data Output module input node is not connected with any other module");
                return false;
            }

            string prevModName = InputModule.DisplayName;
            var workFlow = InputModule.GetState(InputModule);
            var state = ((OutputNode)this.inNode.Connector).State;
            var schema = ((OutputNode)this.inNode.Connector).GetSchema();

            if (state == null)
            {
                MessageService.ShowError("Data Connection Error", $"In Module {prevModName} passed invalid state object");
                return false;
            }



            return true;
        }

        private bool IsConnected()
        {
            if (!this.inNode.IsConnected())
                return false;

            return !(this.inNode.Connector == null);
        }

        private BaseModule InputModule
        {
            get
            {

                if (this.inNode.Connector == null) return null;

                return this.inNode.Connector.GetModule();
            }
        }




        #region HouseKeeping


        public override void WriteParameter(XmlWriter writer)
        {
            /*
            if (SortingFields.Count > 0)
            {
                

                writer.WriteStartElement("Parameters");

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<SortingField>));

                xmlSerializer.Serialize(writer, SortingFields);

                writer.WriteEndElement();
            }*/
        }

        public override void ReadParameter(XmlReader reader)
        {
            /*
            reader.ReadStartElement();
            //reader.ReadStartElement();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<SortingField>));
            SortingFields = (List<SortingField>)xmlSerializer.Deserialize(reader);
            // reader.ReadEndElement();
            reader.ReadEndElement();
            */
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

        #endregion HouseKeeping
    }
}
