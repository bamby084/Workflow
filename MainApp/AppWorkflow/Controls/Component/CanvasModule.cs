using JdSuite.Common;
using JdSuite.Common.Logging;
using JdSuite.Common.Logging.Enums;
using JdSuite.Common.Module;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace AppWorkflow.Controls
{
    /// <summary>
	/// A module's representation on a workflow canvas
	/// </summary>
	public class CanvasModule : Grid, ISerializedLayout
    {
        NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(CanvasModule));

        public TextBox TextBoxDisplayName { get; private set; } = new TextBox();

        public List<RenderNode> InputNodes { get; private set; } = new List<RenderNode>();
        public List<RenderNode> OutputNodes { get; private set; } = new List<RenderNode>();

        public BaseModule Module { get; private set; }

        public Action<CanvasModule> OnModuleMove { get; set; }

        public Action<RenderNode> OnNodeStartConnectionLine { get; set; }

        /// <summary>
        /// During drag/drop process this is parent
        /// </summary>
        public DragCanvas Parent2 { get; set; }


        public Rectangle PageRect { get; set; }
        public ScrollViewer Scroller { get; set; }

        /// <summary>
        /// This is actual parent of CanvasModule
        /// </summary>
        public WorkflowCanvas WfCanvas { get; set; }

        private Border PageImageBorder { get; set; }

        public void SetBorderColor(Brush brush)
        {
            bool isOutputModule = Module.GetType().Name.Contains("DataOutput");
            if (!isOutputModule)
                this.PageImageBorder.BorderBrush = brush;
        }

        private Image PageImage;

        // Only valid when deserializing from XML
        public double XmlLeft;

        public double XmlTop;
        private bool IsDragged { get; set; } = false;
        private Grid ModulePage;
        private Point RelMousePos;
        private ToolTip tooltip;

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;


                if (_isActive)
                {
                    SetBorderColor(Brushes.Red);
                }
                else
                {
                    SetBorderColor(Brushes.Black);

                    IsDragged = false;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasModule"/> class.
        /// Empty constructor for internal use only for XML deserialization.
        /// </summary>
        public CanvasModule()
        {
            logger = NLog.LogManager.GetLogger(nameof(CanvasModule));
            logger.Info("Creating object 1 ClassName=CanvasModule");
        }

        public CanvasModule(BaseModule module)
        {
            Initialize(module);
            logger = NLog.LogManager.GetLogger("Canvas-" + module.DisplayName);
            logger.Info("Creating object 2 ClassName=CanvasModule");
            this.TextBoxDisplayName.KeyDown += DisplayName_KeyDown;
            this.TextBoxDisplayName.LostKeyboardFocus += DisplayName_LostKeyboardFocus;
            //this.MaxWidth = 80;
            // this.MaxHeight = 120;
        }

        private void DisplayName_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

            WfCanvas.AdjustModuleDisplayNames();
        }

        private void DisplayName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
            }


        }

        /// <summary>
        /// Determines whether this instance contains the specified ellipse visual.
        /// </summary>
        /// <param name="ellipse">The ellipse to test against.</param>
        /// <returns>
        ///   <c>true</c> if this instance contains the specified ellipse visual; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsNodeVisual(Ellipse ellipse)
        {
            return GetRenderNode(ellipse) != null;
        }

        public RenderNode GetRenderNode(Ellipse ellipse)
        {
            foreach (var node in InputNodes)
            {
                if (node.Visual == ellipse)
                {
                    return node;
                }
            }

            foreach (var node in OutputNodes)
            {
                if (node.Visual == ellipse)
                {
                    return node;
                }
            }

            return null;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void OnConfigureLayout()
        {
            Canvas.SetLeft(this, XmlLeft);
            Canvas.SetTop(this, XmlTop);
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            string name = reader.GetAttribute("Name");
            XmlLeft = double.Parse(reader.GetAttribute("Left"));
            XmlTop = double.Parse(reader.GetAttribute("Top"));
            reader.ReadStartElement(); // Module
            var typeStr = reader.GetAttribute("Type");
            var type = Type.GetType(typeStr, (asmName) =>
            {
                return AppDomain.CurrentDomain.GetAssemblies().Where(z => z.FullName == asmName.FullName).FirstOrDefault();
            }, null, true);

            var module = ((App)Application.Current).CreateModule(type);
            if (module == null)
            {
                throw new XmlException("Requested module is not available");
            }
            module.ReadXml(reader);
            reader.Skip(); // Since there are no elements, advance to next node

            Initialize((BaseModule)module);
            TextBoxDisplayName.Text = name;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", TextBoxDisplayName.Text);
            writer.WriteAttributeString("Left", Canvas.GetLeft(this).ToString());
            writer.WriteAttributeString("Top", Canvas.GetTop(this).ToString());
            XmlSerializer keySerializer = new XmlSerializer(typeof(BaseModule));
            keySerializer.Serialize(writer, Module);
        }

        protected void Initialize(BaseModule module)
        {
            logger.Trace($"Creating visual controls for module:{module.DisplayName}");
            var mainWindow = ((MainWindow)Application.Current.MainWindow);
            module.RequestStateUpdate += mainWindow.Module_RequestStateUpdate;
            module.GetState += mainWindow.Module_GetWorkflow;


            ColumnDefinitions.Add(new ColumnDefinition());
            ColumnDefinitions.Add(new ColumnDefinition());
            ColumnDefinitions.Add(new ColumnDefinition());
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions.Add(new RowDefinition());
            this.Module = module;

            // Tooltip is hooked up in each relevant memeber function
            tooltip = new ToolTip();
            tooltip.Content = module.GetToolTipDescription();
            logger.Info("Creating BaseModule Page ");

            ModulePage = CreatePage(module);
            ModulePage.UpdateLayout();
            StackPanel leftStack = CreateNodeStack(module.GetInputNodes().Cast<INode>().ToList(),
                InputNodes, HorizontalAlignment.Right);

            StackPanel rightStack = CreateNodeStack(module.GetOutputNodes().Cast<INode>().ToList(),
                OutputNodes, HorizontalAlignment.Left);


            TextBoxDisplayName.Background = Brushes.Transparent;
            TextBoxDisplayName.BorderBrush = Brushes.Transparent;
            TextBoxDisplayName.IsReadOnly = false;
            TextBoxDisplayName.MaxWidth = PageRect.Width;

            Binding binding = new Binding("DisplayName");
            binding.Source = this.Module;
            binding.Mode = BindingMode.TwoWay;
            binding.BindsDirectlyToSource = true;
            binding.NotifyOnSourceUpdated = true;
            binding.NotifyOnTargetUpdated = true;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            TextBoxDisplayName.SetBinding(TextBox.TextProperty, binding);
            TextBoxDisplayName.DataContext = this.Module;


            TextBoxDisplayName.TextWrapping = TextWrapping.Wrap;
            TextBoxDisplayName.TextAlignment = TextAlignment.Center;
            TextBoxDisplayName.FontSize = 12;
            TextBoxDisplayName.HorizontalAlignment = HorizontalAlignment.Center;

            leftStack.VerticalAlignment = VerticalAlignment.Center;
            rightStack.VerticalAlignment = VerticalAlignment.Center;

            SetColumn(leftStack, 0);
            SetRow(leftStack, 0);

            SetColumn(ModulePage, 1);
            SetRow(ModulePage, 0);

            SetColumn(rightStack, 2);
            SetRow(rightStack, 0);

            SetColumn(TextBoxDisplayName, 1);
            SetRow(TextBoxDisplayName, 1);

            Children.Add(leftStack);
            Children.Add(rightStack);
            Children.Add(TextBoxDisplayName);
            Children.Add(ModulePage);

            MouseMove += Page_OnMouseMove;

            this.Focusable = true;
            this.KeyDown += KeyBoard_KeyDown;
            this.LostFocus += CanvasModule_LostFocus;

            this.ContextMenu = new ContextMenu();
            this.ContextMenu.Items.Add(new MenuItem().Header = "Add To Favorites");

            //ModulePage.ContextMenu = new ContextMenu();
            //ModulePage.ContextMenu.Items.Add(new MenuItem() { Header = "Hello" });
        }

        private void CanvasModule_LostFocus(object sender, RoutedEventArgs e)
        {
            logger.Trace("ModulePage.Focus()=False");
            SetBorderColor(Brushes.Black);
        }

        private void KeyBoard_KeyDown(object sender, KeyEventArgs e)
        {


            logger.Info("Key {0}", e.Key);

            if (e.Key == Key.F5)
            {
                var workflow = GetCurrentWorkFlow();
                workflow.Command = (int)e.Key;

                logger.Info("Executing F5(Run Data) command");
                Module.Execute(workflow);
                e.Handled = true;

            }
            else if (e.Key == Key.F6)
            {
                var workflow = GetCurrentWorkFlow();
                workflow.Command = (int)e.Key;
                logger.Info("Executing F6(View Data) command");
                Module.Execute(workflow);
                e.Handled = true;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (Parent != WfCanvas)
            {
                return;
            }

            // Keyboard.ClearFocus();
            //e.Handled = true;

            if (e.OriginalSource == PageImage)
            {
                if (!this.IsFocused)
                {
                    if (this.Focus())
                    {
                        IsActive = true;
                    }
                }
            }
            else if (e.OriginalSource.GetType() == typeof(Ellipse))
            {
                IsActive = true;
            }



            if (e.ClickCount > 1)
            {
                OnDoubleClick(this, e);
                return;
            }

            /*
            if (MouseButtonState.Pressed == e.RightButton)
            {
                logger.Info("this.PageImage.ContextMenu.IsOpen = true");
                if (!this.PageImage.ContextMenu.IsOpen)
                    this.PageImage.ContextMenu.IsOpen = true;
            }
            */

            if (!IsDragged && MouseButtonState.Pressed == e.LeftButton)
            {
                RelMousePos = e.MouseDevice.GetPosition(this);
                // Use a hit test to determine if node
                HitTestResult result = VisualTreeHelper.HitTest(
                    this, e.MouseDevice.GetPosition(this));

                if (result.VisualHit.GetType().IsAssignableFrom(typeof(Ellipse)))
                {
                    Ellipse shape = (Ellipse)result.VisualHit;
                    RenderNode node = null;
                    foreach (var inputNode in InputNodes)
                    {
                        if (inputNode.Visual == shape)
                        {
                            node = inputNode;
                        }
                    }
                    foreach (var outputNode in OutputNodes)
                    {
                        if (outputNode.Visual == shape)
                        {
                            node = outputNode;
                        }
                    }
                    logger.Info("Calling delete OnNodeDragStart to start drawing connection line ");
                    OnNodeStartConnectionLine(node ?? throw new Exception("Hit ellipse shape is not in a node map"));
                    return;
                }
                // If not, just drag the module
                IsDragged = true;
                logger.Info("IsDragged ={0}", IsDragged);

                WorkflowCanvas.SetZIndex(this, 2);
                var window = Window.GetWindow(this);

                if (window != null)
                    window.MouseUp += OnCanvasDragDrop;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            IsDragged = false;

            if (MouseButtonState.Pressed == e.RightButton)
            {
                logger.Info("this.PageImage.ContextMenu.IsOpen = true");
                if (!this.PageImage.ContextMenu.IsOpen)
                    this.PageImage.ContextMenu.IsOpen = true;
            }
        }
        /// <summary>
        /// Creates the node visual as an ellipse.
        /// </summary>
        /// <param name="color">The color of the ellipse.</param>
        /// <returns>The newly created ellipse</returns>
        private static Ellipse CreateNodeVisual(string color)
        {
            Ellipse ellipse = new Ellipse();
            SolidColorBrush brush = new SolidColorBrush();

            // Use reflection to cast string to color property type
            var reflection = typeof(Colors).GetProperty(color);
            brush.Color = (Color)reflection.GetValue(null, null);

            ellipse.Fill = brush;
            ellipse.StrokeThickness = 1;
            ellipse.Stroke = Brushes.Gray;
            ellipse.Width = 16;
            ellipse.Height = 16;

            return ellipse;
        }

        /// <summary>
        /// Creates the node stack for use on either the left or right side of
        /// the canvas module.
        /// </summary>
        /// <param name="nodes">The input or output nodes attached to a module.</param>
        /// <param name="renderNodes">The list to insert the new RenderNode into.</param>
        /// <returns>The newly created RenderNode stack/</returns>
        private StackPanel CreateNodeStack(List<INode> nodes, List<RenderNode> renderNodes, HorizontalAlignment ellipseAlignment)
        {
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Vertical;
            stack.VerticalAlignment = VerticalAlignment.Stretch;
            foreach (var node in nodes)
            {
                string colorName = "";
                switch (node.GetNodeType())
                {
                    case INodeType.LINE:
                        colorName = ConfigurationManager
                            .ConnectionStrings["LineColor"]
                            .ConnectionString;
                        break;

                    case INodeType.HOOK:
                        colorName = ConfigurationManager
                            .ConnectionStrings["HookColor"]
                            .ConnectionString;
                        break;

                    default:
                        colorName = "Fuchsia";
                        Logger.Log(Severity.WARN, LogCategory.CONTROL, "No color assigned for input node type");
                        break;
                }
                var innerContainer = new StackPanel();
                innerContainer.Orientation = Orientation.Vertical;
                innerContainer.VerticalAlignment = VerticalAlignment.Top;
                innerContainer.HorizontalAlignment = ellipseAlignment;

                var label = new Label();
                label.Padding = new Thickness(0);
                label.Content = node.GetDisplayName();
                label.VerticalAlignment = VerticalAlignment.Bottom;
                label.HorizontalAlignment = ellipseAlignment;
                innerContainer.Children.Add(label);

                var circleControl = CreateNodeVisual(colorName);
                circleControl.VerticalAlignment = VerticalAlignment.Top;
                circleControl.HorizontalAlignment = ellipseAlignment;
                innerContainer.Children.Add(circleControl);

                stack.Children.Add(innerContainer);
                renderNodes.Add(new RenderNode(this, circleControl, node));
            }

            return stack;
        }

        private Grid CreatePage(IModule module)
        {
            logger.Info("Entered DisplayName:{0}", module.DisplayName);
            Grid pageGrid = new Grid();
            pageGrid.MaxWidth = 80;
            pageGrid.MaxHeight = 120;
            RowDefinition row = new RowDefinition();
            row.Height = GridLength.Auto;

            ColumnDefinition col = new ColumnDefinition();
            col.Width = GridLength.Auto;
            pageGrid.RowDefinitions.Add(row);
            pageGrid.ColumnDefinitions.Add(col);

            PageRect = new Rectangle();
            PageRect.Stroke = Brushes.Gray;
            PageRect.StrokeThickness = 1;
            PageRect.Fill = Brushes.GhostWhite;
            PageRect.HorizontalAlignment = HorizontalAlignment.Center;
            PageRect.VerticalAlignment = VerticalAlignment.Center;
            PageRect.Height = 120;
            PageRect.Width = 80;
            Grid.SetRow(PageRect, 0);
            Grid.SetColumn(PageRect, 0);
            pageGrid.Children.Add(PageRect);

            BitmapImage bmp = module.RetrieveBitmapIcon();

            var pageBorder = new Border();
            pageBorder.BorderThickness = new Thickness(2);
            pageBorder.BorderBrush = Brushes.Black;
            pageBorder.Width = 70;
            pageBorder.Height = 110;
            pageBorder.Padding = new Thickness(5);
            Grid.SetRow(pageBorder, 0);
            Grid.SetColumn(pageBorder, 0);
            Canvas.SetZIndex(pageBorder, 9999);

            this.PageImageBorder = pageBorder;

            pageGrid.Children.Add(PageImageBorder);

            PageImage = new Image();
            PageImage.HorizontalAlignment = HorizontalAlignment.Center;
            PageImage.VerticalAlignment = VerticalAlignment.Center;


            PageImage.Source = new TransformedBitmap(bmp, VisualHelper.GetImageScaleWithBounds(PageRect.Width * 2, bmp));


            pageBorder.Child = PageImage;


            PageRect.ToolTip = tooltip;
            PageImage.ToolTip = tooltip;

            PageImage.ContextMenu = new ContextMenu();

            module.ModulePageBorder = pageBorder;


            var item = new MenuItem();
            item.Click += MenuItemOpen_Click;
            item.Header = "Open";
            PageImage.ContextMenu.Items.Add(item);

            item = new MenuItem();
            item.Click += MenuItemRename_Click;
            item.Header = "Rename";
            PageImage.ContextMenu.Items.Add(item);

            item = new MenuItem();
            item.Click += MenuItemDelete_Click;
            item.Header = "Delete";
            PageImage.ContextMenu.Items.Add(item);


            module.SetContextMenuItems(PageImage.ContextMenu);

            return pageGrid;
        }

        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UnRegisterEvents();

                Parent.RemoveChild(this);
                WfCanvas.RemoveChild(this);
                Parent2.RemoveChild(this);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void MenuItemRename_Click(object sender, RoutedEventArgs e)
        {
            ((UIElement)TextBoxDisplayName).Focus();
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            Module.OpenWindow(GetCurrentWorkFlow());
        }

        /// <summary>
        /// Called when the dropped onto an element from a drag operation.
        /// </summary>
        private void OnCanvasDragDrop(object sender, MouseButtonEventArgs e)
        {
            logger.Info("Entered");

            Window window = Window.GetWindow(this);
            IsDragged = false;
            logger.Info("IsDragged={0}", IsDragged);
            e.Handled = true;

            // Modules are only allowed to be dropped on the workflow canvas
            // So if it's not being dropped withing the scroll view bounds,
            // let it fall out of scope
            var relPoint = e.GetPosition(Scroller);
            if (relPoint.X < 0 || relPoint.Y < 0 ||
                relPoint.X > Scroller.ActualWidth ||
                relPoint.Y > Scroller.ActualWidth)
            {
                WfCanvas.Children.Remove(this);
            }
            if (window != null)
                window.MouseUp -= OnCanvasDragDrop;
        }


        private Workflow GetCurrentWorkFlow()
        {
            var window = (MainWindow)Window.GetWindow(this);
            var workflow = ((WorkflowScrollViewer)window.TabControl.SelectedContent).ActiveWorkflow;
            window.CacheCurrentWorkflow();

            return workflow;
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Use a hit test to determine if page
            HitTestResult result = VisualTreeHelper.HitTest(
                this, e.MouseDevice.GetPosition(this));

            var workflow = GetCurrentWorkFlow();

            if (result.VisualHit is Visual visual)
            {
                object data = null;
                workflow.Command = (int)JdSuite.Common.Module.Commands.DoubleClick;

                if (visual.IsDescendantOf(ModulePage))
                {

                    data = Module.Execute(workflow);
                }
                else if (visual is Grid grid)
                {
                    if (grid == ModulePage)
                    {
                        data = Module.Execute(workflow);
                    }
                }

                if (data != null)
                {
                    string extension = Module.GetExtension();
                    if (extension == null)
                    {
                        extension = Module.GetOutputNodes()[0]
                        .GetSupportedExtensions()[0];
                    }
                    string tempPath = System.IO.Path.Combine(
                        System.IO.Path.GetTempPath(),
                        "AppWorkflowTemp." + extension);
                    using (Stream stream = File.Open(tempPath, FileMode.Create))
                    {
                        // This may or may not produce a readable format
                        // depending on what the data is
                        var formatter = new BinaryFormatter();
                        formatter.Serialize(stream, data);
                    }
                    Process.Start(tempPath);
                }
            }
        }

        private void Page_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDragged)
            {
                return;
            }
            e.Handled = true;



            if (e.LeftButton == MouseButtonState.Pressed && IsDragged)
            {
                OnModuleMove?.Invoke(this);
                Point mousePos = e.MouseDevice.GetPosition(WfCanvas);
                Point offsetPos = new Point(mousePos.X - RelMousePos.X, mousePos.Y - RelMousePos.Y);

                WfCanvas.SetCanvasModulePos(this, offsetPos);
            }
        }

        public void UnRegisterEvents()
        {
            var mainWindow = ((MainWindow)Application.Current.MainWindow);

            if (mainWindow != null)
            {
                this.Module.RequestStateUpdate -= mainWindow.Module_RequestStateUpdate;
                Module.GetState -= mainWindow.Module_GetWorkflow;
            }
        }
    }
}
