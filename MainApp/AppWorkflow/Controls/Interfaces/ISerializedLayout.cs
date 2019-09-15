using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AppWorkflow.Controls
{
    internal interface ISerializedLayout : IXmlSerializable
    {
        void OnConfigureLayout();
    }
}
