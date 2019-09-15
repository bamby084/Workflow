using JdSuite.Common.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;

namespace AppWorkflow.Controls
{
    /// <summary>
	/// A structure used to represent module connections on a canvas
	/// </summary>
	public class Connection
    {
        public ContextMenu contextMenu;
        public RenderNode End;
        public BaseInputNode Input;
        public ConnectingLine Line;
        public BaseOutputNode Output;
        public RenderNode Start; // A visual containing an output node

        // A visual containing an input node
        public WorkflowCanvas Canvas { get; set; }

        public Connection(WorkflowCanvas canvas, RenderNode startPoint, RenderNode endPoint, ConnectingLine line)
        {
            this.Canvas = canvas;
            Line = line;
            if (startPoint.Node.GetConnectionType() == INodeConnectionType.OUTPUT &&
                endPoint.Node.GetConnectionType() == INodeConnectionType.INPUT)
            {
                Input = (BaseInputNode)endPoint.Node;
                Output = (BaseOutputNode)startPoint.Node;

                Start = startPoint;
                End = endPoint;
            }
            else if (startPoint.Node.GetConnectionType() == INodeConnectionType.INPUT
                && endPoint.Node.GetConnectionType() == INodeConnectionType.OUTPUT)
            {
                Input = (BaseInputNode)startPoint.Node;
                Output = (BaseOutputNode)endPoint.Node;
                Start = endPoint;
                End = startPoint;
            }

            contextMenu = new ContextMenu();
            MenuItem item = new MenuItem();
            item.Header = "Disconnect";
            contextMenu.Items.Add(item);

            item.Click += delegate
            {
                Disconnect();
            };
        }

        internal Connection() { }
        internal static XMLConnection ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            reader.ReadStartElement(); // OutputNode
            XMLConnection.XMLNode output = ReadXmlNode(reader);
            XMLConnection.XMLNode input = ReadXmlNode(reader);

            XMLConnection connection = new XMLConnection();
            connection.Output = output;
            connection.Input = input;
            return connection;
        }

        public static void WriteXml(XmlWriter writer, Connection connection)
        {
            writer.WriteStartElement("OutputNode");
            WriteXmlNode(writer, connection.Start);
            writer.WriteEndElement();
            writer.WriteStartElement("InputNode");
            WriteXmlNode(writer, connection.End);
            writer.WriteEndElement();
            // Not writing line points to save on file size. Regenerate on read.
        }

        public bool Connect()
        {
            if (!IsValid())
            {
                return false;
            }

            bool result = Output.ConnectTo(Input);
            if (result)
            {
                OnConnect();
            }
            return result;
        }

        public bool ContainsModuleVisual(CanvasModule canvasModule)
        {
            if (Start.Parent == canvasModule)
            {
                return true;
            }
            else if (End.Parent == canvasModule)
            {
                return true;
            }
            return false;
        }

        public void Disconnect()
        {
            OnDisconnect();
            Output.Disconnect();
            Input.Disconnect();
            this.Canvas.PruneDeadConnections();
        }

        public bool IsValid()
        {
            if (Input == null)
            {
                return false;
            }

            if (Output == null)
            {
                return false;
            }

            if (!Output.CanConnectTo(Input))
            {
                return false;
            }

            return true;
        }

        public void OnConnect()
        {
            ((BaseNode)Start.Node).OnDisconnect += OnNodeDisconnect;
            ((BaseNode)End.Node).OnDisconnect += OnNodeDisconnect;
            Start.Visual.ContextMenu = contextMenu;
            End.Visual.ContextMenu = contextMenu;
            Line.SetContextMenu(contextMenu);
        }

        public void OnDisconnect()
        {
            ((BaseNode)Start.Node).OnDisconnect -= OnNodeDisconnect;
            ((BaseNode)End.Node).OnDisconnect -= OnNodeDisconnect;
            try
            {
                Start.Visual.ContextMenu = null;
                End.Visual.ContextMenu = null;
                Line.SetContextMenu(null);
            }
            catch (InvalidOperationException) { } // For catching irrelevant Finalizing AppDomain errors
        }

        private void OnNodeDisconnect()
        {
            Disconnect();
        }

        private static XMLConnection.XMLNode ReadXmlNode(XmlReader reader)
        {
            reader.MoveToContent();
            XMLConnection.XMLNode node = new XMLConnection.XMLNode();
            node.ModuleName = reader.GetAttribute("ModuleName");
            node.index = int.Parse(reader.GetAttribute("Index"));
            node.Identity = Guid.Parse(reader.GetAttribute("Identity"));
            reader.Skip();
            return node;
        }

        private static void WriteXmlNode(XmlWriter writer, RenderNode node)
        {
            writer.WriteAttributeString("ModuleName", node.Parent.TextBoxDisplayName.Text);
            var idx = -1;
            if (node.Node.GetConnectionType() == INodeConnectionType.OUTPUT)
            {
                idx = node.Parent.OutputNodes.IndexOf(node);
            }
            else
            {
                idx = node.Parent.InputNodes.IndexOf(node);
            }

            if (idx < 0)
            {
                throw new XmlException("Could not serialize connection: Node not found in module");
            }
            writer.WriteAttributeString("Index", idx.ToString());
            writer.WriteAttributeString("Identity", node.Node.Identity.ToString());
        }
    }
}
