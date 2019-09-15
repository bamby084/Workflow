using JdSuite.Common.Module;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AppWorkflow.Controls
{



    /// <summary>
    /// The main canvas used to display a module workflow
    /// </summary>
    public class WorkflowCanvas : Canvas, ISerializedLayout
    {
        NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(WorkflowCanvas));

        public static readonly Point CANVAS_SIZE = new Point(
            int.Parse(ConfigurationManager.ConnectionStrings["WorkflowCanvasWidth"]
                .ConnectionString),
            int.Parse(ConfigurationManager.ConnectionStrings["WorkflowCanvasHeight"]
                .ConnectionString));

        /// <summary>
        /// List of open connections. Must disconnect nodes and remove
        /// Connection.Line from the Canvas children before removing from
        /// this list.
        /// </summary>
        public List<Connection> Connections = new List<Connection>();

        public ScrollViewer Scroller;
        private static readonly double LINE_THICKNESS_FRACTION = 2.25d;
        private static readonly double SCROLL_FACTOR = 0.0005d;
        private static byte[,] pathFindingGrid = new byte[128, 128];
        private Point ConnectionStartPos;
        private bool IsPanning = false;
        private Point LastPanPos;
        private RenderNode StartNode;
        private ConnectingLine TempLine = new ConnectingLine(Brushes.Fuchsia);



        // Only true when XML deserialization is in progress
        private bool XmlInProgress = false;

        public WorkflowCanvas()
        {
            logger.Trace("Creating_object");

            Background = Brushes.LightGray;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;
            LayoutTransform = new ScaleTransform(1d, 1d);

            Width = CANVAS_SIZE.X;
            Height = CANVAS_SIZE.Y;

            TempLine.SetVisibility(Visibility.Collapsed);

            Children.Add(TempLine.Line);
            SetZIndex(TempLine.Line, 0);
            Children.Add(TempLine.Arrowhead);
            SetZIndex(TempLine.Arrowhead, 0);

            // All costs on the grid are the same, so set them to 1
            for (int x = 0; x <= pathFindingGrid.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= pathFindingGrid.GetUpperBound(0); y++)
                {
                    pathFindingGrid[x, y] = 1;
                }
            }

            this.Focusable = true;
        }

        public bool ContainsModule(BaseModule module)
        {
            foreach (var cv in Children.OfType<CanvasModule>())
            {
                if (cv.Module == module)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Finishes a temporary connection started by <see cref="StartConnection"/>.
        /// This will solidify a connection and add it to the active connections
        /// list to prevent redrawing of static lines.
        /// </summary>
        /// <param name="node">The node.</param>
        public void FinishConnection(RenderNode node)
        {


            ConnectingLine connectingLine = new ConnectingLine(TempLine.Line.Stroke);
            connectingLine.Line.StrokeThickness = node.Visual.Width / LINE_THICKNESS_FRACTION;

            Connection connection = new Connection(this, StartNode, node, connectingLine);

            if (connection.IsValid())
            {
                bool success = connection.Connect();
                if (success)
                {
                    RedrawLine(connection);

                    Connections.Add(connection);
                    Children.Add(connection.Line.Line);
                    Children.Add(connection.Line.Arrowhead);
                    connection.Start.Parent.OnModuleMove += OnConnectedModuleMove;
                    connection.End.Parent.OnModuleMove += OnConnectedModuleMove;
                }
            }
            // else let it fall out of scope to be garbage collected
            StartNode = null;
            TempLine.SetVisibility(Visibility.Collapsed);
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void OnConfigureLayout()
        {
            // notify CanvasModules
            foreach (var module in Children.OfType<CanvasModule>())
            {
                module.OnConfigureLayout();
            }

            foreach (var connection in Connections)
            {
                RedrawLine(connection);
            }
            XmlInProgress = false;
        }

        /// <summary>
        /// Prunes dead connections and their connected lines, if any.
        /// </summary>
        public void PruneDeadConnections()
        {
            Connections.RemoveAll((Connection connection) =>
            {
                if (!connection.Output.IsConnected() &&
                    !connection.Input.IsConnected())
                {
                    Children.Remove(connection.Line.Line);
                    Children.Remove(connection.Line.Arrowhead);
                    return true;
                }
                return false;
            });
        }

        public void ReadXml(XmlReader reader)
        {
            XmlInProgress = true;
            reader.MoveToContent();
            double zoom = double.Parse(reader.GetAttribute("Zoom"));

            // CanvasModules
            reader.ReadStartElement();
            int count = int.Parse(reader.GetAttribute("Count"));
            if (count > 0)
            {
                reader.ReadStartElement();
                for (int i = 0; i < count; i++)
                {
                    CanvasModule module = new CanvasModule();
                    module.ReadXml(reader);
                    Children.Add(module);
                    reader.ReadEndElement();
                }
                reader.ReadEndElement();
            }
            else
            {
                reader.Skip();
            }

            // Connections
            count = int.Parse(reader.GetAttribute("Count"));
            if (count > 0)
            {
                reader.ReadStartElement();
                for (int i = 0; i < count; i++)
                {
                    AddConnectionFromXml(Connection.ReadXml(reader));
                    reader.ReadEndElement();
                }
                reader.Skip();
            }
            else
            {
                reader.Skip();
            }

            ScaleTransform scale = LayoutTransform as ScaleTransform;
            scale.ScaleX = zoom;
            scale.ScaleY = zoom;
        }

        public void WriteXml(XmlWriter writer)
        {
            logger.Trace("Serializing to xml");
            // Zoom is equal on both axes so just use one
            writer.WriteAttributeString("Zoom", ((ScaleTransform)LayoutTransform).ScaleX.ToString());
            writer.WriteStartElement("CanvasModules");
            var modules = Children.OfType<CanvasModule>();
            writer.WriteAttributeString("Count", modules.Count().ToString());
            foreach (var module in modules)
            {
                logger.Info("Writing module xml {0}", module.TextBoxDisplayName);
                writer.WriteStartElement("CanvasModule");
                module.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteStartElement("Connections");
            writer.WriteAttributeString("Count", Connections.Count.ToString());
            foreach (var connection in Connections)
            {
                writer.WriteStartElement("Connection");
                Connection.WriteXml(writer, connection);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Sets the canvas module position relative to this canvas instance.
        /// </summary>
        /// <param name="module">The child module to move.</param>
        /// <param name="pos">The relative position.</param>
        public void SetCanvasModulePos(CanvasModule module, Point pos)
        {
            Rect viewport = VisualHelper.GetBoundingBox(this);
            Rect bbox = VisualHelper.GetBoundingBox(module);

            if (bbox.BottomRight.Y + pos.Y > viewport.Bottom)
            {
                pos.Y = viewport.Bottom - bbox.Bottom;
            }

            if (bbox.BottomRight.X + pos.X > viewport.Right)
            {
                pos.X = viewport.Right - bbox.Right;
            }

            if (pos.Y < 0)
            {
                pos.Y = 0;
            }
            if (pos.X < 0)
            {
                pos.X = 0;
            }

            // logger.Trace($"Setting Module_Position ({pos.X},{pos.Y}) Module= {module.DisplayName}");
            SetLeft(module, pos.X);
            SetTop(module, pos.Y);
            SetZIndex(module, 1);
        }

        /// <summary>
        /// Starts a new connection from the specified node. This will trigger
        /// a temporary line to be drawn from the starting node to the mouse
        /// cursor. If this node is already connected, this method will have
        /// no effect.
        /// </summary>
        /// <param name="node">The starting node.</param>
        public void StartConnection(RenderNode node)
        {
            // Cannot connect an already-connected node
            if (node.Node.IsConnected())
            {
                return;
            }
            logger.Info($"Starting connection NodeParent:{node.Parent.TextBoxDisplayName.Text} NodeName: {node.Node.GetDisplayName()}");

            StartNode = node;
            TempLine.SetVisibility(Visibility.Visible);
            TempLine.SetBrush(node.Visual.Fill);
            TempLine.Line.StrokeThickness = node.Visual.Width / LINE_THICKNESS_FRACTION;

            ConnectionStartPos = node.Visual.TransformToAncestor(this).Transform(new Point(0, 0));
            ConnectionStartPos.Y += node.Visual.ActualHeight / 2;

            if (node.Node.GetConnectionType() == INodeConnectionType.INPUT)
            {
                ConnectionStartPos.X -= node.Visual.ActualWidth / 4;
                TempLine.Redraw(Mouse.GetPosition(this), ConnectionStartPos);
                TempLine.SetCanvasPos(Mouse.GetPosition(this), ConnectionStartPos);
            }
            else
            {
                ConnectionStartPos.X += node.Visual.ActualWidth / 2;
                TempLine.Redraw(ConnectionStartPos, Mouse.GetPosition(this));
                TempLine.SetCanvasPos(ConnectionStartPos, Mouse.GetPosition(this));
            }
        }



        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            e.Handled = true;
            // Keyboard.ClearFocus();
            IsPanning = true;
            LastPanPos = LayoutTransform.Transform(e.GetPosition(this));

            //Added by Shakeel to set a module inactive
            if (e.OriginalSource == this)
            {
                var activeModule = this.Children.OfType<CanvasModule>().FirstOrDefault(x => x.IsActive);

                if (activeModule != null)
                {
                    activeModule.IsActive = false;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (IsPanning)
            {
                e.Handled = true;
                var delta = LastPanPos - LayoutTransform.Transform(e.GetPosition(this));
                Scroller.ScrollToHorizontalOffset(Scroller.HorizontalOffset + delta.X);
                Scroller.ScrollToVerticalOffset(Scroller.VerticalOffset + delta.Y);
            }
            if (TempLine.Visibility == Visibility.Visible)
            {
                e.Handled = true;
                if (StartNode.Node.GetConnectionType() == INodeConnectionType.INPUT)
                {
                    TempLine.Redraw(e.GetPosition(this), ConnectionStartPos);
                    TempLine.SetCanvasPos(e.GetPosition(this), ConnectionStartPos);
                }
                else
                {
                    TempLine.Redraw(ConnectionStartPos, e.GetPosition(this));
                    TempLine.SetCanvasPos(ConnectionStartPos, e.GetPosition(this));
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (IsPanning)
            {
                IsPanning = false;
                e.Handled = true;
            }
            if (TempLine.Visibility != Visibility.Collapsed)
            {
                // Must be drawing a temp line
                e.Handled = true;
                TempLine.SetVisibility(Visibility.Collapsed);

                // Attempt to get a RenderNode under the mouse
                // Ensure that the RenderNode isn't from the same CanvasModule
                RenderNode endpoint = null;
                VisualTreeHelper.HitTest(this, null, (HitTestResult result) =>
                {
                    if (result.VisualHit.GetType().IsAssignableFrom(typeof(Ellipse)))
                    {
                        var ellipse = result.VisualHit as Ellipse;
                        if (!StartNode.Parent.ContainsNodeVisual(ellipse))
                        {
                            endpoint =
                            VisualHelper.FindParentOfType<CanvasModule>(ellipse)
                            .GetRenderNode(ellipse);

                            return HitTestResultBehavior.Stop;
                        }
                    }
                    return HitTestResultBehavior.Continue;
                }, new PointHitTestParameters(e.GetPosition(this)));

                if (endpoint != null)
                {
                    FinishConnection(endpoint);
                }
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Keyboard.ClearFocus();
            this.Focus();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            var center = new Point((ActualWidth + 20) / 2, (ActualHeight + 20) / 2);
            var centerOffset = this.LayoutTransform.Transform(center);

            double delta = e.Delta * SCROLL_FACTOR;
            ScaleTransform scaleTransform = LayoutTransform as ScaleTransform;
            scaleTransform.ScaleX += delta;
            scaleTransform.ScaleY += delta;
            if (scaleTransform.ScaleX < 1)
            {
                scaleTransform.ScaleX = 1;
                scaleTransform.ScaleY = 1;
            }
            UpdateLayout();

            var newCenterOffset = this.LayoutTransform.Transform(center);
            var deltaOffset = newCenterOffset - centerOffset;

            Scroller.ScrollToHorizontalOffset(Scroller.HorizontalOffset + deltaOffset.X);
            Scroller.ScrollToVerticalOffset(Scroller.VerticalOffset + deltaOffset.Y);
        }

        protected override void OnVisualChildrenChanged(
            DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            if (visualAdded != null &&
                visualAdded.GetType().IsAssignableFrom(typeof(CanvasModule)))
            {
                OnAddCanvasModule((CanvasModule)visualAdded);
            }

            if (visualRemoved != null &&
                visualRemoved.GetType().IsAssignableFrom(typeof(CanvasModule)))
            {
                OnRemoveCanvasModule((CanvasModule)visualRemoved);
            }
        }

        private void AddConnectionFromXml(XMLConnection xmlConnection)
        {
            // Find node endpoints for the connection
            var canvasModules = Children.OfType<CanvasModule>();
            RenderNode start = null;
            RenderNode end = null;
            foreach (var canvasModule in canvasModules)
            {
                if (canvasModule.TextBoxDisplayName.Text == xmlConnection.Output.ModuleName)
                {
                    start = canvasModule.OutputNodes.Where(t => t.Node.Identity == xmlConnection.Output.Identity).FirstOrDefault();
                }
                else if (canvasModule.TextBoxDisplayName.Text == xmlConnection.Input.ModuleName)
                {
                    end = canvasModule.InputNodes.Where(t => t.Node.Identity == xmlConnection.Input.Identity).FirstOrDefault();
                }

                if (start != null && end != null)
                {
                    break;
                }
            }

            if (start == null || end == null)
            {
                throw new XmlException("Cannot re-establish module connection:" +
                    " one of the endpoints were not found");
            }

            // Finalize connection
            StartConnection(start);
            FinishConnection(end);
        }

        /// <summary>
        /// Adjusts the module display names to correspond with how many of the
        /// same modules are on the canvas to prevent duplicate names.
        /// </summary>
        public void AdjustModuleDisplayNames()
        {
            logger.Trace("Adjusting Display name");
            // Append number to module display name reflecting the number of
            // modules on this canvas of the same type
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            foreach (var child in Children)
            {
                if (child is CanvasModule cm)
                {
                    string moduleName = cm.Module.DisplayName;

                    if (dictionary.ContainsKey(moduleName))
                    {
                        int i = 1;
                        string newmodulename = moduleName + i.ToString();

                        while (dictionary.ContainsKey(newmodulename))
                            newmodulename = moduleName + (i++).ToString();

                        cm.Module.DisplayName = newmodulename;
                        dictionary.Add(cm.Module.DisplayName, 1);
                    }
                    else
                    {
                        dictionary.Add(moduleName, 1);
                    }
                }
            }
        }

        private void OnAddCanvasModule(CanvasModule module)
        {
            string dataDir = Properties.Settings.Default.data_folder;
            if (dataDir.Last() != System.IO.Path.DirectorySeparatorChar)
            {
                dataDir += System.IO.Path.DirectorySeparatorChar;
            }
            module.Module.DataDir = dataDir;
            ValidateDataFolder();

            module.WfCanvas = this;
            module.Scroller = Scroller;
            module.OnNodeStartConnectionLine += StartConnection;
            SetCanvasModulePos(module, Mouse.GetPosition(this));
            SetZIndex(module, 1);

            if (!XmlInProgress)
            {
                AdjustModuleDisplayNames();
            }
        }

        private void ValidateDataFolder()
        {
            if (!System.IO.Directory.Exists(Properties.Settings.Default.data_folder))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(Properties.Settings.Default.data_folder);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Cannot create data_folder: {0}", Properties.Settings.Default.data_folder);
                    MessageBox.Show(string.Format("Cannot create data_folder: [{0}], please provide a valid data_folder path in AppWorkflow.exe.config",
                        Properties.Settings.Default.data_folder));
                }
            }
        }

        private void OnConnectedModuleMove(CanvasModule module)
        {
            foreach (var connection in Connections)
            {
                if (connection.ContainsModuleVisual(module))
                {
                    RedrawLine(connection);
                }
            }
        }

        private void OnRemoveCanvasModule(CanvasModule module)
        {
            module.OnNodeStartConnectionLine -= StartConnection;

            // Safe to just disconnect all nodes on the module instead of the
            // connection, as it disconnects on both ends anyway.
            module.Module.Disconnect();

            // Check each connection entry and remove the one that doesn't have
            // active connections
            PruneDeadConnections();
            AdjustModuleDisplayNames();
        }

        private void RedrawLine(Connection connection)
        {
            double halfWidth = connection.Start.Visual.Width / 2d;
            Point start = connection.Start.Visual.TransformToAncestor(this).Transform(new Point(halfWidth, halfWidth));
            Point end = connection.End.Visual.TransformToAncestor(this).Transform(new Point(0, halfWidth));
            end.X -= connection.Start.Visual.ActualWidth / 4;
            connection.Line.Redraw(start, end);
            connection.Line.SetCanvasPos(start, end);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {

            foreach (var canvasModule in this.Children.OfType<CanvasModule>().Where(x => x.Module.GetType().Name.Contains("DataOutput")))
            {
                canvasModule.Module.Run((int)e.Key);
            }


            base.OnKeyDown(e);
        }


    }
}
