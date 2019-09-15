using JdSuite.Common.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace AppWorkflow.Controls
{
    /// <summary>
    /// A structure used in defining visual connections to each module
    /// </summary>
    public class RenderNode
    {
        public INode Node;
        public CanvasModule Parent;
        public Ellipse Visual;

        public RenderNode(CanvasModule parent, Ellipse shape, INode node)
        {
            Parent = parent;
            Visual = shape;
            Node = node;
        }
    }
}
