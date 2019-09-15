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
using JdSuite.DataSorting.Properties;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Xml;

namespace JdSuite.DataSorting
{

    [Export(typeof(IModule)), PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportMetadata("ModuleCategory", IModuleCategory.DATA_MANIPULATION)]
    [ExportMetadata("ModuleType", typeof(DataSorterModule))]
    public class DataSorterModule : BaseModule
    {
        NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(DataSorterModule));

        // private OutputNode outNode;
        private InputNode _inputNode;

        //See its further work in xml module frm Main and copy it
        private OutputNode _outputNode;

        private List<SortingField> SortingFields = new List<SortingField>();

        public Workflow WorkFlow { get; private set; }
        public InputNode InputNode
        {
            get { return _inputNode; }
            set { _inputNode = value; }
        }

        public OutputNode OutputNode
        {
            get { return _outputNode; }
            set { _outputNode = value; }
        }


        public DataSorterModule()
        {
            DisplayName = "DataSorter";
            logger.Info("creating object");
            Logger.AppName = ModuleName;

            InputNode = new InputNode(this, "Schema", "DataLocation");
            AddInputNode(InputNode);

            OutputNode = new OutputNode(this, "Schema", "DataLocation");

            OutputNode.State = new ModuleState();
            AddOutputNode(OutputNode);
        }

        protected override string ModuleName { get { return "DataSorter"; } }

      

        protected override Bitmap Icon { get { return Resources.sorting_icon_9; } }

        protected override string ToolTip { get { return "Provides Sorting for other modules."; } }


        public override void OpenWindow(Workflow workflow)
        {
            logger.Info("Entered");

            if (!CanProcess())
                return;
            
            this.WorkFlow = workflow;

            ShowParameterWindow();
        }

        public override object Execute(Workflow workflow)
        {
            logger.Info("Entered");

            if (!CanProcess())
                return null;

            string msg = "";
            bool failed = false;
            this.WorkFlow = workflow;

            if (workflow.Command==(int)JdSuite.Common.Module.Commands.DoubleClick)
            {
                ShowParameterWindow();
            }
            else if (workflow.Command == (int)Key.F6)
            {
                Run((int)workflow.Command);
            }

            var element = workflow.GetAppState(ModuleName, Guid);
            if (element != null)
            {
                //ReadProgramCSV(element.Element("Root"));
            }



            if (this.OutputNode.State.Schema == null)
            {
                msg += "Process is incomplete as Output Schema is not set.";
                failed = true;
            }

            if (this.OutputNode.State.InputPath == null)
            {

                msg += " Output node data file path is not set. ";
                failed = true;
            }

            

            if (failed)
            {
                logger.Error(msg);
                MessageService.ShowError("Output node validation error", msg);
            }

            if (failed)
            {
                if (OutputNode.IsConnected())
                {
                    msg = "Disconnecting module due to errors: " + msg;
                    OutputNode.Disconnect();
                }

                logger.Warn(msg);

                this.OutputNode.State.InputPath = "";
                return null;
            }


            logger.Info("Calling   RequestStateUpdate?.Invoke(workflow)");
            RequestStateUpdate?.Invoke(workflow);

            return null;
        }



        public override void SetContextMenuItems(ContextMenu ctxMenu)
        {
            MenuItem item = new MenuItem();
            item.Click += MenuItemViewData_Click;
            item.Header = "View Data(F5)";
            //item.Icon = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("pack://application:,,,/Images/cancel.png", UriKind.Absolute)) };

            ctxMenu.Items.Add(item);

            item = new MenuItem();
            item.Click += MenuItemRunData_Click;
            item.Header = "Run Data(F6)";

            ctxMenu.Items.Add(item);
        }

        private void MenuItemViewData_Click(object sender, RoutedEventArgs e)
        {
            ShowParameterWindow();

        }

        private void MenuItemRunData_Click(object sender, RoutedEventArgs e)
        {
            Run((int)Key.F6);
        }

        /// <summary>
        /// View data menu workflow
        /// </summary>
        private void ShowParameterWindow()
        {
            if (!CanProcess())
                return;

            ModulePageBorder.BorderBrush = System.Windows.Media.Brushes.Blue;

            ModuleState state = null;
            try
            {
                var workFlow = InputModule.GetState(InputModule);
                state = ((OutputNode)this.InputNode.Connector).State;
                var schema = ((OutputNode)this.InputNode.Connector).GetSchema();


                MainWindow mainWindowClass = new MainWindow();
                //1. Load Schema
                mainWindowClass.LoadSchema(state.Schema);

                logger.Info("Loading data from xml file {0}", state.InputPath);
                //2. Set Input Data file
                mainWindowClass.WindowViewModel.InputDataFile = state.InputPath;

                var outputFile = DataDir + "DataSorter_" + DisplayName+ "_" + DateTime.Now.ToString("MMddHHmmssfff") + ".xml";

                //3.
                mainWindowClass.WindowViewModel.OutputDataFile = outputFile;

                //4.Load Sorting Fields
                foreach (var item in SortingFields)
                    mainWindowClass.WindowViewModel.SortingFields.Add(item);

                mainWindowClass.ShowDialog();

                //Sync back sorting fields
                this.SortingFields.Clear();
                this.SortingFields.AddRange(mainWindowClass.WindowViewModel.SortingFields.ToList());
                ParameterCount = this.SortingFields.Count;


                this.InputNode.State = state;

                this.OutputNode.State.InputPath = mainWindowClass.WindowViewModel.OutputDataFile;
                this.OutputNode.State.Schema = state.Schema;
                this.OutputNode.State.TextEncoding = state.TextEncoding;
                this.OutputNode.State.InputIsSchema = state.InputIsSchema;

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
        /// Run Data menu workflow
        /// </summary>
        public override bool Run(int Command)
        {
            logger.Info($"Received Command {Command}");

            if (!CanProcess())
                return false;

            var workFlow = InputModule.GetState(InputModule);
            var state = ((OutputNode)this.InputNode.Connector).State;
           

            this.InputNode.State = state;

            string msg = "";
            if (OutputNode == null)
            {
                msg = "Cannot run datasorter as Output nod is not set yet";
                logger.Error(msg);
                MessageService.ShowError("Data Sorting Error", msg);
                return false;
            }

            if (OutputNode.State == null)
            {
                msg = "Cannot run datasorter as Output nod state is not set yet";
                logger.Error(msg);
                MessageService.ShowError("Data Sorting Error", msg);
                return false;
            }

            if (string.IsNullOrEmpty(OutputNode.State.InputPath))
            {
                msg = "Cannot run datasorter as Output file path is not set yet";
                logger.Error(msg);
                MessageService.ShowError("Data Sorting Error", msg);
                return false;
            }



            try
            {
                if (SortingFields.Count <= 0)
                {
                    msg = string.Format("Sorting fields are not providing, therefore cannot sort {0}", InputNode.State.InputPath);
                    logger.Warn(msg);

                    MessageService.ShowError("Data Sorting Error", msg);
                    return false;
                }

                ModulePageBorder.BorderBrush = System.Windows.Media.Brushes.Green;

                logger.Trace("Creating sorter object");
                XMLSorter sorter = new XMLSorter();
                sorter.DataFile = InputNode.State.InputPath;
                sorter.OutputFileName = OutputNode.State.InputPath;
                sorter.SortingFields.AddRange(this.SortingFields);

                sorter.LoadData();
                logger.Info("Calling Sorter.Sort()");
                sorter.Sort();
                logger.Info("Saving sorted data to file {0}", sorter.OutputFileName);
                sorter.Save();

                return true;

            }
            catch (Exception ex)
            {
                MessageService.ShowError("Data Sorting Error", ex.Message);
                msg = $"Critical failure in Datasorter module: {ex.Message} ";
                logger.Error(msg);
            }

            return false;
        }

        private bool CanProcess()
        {
            if (!IsConnected())
            {
                MessageService.ShowError("Data Connection Error", "Data Sorter module input node is not connected with any other module");
                return false;
            }

            string prevModName = InputModule.DisplayName;
            var workFlow = InputModule.GetState(InputModule);
            var state = ((OutputNode)this.InputNode.Connector).State;
            var schema = ((OutputNode)this.InputNode.Connector).GetSchema();

            if (state == null)
            {
                MessageService.ShowError("Data Connection Error", $"In Module {prevModName} passed invalid state object");
                return false;
            }

            if (!File.Exists(state.InputPath))
            {
                MessageService.ShowError("Data Connection Error", $"In Module {prevModName} Sate has invalid input path");
                return false;
            }

            if (state.Schema == null)
            {
                MessageService.ShowError("Data Connection Error", $"In Module {prevModName} Sate has invalid Schema");
                return false;
            }


            return true;
        }

        private bool IsConnected()
        {
            if (!this.InputNode.IsConnected())
                return false;

            return !(this.InputNode.Connector == null);
        }

        private BaseModule InputModule
        {
            get
            {

                if (this.InputNode.Connector == null) return null;

                return this.InputNode.Connector.GetModule();
            }
        }




        #region HouseKeeping

        public override void WriteParameter(XmlWriter writer)
        {
            if (SortingFields.Count > 0)
            {
                writer.WriteStartElement("Parameters");

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<SortingField>));

                xmlSerializer.Serialize(writer, SortingFields);

                writer.WriteEndElement();
            }
        }

        public override void ReadParameter(XmlReader reader)
        {
            reader.ReadStartElement();
            //reader.ReadStartElement();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<SortingField>));
            SortingFields = (List<SortingField>)xmlSerializer.Deserialize(reader);
            // reader.ReadEndElement();
            reader.ReadEndElement();
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
