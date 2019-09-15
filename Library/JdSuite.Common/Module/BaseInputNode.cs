using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdSuite.Common.Module
{
    public abstract class BaseInputNode : BaseNode
    {
        NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(BaseInputNode));

        public BaseInputNode(BaseModule module, string displayName) : base(module, displayName)
        {
            logger.Info($"Module:{module.DisplayName}");
            State = new ModuleState();
        }

        ~BaseInputNode()
        {
            if (!System.Environment.HasShutdownStarted)
            {
                Disconnect();
            }
        }

        public override INodeConnectionType GetConnectionType()
        {
            return INodeConnectionType.INPUT;
        }

        public ModuleState State { get; set; }
       

        /// <summary>
        /// Receive an incoming connection from an output node
        /// </summary>
        /// <param name="output">The output node to connect.</param>
        /// <returns>
        /// True if the extensions/connection types match
        /// and the connection was made.
        /// </returns>
        public virtual bool RecieveConnection(BaseOutputNode output)
        {
            logger.Info("Entered OutputNode: {0}",output.GetDisplayName());

            if (IsConnected())
            {
                if (Connector == output)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (this.CanConnectTo(output))
            {
                Connector = output;
                this.OnConnected();
                return true;
            }

            return false;
        }
    }
}
