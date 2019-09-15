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

namespace DataOutput.Controls
{
    public partial class SchemaControl : UserControl
    {
        NLog.ILogger logger = NLog.LogManager.GetLogger("OutputSchemaControl");

        public Field Schema { get; set; }
        public XDocument Data { get; set; }

        public int RecordIndex { get; set; }
        public int PageSize { get; set; } = 5;

        public int RecordCount
        {
            get
            {
                if (Data == null) return 0;
                return Data.Root.Elements().Count();
            }
        }

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
            try
            {
                var nodeName = element.Name.LocalName;

                var schemaNode = schema;
                if (schemaNode.Name != nodeName)
                {
                    schemaNode = schema.ChildNodes.FirstOrDefault(x => x.Name == nodeName);
                }

                //var type = from node in schema.ChildNodes where node.Name == nodeName select node.Type;
                //var typeStr=type.FirstOrDefault();

                var typeStr = "";
                if (schemaNode != null)
                {
                    typeStr = schemaNode.DataType;
                }
                //typeStr = typeStr == null ? "" : typeStr;

                object[] param = { nodeName, element.HasElements ? "Composit" : typeStr.Replace("xs:", ""), element.HasElements ? "Array" : element.Value };
                var root = parent != null ? parent.Nodes.Add(param) : treeGridView1.Nodes.Add(param);

                foreach (var child in element.Elements())
                {
                    // AddNode(root, child, schema);
                    AddNode(root, child, schemaNode);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"XElement_processing_error {element.Name.LocalName}");
            }
        }

        public bool RefreshData()
        {
            bool result = true;
            logger.Info("Processing XDocument data Page Size: {0} RecordIndex:{1}", PageSize, RecordIndex);
            try
            {
                treeGridView1.Nodes.Clear();
                int count = PageSize;
                do
                {
                    var root = Data.Root.Elements().ElementAtOrDefault(RecordIndex);
                    if (root == null)
                        break;
                    AddNode(null, root, Schema);
                    RecordIndex++;
                    count--;
                } while (count > 0);

                treeGridView1.Nodes.First()?.Expand();
            }
            catch (Exception ex)
            {
                result = false;
                logger.Error(ex, $"XML Node Display Error NodeIndex: {RecordIndex}");
            }

            return result;
        }
    }
}
