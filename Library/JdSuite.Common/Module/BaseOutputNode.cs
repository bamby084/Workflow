using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdSuite.Common.Module
{
    public abstract class BaseOutputNode : BaseNode
    {
        public ModuleState State
        {
            get
            {
                if (Module.Store.ContainsKey(this.Identity))
                    return Module.Store[this.Identity];
                return null;
            }
            set
            {
                Module.Store[this.Identity] = value;
            }
        }

        public BaseOutputNode(BaseModule module, string displayName) : base(module, displayName)
        {
            OnConnected += delegate { };
        }

        /// <summary>
        /// Connects an output node to an input.
        /// </summary>
        /// <param name="input">The input node to connect.</param>
        /// <returns>
        /// True if the extensions match and the connection was made.
        /// </returns>
        public virtual bool ConnectTo(BaseInputNode input)
        {
            if (IsConnected())
            {
                if (Connector != input)
                {
                    return false;
                }
                return true;
            }

            if (this.CanConnectTo(input))
            {
                bool granted = input.RecieveConnection(this);
                if (granted)
                {
                    Connector = input;
                    this.OnConnected();
                    return true;
                }
                return false;
            }

            return false;
        }

        public override void Disconnect()
        {
            base.Disconnect();
        }

        public override INodeConnectionType GetConnectionType()
        {
            return INodeConnectionType.OUTPUT;
        }
    }
}
