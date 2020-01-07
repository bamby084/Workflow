using System;

namespace Designer.DesignerItems
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class XmlOutAttribute: Attribute
    {
        public string Name { get; set; }
    }
}
