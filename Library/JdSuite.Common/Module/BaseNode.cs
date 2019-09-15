using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace JdSuite.Common.Module
{
    public abstract class BaseNode : INode
    {
        NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(BaseNode));

        public Guid Identity { get; set; } = Guid.NewGuid();

        BaseNode _connector = null;
        public BaseNode Connector
        {
            get { return _connector; }
            set
            {
                _connector = value;
                if (_connector != null)
                {
                    logger.Info($"Connector={_connector.GetDisplayName()}");
                }
            }
        }

        /// <summary>
        /// Callback for new connection events. It is safe to attempt access
        /// of the resource by the time this is called. The resource may be
        /// null if the module is not ready however.
        /// </summary>
        /// <seealso cref="Module.BaseNode.IsReady"/>
        public Action OnConnected { get; set; }

        public Action OnDisconnect { get; set; }

        /// <summary>
        /// Callback when the owning module is ready for communication
        /// </summary>
        public Action OnReady { get; set; }

        protected List<string> Extensions = new List<string>();

        private string DisplayName;

        internal BaseModule Module { get; set; }

        private bool ready = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseNode"/> class.
        /// </summary>
        /// <param name="module">The parent module.</param>
        /// <param name="displayName">The display name for this instance.</param>
        protected BaseNode(BaseModule module, string displayName)
        {
            Module = module;
            DisplayName = displayName;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the parent <see cref="BaseModule"/> is ready for communcation.
        /// Set is intended for internal use by the owning <see cref="BaseModule"/> only.
        /// </summary>
        /// <value>
        ///   <c>true</c> if ready; otherwise, <c>false</c>.
        /// </value>
        public bool Ready
        {
            get
            {
                return ready;
            }
            set
            {
                ready = value;
                if (ready)
                {
                    OnReady?.Invoke();
                }
            }
        }

        public bool CanConnectTo(INode node)
        {
            bool result = node.GetConnectionType() == this.GetConnectionType();

            logger.Info($"{node.GetConnectionType()} == {this.GetConnectionType()} ?{result}");

            if (result)
            {
                return false;
            }

            result = node.GetNodeType() != this.GetNodeType();

            logger.Info($"{node.GetNodeType()} == {this.GetNodeType()} ?{result}");

            if (result)
            {
                return false;
            }

            foreach (var ext in Extensions)
            {
                if (node.GetSupportedExtensions().Contains(ext))
                {
                    logger.Info($"True, {node.GetDisplayName()} has supported extension {ext}");
                    return true;
                }
            }

            logger.Info($"False, {node.GetDisplayName()}");
            return false;
        }

        public bool IsConnected()
        {
            return Connector != null;
        }

        public virtual void Disconnect()
        {
            if (Connector == null)
            {
                return;
            }
            var c = Connector;
            Connector = null;
            OnDisconnect();
            c.Disconnect();
        }

        public abstract INodeConnectionType GetConnectionType();

        public INode GetConnector()
        {
            return Connector;
        }

        public String GetDisplayName()
        {
            return DisplayName;
        }

        public BaseModule GetModule()
        {
            return Module;
        }

        public abstract INodeType GetNodeType();

        public virtual XmlSchema GetSchema()
        {
            return null;
        }

        public List<string> GetSupportedExtensions()
        {
            return Extensions;
        }

        public virtual void ReadXml(XmlReader reader)
        { }

        public virtual void WriteXml(XmlWriter writer)
        { }
    }
}
