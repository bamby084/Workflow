using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Controls
{
    /// <summary>
	/// A structure holding data necessary to reconstruct a connection from
	/// XML.
	/// </summary>
	internal class XMLConnection
    {
        public XMLNode Input;

        public XMLNode Output;

        public class XMLNode
        {
            // Index in array of the parent CanvasModule
            public int index;

            // Display name of the parent CanvasModule
            public string ModuleName;

            public Guid Identity { get; internal set; }
        }
    }
}
