using JdSuite.Common.Internal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace JdSuite.Common.Module
{
    public abstract class BaseModule : NotifyPropertyChangeBase, IModule
    {
        NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(BaseModule));

        private string _displayName = "BaseModule";

        public SerializableDictionary<Guid, ModuleState> Store { get; set; } = new SerializableDictionary<Guid, ModuleState>();

        public int ParameterCount { get; set; } = 0;

        public string DataDir { get; set; }

        protected abstract string ModuleName { get; }

        public string DisplayName
        {
            get { return _displayName; }
            set { SetPropertry(ref _displayName, value); }
        }
        protected abstract Bitmap Icon { get; }

        protected abstract string ToolTip { get; }

        public Border ModulePageBorder { get; set; }

        private List<BaseInputNode> InputNodes = new List<BaseInputNode>();
        private List<BaseOutputNode> OutputNodes = new List<BaseOutputNode>();

        private Guid _Guid = Guid.NewGuid();

        ~BaseModule()
        {
            if (!System.Environment.HasShutdownStarted)
            {
                Disconnect();
            }
        }

        public virtual void SetContextMenuItems(ContextMenu ctxMenu)
        {


        }



        /// <summary>
        /// When called, prompt the parent-level application to update with the
        /// updated workflow.
        /// </summary>
        public Action<Workflow> RequestStateUpdate;

        /// <summary>
        /// Get the current workflow state from the parent application
        /// </summary>
        public Func<BaseModule, Workflow> GetState;

        public Guid Guid => _Guid;

        public void Disconnect()
        {
            foreach (var input in InputNodes)
            {
                input.Disconnect();
            }

            foreach (var output in OutputNodes)
            {
                output.Disconnect();
            }
        }

        /// <summary>
        /// Execute a representation of the data for this module.
        /// The return object should be in a human-readable file
        /// format (string), or at least a file format readable by a program capable of reading
        /// the required file extension. A module may return null if it wishes
        /// to block and provide it's own human-readable interface.
        /// </summary>
        /// <param name="workflow">The current *.flo file.</param>
        /// <returns>
        /// The representation of this module's data.
        /// If it is not readily= available, this should return null.
        /// </returns>
        public virtual object Execute(Workflow workflow) { return null; }

        public virtual void OpenWindow(Workflow workflow) { }

        public virtual bool Run(int Command) { throw new NotImplementedException("Run is not implemented " + DisplayName); }



        public virtual string GetExtension()
        {
            return null;
        }

        public List<BaseInputNode> GetInputNodes()
        {
            return InputNodes;
        }

        public List<BaseOutputNode> GetOutputNodes()
        {
            return OutputNodes;
        }

        public virtual int GetMajorVersion()
        {
            return 0;
        }

        public virtual int GetMinorVersion()
        {
            return 0;
        }



        public virtual int GetPatchVersion()
        {
            return 0;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public virtual string GetToolTipDescription()
        {
            return ToolTip;
        }

        public bool HasConnections()
        {
            foreach (var input in InputNodes)
            {
                if (input.IsConnected())
                {
                    return true;
                }
            }

            foreach (var output in OutputNodes)
            {
                if (output.IsConnected())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether this instance is ready for node communication.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is ready; otherwise, <c>false</c>.
        /// </returns>
        public bool IsReady()
        {
            foreach (var input in InputNodes)
            {
                if (!input.IsConnected())
                {
                    return false;
                }
            }
            foreach (var output in OutputNodes)
            {
                if (!output.IsConnected())
                {
                    return false;
                }
            }

            return true;
        }

        public BitmapImage RetrieveBitmapIcon()
        {
            return this.Icon.ToBitmap();
        }

        public virtual void ReadXml(XmlReader reader)
        {
            int attrCount = reader.AttributeCount;
            _Guid = Guid.Parse(reader.GetAttribute("Guid"));
            var inputCount = Int32.Parse(reader.GetAttribute("InputCount"));
            var outputCount = Int32.Parse(reader.GetAttribute("OutputCount"));
            var stateCount = Int32.Parse(reader.GetAttribute("StateCount"));
            if (reader.MoveToAttribute("ParameterCount"))
                this.ParameterCount = Int32.Parse(reader.GetAttribute("ParameterCount"));

            reader.ReadStartElement();
            if (inputCount > 0)
            {
                reader.ReadStartElement();
                foreach (var item in InputNodes)
                {
                    item.Identity = Guid.Parse(reader.GetAttribute("Identifier"));
                    reader.Read();
                }
                reader.ReadEndElement();
            }

            if (outputCount > 0)
            {
                reader.ReadStartElement();
                foreach (var item in OutputNodes)
                {
                    item.Identity = Guid.Parse(reader.GetAttribute("Identifier"));
                    reader.Read();
                }
                reader.ReadEndElement();
            }

            if (stateCount > 0)
            {
                XmlSerializer keySerializer = new XmlSerializer(typeof(SerializableDictionary<Guid, ModuleState>));
                Store = (SerializableDictionary<Guid, ModuleState>)keySerializer.Deserialize(reader);
            }

            ReadParameter(reader);
        }

        public virtual void WriteParameter(XmlWriter writer)
        {

        }

        public virtual void ReadParameter(XmlReader reader)
        {

        }

        public virtual void WriteXml(XmlWriter writer)
        {
            try
            {
                writer.WriteAttributeString("Type", GetType().AssemblyQualifiedName);
                writer.WriteAttributeString("Guid", Guid.ToString());
                writer.WriteAttributeString("StateCount", Store.Count.ToString());
                writer.WriteAttributeString("InputCount", InputNodes.Count.ToString());
                writer.WriteAttributeString("OutputCount", OutputNodes.Count.ToString());
                writer.WriteAttributeString("ParameterCount", ParameterCount.ToString());

                if (InputNodes.Count > 0)
                {
                    writer.WriteStartElement("InputNodes");
                    foreach (var item in InputNodes)
                    {
                        writer.WriteStartElement("InputNode");
                        writer.WriteAttributeString("Identifier", item.Identity.ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                }
                if (OutputNodes.Count > 0)
                {
                    writer.WriteStartElement("OutputNodes");
                    foreach (var item in OutputNodes)
                    {
                        writer.WriteStartElement("OutputNode");
                        writer.WriteAttributeString("Identifier", item.Identity.ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                if (Store.Count > 0)
                {

                    XmlSerializer keySerializer = new XmlSerializer(typeof(SerializableDictionary<Guid, ModuleState>));
                    keySerializer.Serialize(writer, Store);
                }

                WriteParameter(writer);
            }
            catch (Exception ex)
            {

                logger.Error(ex);
            }

        }

        protected void AddInputNode(BaseInputNode node)
        {
            node.OnConnected += OnNodeConnected;
            node.OnDisconnect += OnNodeDisconnected;
            InputNodes.Add(node);
        }

        protected void AddOutputNode(BaseOutputNode node)
        {
            node.OnConnected += OnNodeConnected;
            node.OnDisconnect += OnNodeDisconnected;
            OutputNodes.Add(node);
        }

        private void OnNodeConnected()
        {
            // if all nodes are connected, set them to ready
            if (IsReady())
            {
                foreach (var input in InputNodes)
                {
                    input.Ready = true;
                }

                foreach (var output in OutputNodes)
                {
                    output.Ready = true;
                }
            }
        }

        private void OnNodeDisconnected()
        {
            foreach (var input in InputNodes)
            {
                input.Ready = false;
            }
            foreach (var output in OutputNodes)
            {
                output.Ready = false;
            }
        }
    }
}
