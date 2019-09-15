using JdSuite.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Schema;

namespace AppWorkflow.Controls
{
    /// <summary>
	/// The parent of the Workflow Canvas, which is used to scroll when zoomed
	/// </summary>
	/// <seealso cref="System.Windows.Controls.ScrollViewer" />
	/// <seealso cref="AppWorkflow.Controls.ISerializedLayout" />
	public class WorkflowScrollViewer : ScrollViewer, ISerializedLayout
    {
        NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(WorkflowScrollViewer));

        public WorkflowCanvas ChildCanvas { get; set; }

        public Workflow ActiveWorkflow { get; set; }

        public double XmlScrollH;

        public double XmlScrollV;

        // Only valid when deserializing from XML
        public string XmlTabHeader;

        public WorkflowScrollViewer()
        {
            PanningMode = PanningMode.Both;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            DockPanel panel = new DockPanel();

            WorkflowCanvas canvas = new WorkflowCanvas();
            canvas.Scroller = this;
            panel.Children.Add(canvas);

            ChildCanvas = canvas;
            Content = panel;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void OnConfigureLayout()
        {
            ChildCanvas.OnConfigureLayout();
            ScrollToHorizontalOffset(XmlScrollH);
            ScrollToVerticalOffset(XmlScrollV);
        }

        public void ReadXml(XmlReader reader)
        {
            logger.Trace("Reading xml");

            reader.MoveToContent();
            XmlTabHeader = reader.GetAttribute("Title");
            XmlScrollH = double.Parse(reader.GetAttribute("ScrollH"));
            XmlScrollV = double.Parse(reader.GetAttribute("ScrollV"));
            reader.ReadStartElement();
            ChildCanvas.ReadXml(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            logger.Trace("Writing xml");
            var header = ((TabItem)Parent).Header as TabHeader;
            writer.WriteAttributeString("Title", (string)header.NameLabel.Content);
            writer.WriteAttributeString("ScrollH", HorizontalOffset.ToString());
            writer.WriteAttributeString("ScrollV", VerticalOffset.ToString());
            writer.WriteStartElement("WorkflowCanvas");
            ChildCanvas.WriteXml(writer);
            writer.WriteEndElement();
        }
    }
}
