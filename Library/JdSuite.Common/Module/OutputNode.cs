using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdSuite.Common.Module
{
    public class OutputNode : BaseOutputNode
    {
        public OutputNode(BaseModule module, String displayName, String extension) : base(module, displayName)
        {
            Extensions.Add(extension);
        }

        public override INodeType GetNodeType()
        {
            return INodeType.HOOK;
        }
    }
}
