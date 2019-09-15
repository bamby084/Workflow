using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using AdvancedDataGridView;
using JdSuite.Common.Module;

namespace Designer
{
    public partial class SchemaControl : UserControl
    {
        public SchemaControl()
        {
            InitializeComponent();
            this.Font = new System.Drawing.Font(
                        "Segoe UI",
                        9,
                        System.Drawing.FontStyle.Regular,
                        System.Drawing.GraphicsUnit.Point,
                        ((byte)(0)));
        }

        private void AddNode(TreeGridNode parent, XElement element, Field schema)
        {
            var nodeName = element.Name.LocalName;
            var type = from node in schema.ChildNodes
                       where node.Name == nodeName
                       select node.Type;
            var typeStr = type.FirstOrDefault();
            typeStr = typeStr == null ? "" : typeStr;

            object[] param = { nodeName, element.HasElements ? "" : typeStr.Replace("xs:", ""), element.HasElements ? "Array" : element.Value };
            var root = parent != null ? parent.Nodes.Add(param) : treeGridView1.Nodes.Add(param);

            foreach (var child in element.Elements())
            {
                AddNode(root, child, schema);
            }
        }

        public void RefreshData(Field schema, XDocument data, int recordIndex)
        {
            treeGridView1.Nodes.Clear();
            var root = data.Root.Elements().ElementAtOrDefault(recordIndex);
            AddNode(null, root, schema);
            treeGridView1.Nodes.First()?.Expand();
        }
    }
}
