using Designer.Controls;
using Designer.Tools;
using Designer.Variables;
using JdSuite.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using osk2.TypographicFonts;
using System.Xml;
using System.Reflection;
using System.Drawing;
using System.ComponentModel;
using System.IO;
using System.Drawing.Text;
using System.Xml.Serialization;
using JdSuite.Common.Logging.Enums;
using JdSuite.Common.Module;

namespace Designer
{

    public enum TvItemCategory
    {
        VARIABLE_SYSTEM,
        VARIABLE_DATA,
        PAGES,
        DYNAMIC_COMMUNICATION,
        ELEMENTS,
        PARAGRAPH_STYLES,
        TEXT_STYLES,
        FONTS,
        BORDER_STYLES,
        COLORS,
        IMAGES,
        TABLES,
        LINE_STYLES,
        FILL_STYLES,
        BLOCK_STYLES,
        FLOW_STYLES
    };

    /// <summary>
    /// Interaction logic for Designer.xaml
    /// </summary>
    public partial class DesignerPage : UserControl
    {
        public const string DRAGDROP_FORMAT = "JohnDeiuliis.Designer.LayoutTreeItem";
        public LibraryTreeItem DraggedItem = null;
        public Pages Pages = new Pages();
        private PageBase ActivePage = null;
        private List<ContextMenu> LtContextMenus = new List<ContextMenu>();
        private List<LibraryTreeItem> TvItemElements = new List<LibraryTreeItem>();

        public int _page_id = 1;
        public int _font_id = 1;
        public int _image_id = 1;
        public int _para_id = 1;
        public int _container_id = 1;
        public int _text_id = 1;
        public int _line_id = 1;
        public int _border_id = 1;
        public int _table_id = 1;
        public int _color_id = 1;
        public int _block_id = 1;
        public int _flow_id = 1;


        public static DesignerPage self;

        public DesignerPage()
        {
            InitializeComponent();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            self = this;

            foreach (var sysVar in SystemVariable.LIST)
            {
                var node = GenerateTvItemElement<CanvasVariableSystem>(sysVar.Name, TvItemCategory.VARIABLE_SYSTEM);
                TreeViewSystemVars.Items.Add(node);
            }

            PanelCanvas.AllowDrop = true;
            PanelCanvas.Drop += PanelCanvas_Drop;

            //TreeViewElements.Items.Add(GenerateTvItemElement<CanvasElement>("Element", TvItemCategory.ELEMENTS));
            //TreeViewElements.Items.Add(GenerateTvItemElement<CanvasRichTextBox>("Text", TvItemCategory.ELEMENTS));
            //TreeViewElements.Items.Add(GenerateTvItemElement<CanvasImage>("Image", TvItemCategory.ELEMENTS));

            InitFont();
            InitColorComboBox();

        }

        public Page AddNewPage()
        {
            var page = new Page(_page_id);
            _page_id++;
            AddPage(page);
            return page;
        }

        internal void DeleteBlock(LibraryTreeItem item)
        {
            BlockStyle pg = item.Resources["BlockStyleRef"] as BlockStyle;
            if (pg != null)
                BlockStyles.Remove(pg);
            if (item != null)
            {
                LibraryTreeItem parent = item.Parent as LibraryTreeItem;
                parent.Items.Remove(item);
                PropertiesPanel.Children.Clear();
                parent.IsSelected = true;

                pg.DeleteBorder();
            }
        }

        internal void DeleteFlow(LibraryTreeItem item)
        {
            FlowStyle pg = item.Resources["FlowStyleRef"] as FlowStyle;
            if (pg != null)
                FlowStyles.Remove(pg);
            if (item != null)
            {
                LibraryTreeItem parent = item.Parent as LibraryTreeItem;
                parent.Items.Remove(item);
                PropertiesPanel.Children.Clear();
                parent.IsSelected = true;
            }
        }

        /// <summary>
        /// Loads the specified pages. It is best to create a new Designer
        /// if this needs to be called more than once to make sure state
        /// is cleared.
        /// </summary>
        /// <param name="pages">The pages to load.</param>
        public void Load(Pages pages)
        {
            Pages.Clear();
            Pages.AddRange(pages);
            foreach (var page in Pages)
            {
                if (page is Page p)
                {
                    TreeViewPages.Items.Add(GenerateTvPageElement(p));
                }
                else
                {
                    Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add page to Layout Tree: Unhandled page type.");
                }
            }
            Pages.LayoutProperties = pages.LayoutProperties;
            SetActivePage(Pages[0]);
        }

        /// <summary>
        /// Loads the default Designer view.
        /// </summary>
        public void LoadDefault()
        {
            Load(new Pages { new Page(1) });
        }

        /// <summary>
        /// Sets the active layout for display in the properties panel.
        /// </summary>
        /// <param name="layout">The layout to display in the properties panel, or null to clear the properties panel.</param>
        public void SetActiveProperties(FrameworkElement layout)
        {
            PropertiesPanel.Children.Clear();
            PropertiesPanel.Children.Add(layout);
        }

        public UIElement GetActiveProperties()
        {
            if (PropertiesPanel.Children.Count == 0)
                return new UIElement();

            return PropertiesPanel.Children[0];
        }

        /// <summary>
        /// Unpacks the schema into the Data TreeView TreeView. Undefined behavior will
        /// occur if this is called more than once.
        /// </summary>
        /// <param name="schema">The schema to expand.</param>
        public void UnpackSchema(Field schema)
        {
            TreeViewData.Items.Clear();
            TreeViewData.Items.Add(BuildSubTree(schema));
        }

        private void AddPage(PageBase page)
        {
            if (page is Page p)
            {
                TreeViewPages.Items.Add(GenerateTvPageElement(p));
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add page to Layout Tree: Unhandled page type.");
            }
            Pages.Add(page);
            SetActivePage(page);
        }

        private TreeViewItem BuildSubTree(Field element)
        {
            var name = element.Name;

            if (element.ChildNodes.Count == 0)
            {
                var leaf = GenerateTvItemElement<CanvasVariableData>(name, TvItemCategory.VARIABLE_DATA);
                return leaf;
            }

            var root = new TreeViewItem();
            root.Header = name;

            foreach (var child in element.ChildNodes)
            {
                root.Items.Add(BuildSubTree(child));
            }

            return root;
        }

        private void Canvas_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DRAGDROP_FORMAT))
            {
                return;
            }

            var DesignerControl = MainWindow.Instance.DesignerControl;
            var item = DesignerControl.DraggedItem;
            if (item.ElementType.IsSubclassOf(typeof(CanvasControl)))
            {
                e.Handled = true;
                var control = (CanvasControl)Activator.CreateInstance(item.ElementType);
                if (control.GetType().IsSubclassOf(typeof(CanvasVariable)))
                {
                    CanvasVariable cv = control as CanvasVariable;
                    cv.Label.Content = item.Header;
                }
                // Set ID to the next count of the same type of controls
                var amt = ActivePage.CanvasControls.Count(c =>
                {
                    if (c.GetType() == control.GetType())
                    {
                        return true;
                    }
                    return false;
                });
                control.SetId(amt + 1);

                ActivePage.AddCanvasItem(control, e.GetPosition(ActivePage.Canvas));
            }
        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            ActivePage.ScaleToFit(Scroller);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //SetActiveProperties(ActivePage.GetPropertyLayout());
            //e.Handled = true;
            if (TreeViewRoot == null) return;
            LibraryTreeItem treeItem = TreeViewRoot.SelectedItem as LibraryTreeItem;
            if (treeItem == null) return;
            if (treeItem.ItemCategory == TvItemCategory.BLOCK_STYLES)
            {
                BlockStyle bs = treeItem.Resources["BlockStyleRef"] as BlockStyle;

                if (DesignerPage.self.chkBlockArrow.IsChecked == false)
                    return;

                bs.Canvas_MouseLeftButtonDown();
                e.Handled = true;
                return;
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //SetActiveProperties(ActivePage.GetPropertyLayout());
            //e.Handled = true;
            if (TreeViewRoot == null) return;
            LibraryTreeItem treeItem = TreeViewRoot.SelectedItem as LibraryTreeItem;
            if (treeItem == null) return;

            if (treeItem.ItemCategory == TvItemCategory.BLOCK_STYLES)
            {
                BlockStyle bs = treeItem.Resources["BlockStyleRef"] as BlockStyle;

                if (DesignerPage.self.chkBlockArrow.IsChecked == false)
                    return;

                bs.Canvas_MouseLeftButtonUp();
                e.Handled = true;
                return;
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            //SetActiveProperties(ActivePage.GetPropertyLayout());
            //e.Handled = true;
            if (TreeViewRoot == null) return;
            LibraryTreeItem treeItem = TreeViewRoot.SelectedItem as LibraryTreeItem;
            if (treeItem == null)
                return;
            if (treeItem.ItemCategory == TvItemCategory.BLOCK_STYLES)
            {
                BlockStyle bs = treeItem.Resources["BlockStyleRef"] as BlockStyle;
                if (DesignerPage.self.chkBlockArrow.IsChecked == false)
                    return;

                Application.Current.Dispatcher.BeginInvoke(
                            new Action<DesignerPage>((c) =>
                            {
                                bs.Canvas_MouseMove();
                            }),
                                        System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                                        this
                                );

                e.Handled = true;
                return;
            }
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var mPos = e.GetPosition(ActivePage.Canvas);
            var DesignerControl = MainWindow.Instance.DesignerControl;
            DesignerControl.LabelMouseX.Content = "X: " + string.Format("{0:N3}", mPos.X);
            DesignerControl.LabelMouseY.Content = "Y: " + string.Format("{0:N3}", mPos.Y);
        }

        private void CanvasControls_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshLayoutTree();
        }

        private TvItemElement<Page> GenerateTvPageElement(Page page)
        {
            var element = GenerateTvItemElement<Page>("Page", TvItemCategory.PAGES);
            element.label.Content = page.Id;
            element.Resources.Add("PageRef", page);
            element.MouseMove -= TreeViewItem_MouseMove;
            element.MouseDoubleClick += TvPageElement_MouseDoubleClick;
            element.Selected += TvPageElement_Selected;
            element.imgTreeAdd.Visibility = Visibility.Visible;
            return element;
        }

        private void TvPageElement_Selected(object sender, RoutedEventArgs e)
        {
            PageBase page = (sender as LibraryTreeItem).Resources["PageRef"] as PageBase;
            SetActiveProperties(page.GetPropertyLayout());
            e.Handled = true;
        }

        private void TvPageElement_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var element = sender as LibraryTreeItem;
            RequestSetActivePage(element.Resources["PageRef"] as Page);
            e.Handled = true;
        }

        private TvItemElement<T> GenerateTvItemElement<T>(string displayName, TvItemCategory category) where T : ILayoutNode
        {
            var newTreeViewItem = new TvItemElement<T>(category);
            if (category == TvItemCategory.VARIABLE_SYSTEM)
            {
                newTreeViewItem.Header = displayName;
            }
            else
                newTreeViewItem.label.Content = displayName;

            newTreeViewItem.imgLibrary.Visibility = Visibility.Visible;
            newTreeViewItem.txtbox.Focusable = true;
            newTreeViewItem.EditMode = true;
            //newTreeViewItem.Header = displayName;
            //newTreeViewItem.ContextMenu = new ContextMenu();

            newTreeViewItem.MouseMove += TreeViewItem_MouseMove;

            TvItemElements.Add(newTreeViewItem);
            return newTreeViewItem;
        }

        private void TreeViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            var treeViewItem = sender as LibraryTreeItem;
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Console.WriteLine("Pressed");
                DraggedItem = treeViewItem;
                DragDrop.DoDragDrop(treeViewItem, new DataObject(DRAGDROP_FORMAT, treeViewItem.Header), DragDropEffects.Move);
            }
        }

        private void Page_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            bool shouldFit = false;
            if (e.PropertyName == "PageWidth")
            {
                LabelPageWidth.Content = "W: " + string.Format("{0:N2}", ActivePage.PageWidth / ImageHelper.DPI) + " in.";
                shouldFit = true;
            }
            else if (e.PropertyName == "PageHeight")
            {
                LabelPageHeight.Content = "H: " + string.Format("{0:N2}", ActivePage.PageHeight / ImageHelper.DPI) + " in.";
                shouldFit = true;
            }

            if (shouldFit && Scroller.ActualWidth > 0)
            {
                ActivePage.ScaleToFit(Scroller);
            }
        }

        private void PanelCanvas_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DRAGDROP_FORMAT))
            {
                return;
            }

            var item = DraggedItem;

            if (!item.ElementType.IsSubclassOf(typeof(PageBase)))
            {
                return;
            }

            var page = (PageBase)Activator.CreateInstance(item.ElementType);
            page.SetId(Pages.Count + 1);
            AddPage(page);
            e.Handled = true;
        }

        private void PanelCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ActivePage == null)
            {
                return;
            }

            var center = new System.Windows.Point((PanelCanvas.ActualWidth + 20) / 2, (PanelCanvas.ActualHeight + 20) / 2);
            var centerOffset = PanelCanvas.RenderTransform.Transform(center);

            double delta = e.Delta * MainWindow.SCROLL_FACTOR;
            ScaleTransform scaleTransform = ActivePage.LayoutTransform;
            scaleTransform.ScaleX += delta;
            scaleTransform.ScaleY += delta;
            Scroller.UpdateLayout();

            var newCenterOffset = PanelCanvas.RenderTransform.Transform(center);
            var deltaOffset = newCenterOffset - centerOffset;

            Scroller.ScrollToHorizontalOffset(Scroller.HorizontalOffset + deltaOffset.X);
            Scroller.ScrollToVerticalOffset(Scroller.VerticalOffset + deltaOffset.Y);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var msg = "Saved successfully.";
            try
            {
                MainWindow.Instance.CacheWorkflow();
            }
            catch (Exception ex)
            {
                msg = "Could not save to the current workflow: " + ex.Message;
                Logger.Log(Severity.ERROR, LogCategory.APP, msg);
            }
            finally
            {
                MessageBox.Show(msg, "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RefreshLayoutTree()
        {
            foreach (var item in TvItemElements)
            {
                if (item.ElementType.IsSubclassOf(typeof(PageBase)))
                {
                    continue;
                }

                var itemName = item.Header as string;

                if (item.ElementType.IsSubclassOf(typeof(CanvasControl)))
                {
                    /*
                    item.ContextMenu.ItemsSource = ActivePage.CanvasControls.Where((control) =>
					{
						return itemName == control.TreeViewName;
					}).Select((control) =>
					{
						return control.MenuItem;
					});
                    */
                }
                else
                {
#if DEBUG
                    //throw new NotImplementedException();
#else
					Logger.Log(Severity.WARN, LogCategory.APP, 
						$"Item of type {item.ElementType.ToString()} " +
						$"is not recognized as a valid TreeView Item Category. " +
						$"This element will not show up in the Layout Tree context menu.");
#endif
                }
            }
        }

        public FlowStyles FlowStyles = new FlowStyles();

        public void AddNewFlow(LibraryTreeItem item2)
        {
            BlockStyle pg = item2.Resources["BlockStyleRef"] as BlockStyle;

            FlowStyle ps = new FlowStyle(_flow_id, pg);
            _flow_id++;

            if (ps is FlowStyle p)
            {
                var item = GenerateTvFlowStyleElement(p);
                item2.Items.Add(item);
                item2.IsExpanded = true;
                item.IsSelected = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add FlowStyle to Layout Tree: Unhandled FlowStyle type.");
            }
            FlowStyles.Add(ps);
        }

        private TvItemElement<FlowStyle> GenerateTvFlowStyleElement(FlowStyle p)
        {
            var element = GenerateTvItemElement<FlowStyle>("FlowStyle", TvItemCategory.FLOW_STYLES);
            element.label.Content = p.Id;
            element.Resources.Add("FlowStyleRef", p);
            element.imgTreeAdd.Visibility = Visibility.Visible;
            element.Selected += TvFlowStyleElement_Selected;
            return element;
        }

        private void TvFlowStyleElement_Selected(object sender, RoutedEventArgs e)
        {
            FlowStyle pg = (sender as LibraryTreeItem).Resources["FlowStyleRef"] as FlowStyle;
            SetActiveProperties(pg.GetPropertyLayout());
            e.Handled = true;
        }

        /// <summary>
        /// Set the active page of the Designer View.
        /// </summary>
        /// <param name="page">The page to set.</param>
        /// <returns>True if the page was set, False if the page is not a child of this view.</returns>
        public bool RequestSetActivePage(PageBase page)
        {
            if (!Pages.Contains(page))
            {
                return false;
            }
            SetActivePage(page);
            return true;
        }

        private void SetActivePage(PageBase page)
        {
            PanelCanvas.Children.Clear();
            PanelCanvas.Children.Add(page.GetRootElement());
            if (ActivePage != null)
            {
                page.PropertyChanged -= Page_PropertyChanged;
                page.Canvas.Loaded -= Canvas_Loaded;
                page.Canvas.SizeChanged -= Canvas_Loaded;
                page.Canvas.MouseLeftButtonDown -= Canvas_MouseLeftButtonDown;
                page.Canvas.MouseMove -= Canvas_MouseMove;
                page.Canvas.MouseLeftButtonUp -= Canvas_MouseLeftButtonUp;
                page.Canvas.PreviewMouseMove -= Canvas_PreviewMouseMove;
                page.Canvas.MouseLeftButtonUp -= Canvas_MouseLeftButtonUp;
                page.Canvas.Drop -= Canvas_Drop;
                page.CanvasControls.CollectionChanged -= CanvasControls_CollectionChanged;
            }

            page.PropertyChanged += Page_PropertyChanged;
            page.CanvasControls.CollectionChanged += CanvasControls_CollectionChanged;
            page.Canvas.Loaded += Canvas_Loaded;
            page.Canvas.SizeChanged += Canvas_Loaded;
            page.Canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            page.Canvas.MouseMove += Canvas_MouseMove;
            page.Canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
            page.Canvas.PreviewMouseMove += Canvas_PreviewMouseMove;
            page.Canvas.Drop += Canvas_Drop;

            ActivePage = page;
            SetActiveProperties(ActivePage.GetPropertyLayout());

            Application.Current.Dispatcher.BeginInvoke(
                new Action<DesignerPage>((c) =>
                {
                    PageRuler.Set(page, Scroller);
                }),
                System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                this
            );
            RefreshLayoutTree();
        }

        private void TreeViewPages_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender as TreeViewItem).IsSelected)
            {
                return;
            }

            SetActiveProperties(Pages.LayoutProperties);
            e.Handled = true;
        }

        public BlockStyles BlockStyles = new BlockStyles();
        public void AddNewBlock(BlockStyle bs, string pageId)
        {
            Page page = null;
            LibraryTreeItem item2 = null;
            foreach (TvItemElement<Page> item in TreeViewPages.Items)
            {
                if (item != null)
                {
                    page = item.Resources["PageRef"] as Page;
                    if (page.Id == pageId)
                    {
                        item2 = item;
                        break;
                    }
                }
            }

            if (page == null || item2 == null) return;

            if (bs is BlockStyle p)
            {
                p.SetPage(page);
                p.CreateBorderCanvas();

                var item = GenerateTvBlockStyleElement(p);
                item2.Items.Add(item);
                item2.IsExpanded = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add ParagraphStyle to Layout Tree: Unhandled ParagraphStyle type.");
            }
            BlockStyles.Add(bs);
        }

        public void AddNewBlock(LibraryTreeItem item2)
        {
            Page pg = item2.Resources["PageRef"] as Page;

            BlockStyle ps = new BlockStyle(_block_id, pg);
            _block_id++;

            if (ps is BlockStyle p)
            {
                var item = GenerateTvBlockStyleElement(p);
                item2.Items.Add(item);
                item2.IsExpanded = true;
                item.IsSelected = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add ParagraphStyle to Layout Tree: Unhandled ParagraphStyle type.");
            }
            BlockStyles.Add(ps);
        }

        private TvItemElement<BlockStyle> GenerateTvBlockStyleElement(BlockStyle p)
        {
            var element = GenerateTvItemElement<BlockStyle>("BlockStyle", TvItemCategory.BLOCK_STYLES);
            element.label.Content = p.Id;
            element.Resources.Add("BlockStyleRef", p);
            element.imgTreeAdd.Visibility = Visibility.Visible;
            element.imgTreeAdd.ToolTip = "Add Flow...";
            element.Selected += TvBlockStyleElement_Selected;
            return element;
        }

        private void TvBlockStyleElement_Selected(object sender, RoutedEventArgs e)
        {
            BlockStyle pg = (sender as LibraryTreeItem).Resources["BlockStyleRef"] as BlockStyle;
            SetActiveProperties(pg.GetPropertyLayout());
            e.Handled = true;
        }

        private void CtxMenuPages_Click(object sender, RoutedEventArgs e)
        {
            AddNewPage();
        }

        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 

        public void treeItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var container = (sender as LibraryTreeItem).Resources["ContainerRef"];
            if (container is ContainerEx p)
            {
                SetActivePage(p);
            }
            else
            {
                var flow = (sender as LibraryTreeItem).Resources["FlowRef"];
                if (flow is FlowEx p2)
                {
                    SetActivePage(p2);
                }
            }
        }

        private void Styles_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BorderStyle")
            {
                BlockStyles.LayoutProperties_PropertyChanged(sender, e);
            }
        }

        public Containers Containers = new Containers();
        public Containers DrawingContainers = new Containers();
        private TvItemElement<ContainerEx> GenerateTvContainerElement(ContainerEx ps)
        {
            var element = GenerateTvItemElement<ContainerEx>("Container", TvItemCategory.ELEMENTS);
            element.label.Content = ps.Id;
            element.Resources.Add("ContainerRef", ps);


            //element.MouseMove -= TreeViewItem_MouseMove;
            //element.MouseDoubleClick += TvPageElement_MouseDoubleClick;
            element.Selected += TvContainersElement_Selected;

            return element;
        }

        private void TvContainersElement_Selected(object sender, RoutedEventArgs e)
        {
            ContainerEx pg = (sender as LibraryTreeItem).Resources["ContainerRef"] as ContainerEx;
            SetActiveProperties(pg.GetPropertyLayout());
            e.Handled = true;
        }

        private void AddNewContainer()
        {
            ContainerEx ps = new ContainerEx(_container_id);
            ps.PropertyChanged += Styles_PropertyChanged;

            _container_id++;

            if (ps is ContainerEx p)
            {
                TreeViewElements.Items.Add(GenerateTvContainerElement(p));
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add ParagraphStyle to Layout Tree: Unhandled ParagraphStyle type.");
            }

            Containers.Add(ps);
        }

        public void CtxMenuElements_Click(object sender, RoutedEventArgs e)
        {
            AddNewContainer();
        }

        public FlowExs Flows = new FlowExs();
        public FlowExs DrawingFlows = new FlowExs();
        private TvItemElement<FlowEx> GenerateTvFlowElement(FlowEx ps)
        {
            var element = GenerateTvItemElement<FlowEx>("Flow", TvItemCategory.ELEMENTS);
            element.label.Content = ps.Id;
            element.Resources.Add("FlowRef", ps);


            //element.MouseMove -= TreeViewItem_MouseMove;
            //element.MouseDoubleClick += TvPageElement_MouseDoubleClick;
            element.Selected += TvFlowsElement_Selected;

            return element;
        }

        private void TvFlowsElement_Selected(object sender, RoutedEventArgs e)
        {
            FlowEx pg = (sender as LibraryTreeItem).Resources["FlowRef"] as FlowEx;
            SetActiveProperties(pg.GetPropertyLayout());
            e.Handled = true;
        }

        private void AddNewFlow()
        {
            FlowEx ps = new FlowEx(_flow_id);
            ps.PropertyChanged += Styles_PropertyChanged;

            ps.PageWidth = ActivePage.PageWidth;
            ps.PageHeight = ActivePage.PageHeight;

            _flow_id++;

            if (ps is FlowEx p)
            {
                TreeViewFlows.Items.Add(GenerateTvFlowElement(p));
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add Flow to Layout Tree: Unhandled Flow type.");
            }

            Flows.Add(ps);
        }

        public void CtxMenuFlows_Click(object sender, RoutedEventArgs e)
        {
            AddNewFlow();
        }

        public ColorStyles ColorStyles = new ColorStyles();

        private TvItemElement<ColorStyle> GenerateTvColorStyleElement(ColorStyle ps)
        {
            var element = GenerateTvItemElement<ColorStyle>("Colors", TvItemCategory.COLORS);
            element.label.Content = ps.Id;
            element.Resources.Add("ColorStyleRef", ps);
            //element.MouseMove -= TreeViewItem_MouseMove;
            //element.MouseDoubleClick += TvPageElement_MouseDoubleClick;
            element.Selected += TvColorStyleElement_Selected;
            return element;
        }

        private void TvColorStyleElement_Selected(object sender, RoutedEventArgs e)
        {
            ColorStyle pg = (sender as LibraryTreeItem).Resources["ColorStyleRef"] as ColorStyle;
            SetActiveProperties(pg.GetPropertyLayout());
            e.Handled = true;
        }

        private void AddNewColorStyle()
        {
            ColorStyle ps = new ColorStyle(_color_id);
            ps.PropertyChanged += Styles_PropertyChanged;
            _color_id++;

            if (ps is ColorStyle p)
            {
                var item = GenerateTvColorStyleElement(p);
                TreeViewColors.Items.Add(item);
                TreeViewColors.IsExpanded = true;
                item.IsSelected = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add ParagraphStyle to Layout Tree: Unhandled ParagraphStyle type.");
            }
            ColorStyles.Add(ps);

            cmbColorName.Items.Insert(0,
                new ColorInfo
                {
                    Color = ps.Color,
                    Name = ps.Id,
                    IsUser = true
                });
        }

        public void DeleteColor(LibraryTreeItem item)
        {
            ColorStyle pg = item.Resources["ColorStyleRef"] as ColorStyle;
            if (pg != null)
                ColorStyles.Remove(pg);
            if (item != null)
            {
                TreeViewColors.Items.Remove(item);
                PropertiesPanel.Children.Clear();
                TreeViewColors.IsSelected = true;
            }
        }

        public TextStyles TextStyles = new TextStyles();

        private TvItemElement<TextStyle> GenerateTvTextStyleElement(TextStyle ps)
        {
            var element = GenerateTvItemElement<TextStyle>("TextStyle", TvItemCategory.TEXT_STYLES);
            element.label.Content = ps.Id;
            element.Resources.Add("TextStyleRef", ps);
            //element.MouseMove -= TreeViewItem_MouseMove;
            //element.MouseDoubleClick += TvPageElement_MouseDoubleClick;
            element.Selected += TvTextStyleElement_Selected;
            return element;
        }

        private void TvTextStyleElement_Selected(object sender, RoutedEventArgs e)
        {
            TextStyle pg = (sender as LibraryTreeItem).Resources["TextStyleRef"] as TextStyle;
            SetActiveProperties(pg.GetPropertyLayout());
            e.Handled = true;
        }

        private void AddNewTextStyle()
        {
            TextStyle ps = new TextStyle(_text_id);
            ps.PropertyChanged += Styles_PropertyChanged;
            _text_id++;

            if (ps is TextStyle p)
            {
                var item = GenerateTvTextStyleElement(p);
                TreeViewTS.Items.Add(item);
                TreeViewTS.IsExpanded = true;
                item.IsSelected = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add ParagraphStyle to Layout Tree: Unhandled ParagraphStyle type.");
            }
            TextStyles.Add(ps);
            cmbTextStyle.Items.Add(ps);
        }

        public void DeleteTextStyle(LibraryTreeItem item)
        {
            TextStyle pg = item.Resources["TextStyleRef"] as TextStyle;
            if (pg != null)
                TextStyles.Remove(pg);
            if (item != null)
            {
                TreeViewTS.Items.Remove(item);
                PropertiesPanel.Children.Clear();
                TreeViewTS.IsSelected = true;
            }
        }

        private void CtxMenuTextStyle_Click(object sender, RoutedEventArgs e)
        {
            AddNewTextStyle();
        }

        private ParagraphStyles _ParagraphStyles = new ParagraphStyles();
        public ParagraphStyles ParagraphStyles { get => _ParagraphStyles; set => _ParagraphStyles = value; }


        private TvItemElement<ParagraphStyle> GenerateTvParagraphStyleElement(ParagraphStyle ps)
        {
            var element = GenerateTvItemElement<ParagraphStyle>("ParagraphStyle", TvItemCategory.PARAGRAPH_STYLES);
            element.label.Content = ps.Id;
            element.Resources.Add("ParagraphStyleRef", ps);
            //element.MouseMove -= TreeViewItem_MouseMove;
            //element.MouseDoubleClick += TvPageElement_MouseDoubleClick;
            element.Selected += TvParagraphStyleElement_Selected;
            return element;
        }

        private void TvParagraphStyleElement_Selected(object sender, RoutedEventArgs e)
        {
            ParagraphStyle pg = (sender as LibraryTreeItem).Resources["ParagraphStyleRef"] as ParagraphStyle;
            SetActiveProperties(pg.GetPropertyLayout());
            e.Handled = true;
        }

        private void AddNewParagraphStyle()
        {
            ParagraphStyle ps = new ParagraphStyle(_para_id);
            ps.PropertyChanged += Styles_PropertyChanged;
            _para_id++;

            if (ps is ParagraphStyle p)
            {
                var item = GenerateTvParagraphStyleElement(p);
                TreeViewPS.Items.Add(item);
                TreeViewPS.IsExpanded = true;
                item.IsSelected = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add ParagraphStyle to Layout Tree: Unhandled ParagraphStyle type.");
            }
            ParagraphStyles.Add(ps);
            cmbParagraphStyle.Items.Add(ps);
        }

        public void DeleteParagraphStyle(LibraryTreeItem item)
        {
            ParagraphStyle pg = item.Resources["ParagraphStyleRef"] as ParagraphStyle;
            if (pg != null)
                ParagraphStyles.Remove(pg);
            if (item != null)
            {
                TreeViewPS.Items.Remove(item);
                PropertiesPanel.Children.Clear();
                TreeViewPS.IsSelected = true;
            }
        }

        public ImageStyles ImageStyles = new ImageStyles();

        private TvItemElement<ImageStyle> GenerateTvImageStyleElement(ImageStyle ps)
        {
            var element = GenerateTvItemElement<ImageStyle>("Images", TvItemCategory.IMAGES);
            element.label.Content = ps.Id;
            element.Resources.Add("ImageStyleRef", ps);
            //element.MouseMove -= TreeViewItem_MouseMove;
            //element.MouseDoubleClick += TvPageElement_MouseDoubleClick;
            element.Selected += TvImageStyleElement_Selected;
            return element;
        }

        private void TvImageStyleElement_Selected(object sender, RoutedEventArgs e)
        {
            ImageStyle pg = (sender as LibraryTreeItem).Resources["ImageStyleRef"] as ImageStyle;
            SetActiveProperties(pg.GetPropertyLayout());
            e.Handled = true;
        }

        private void AddNewImageStyle()
        {
            ImageStyle ps = new ImageStyle(_image_id);
            ps.PropertyChanged += Styles_PropertyChanged;
            _image_id++;

            if (ps is ImageStyle p)
            {
                var item = GenerateTvImageStyleElement(p);
                TreeViewImages.Items.Add(item);
                TreeViewImages.IsExpanded = true;
                item.IsSelected = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add ParagraphStyle to Layout Tree: Unhandled ParagraphStyle type.");
            }
            ImageStyles.Add(ps);
        }

        public void DeleteImage(LibraryTreeItem item)
        {
            ImageStyle pg = item.Resources["ImageStyleRef"] as ImageStyle;
            if (pg != null)
                ImageStyles.Remove(pg);
            if (item != null)
            {
                TreeViewImages.Items.Remove(item);
                PropertiesPanel.Children.Clear();
                TreeViewImages.IsSelected = true;
            }
        }

        private void CtxMenuParagraphStyle_Click(object sender, RoutedEventArgs e)
        {
            AddNewParagraphStyle();
        }


        private void CtxMenuImages_Click(object sender, RoutedEventArgs e)
        {
            menuInsertImage_Click(null, null);
        }

        public LineStyles LineStyles = new LineStyles();

        private TvItemElement<LineStyle> GenerateTvLineStyleElement(LineStyle ps)
        {
            var element = GenerateTvItemElement<LineStyle>("LineStyles", TvItemCategory.LINE_STYLES);
            element.label.Content = ps.Id;
            element.Resources.Add("LineStyleRef", ps);
            //element.MouseMove -= TreeViewItem_MouseMove;
            //element.MouseDoubleClick += TvPageElement_MouseDoubleClick;
            element.Selected += TvLineStyleElement_Selected;
            return element;
        }

        private void TvLineStyleElement_Selected(object sender, RoutedEventArgs e)
        {
            LineStyle pg = (sender as LibraryTreeItem).Resources["LineStyleRef"] as LineStyle;

            SetActiveProperties(pg.GetPropertyLayout());
            e.Handled = true;
        }

        private void AddNewLineStyle()
        {
            LineStyle ps = new LineStyle(_line_id);
            ps.PropertyChanged += Styles_PropertyChanged;
            _line_id++;

            if (ps is LineStyle p)
            {
                var item = GenerateTvLineStyleElement(p);
                TreeViewLS.Items.Add(item);
                TreeViewLS.IsExpanded = true;
                item.IsSelected = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add ParagraphStyle to Layout Tree: Unhandled ParagraphStyle type.");
            }
            LineStyles.Add(ps);
        }

        public void DeleteLineStyle(LibraryTreeItem item)
        {
            LineStyle pg = item.Resources["LineStyleRef"] as LineStyle;
            if (pg != null)
                LineStyles.Remove(pg);
            if (item != null)
            {
                TreeViewLS.Items.Remove(item);
                PropertiesPanel.Children.Clear();
                TreeViewLS.IsSelected = true;
            }
        }

        private void CtxMenuLineStyle_Click(object sender, RoutedEventArgs e)
        {
            AddNewLineStyle();
        }

        private void CtxMenuColors_Click(object sender, RoutedEventArgs e)
        {
            AddNewColorStyle();
        }

        private void CtxMenuFillStyle_Click(object sender, RoutedEventArgs e)
        {

        }

        public BorderStyles BorderStyles = new BorderStyles();

        private TvItemElement<BorderStyle> GenerateTvBorderStyleElement(BorderStyle ps)
        {
            var element = GenerateTvItemElement<BorderStyle>("BorderStyle", TvItemCategory.BORDER_STYLES);
            element.label.Content = ps.Id;
            element.Resources.Add("BorderStyleRef", ps);
            //element.MouseMove -= TreeViewItem_MouseMove;
            //element.MouseDoubleClick += TvPageElement_MouseDoubleClick;
            element.Selected += TvBorderStyleElement_Selected;
            return element;
        }

        private void TvBorderStyleElement_Selected(object sender, RoutedEventArgs e)
        {
            BorderStyle pg = (sender as LibraryTreeItem).Resources["BorderStyleRef"] as BorderStyle;
            SetActiveProperties(pg.GetPropertyLayout());
            e.Handled = true;
        }

        private void AddNewBorderStyle()
        {
            BorderStyle ps = new BorderStyle(_border_id);
            ps.PropertyChanged += Styles_PropertyChanged;
            _border_id++;

            if (ps is BorderStyle p)
            {
                var item = GenerateTvBorderStyleElement(p);
                TreeViewBS.Items.Add(item);
                TreeViewBS.IsExpanded = true;
                item.IsSelected = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add ParagraphStyle to Layout Tree: Unhandled ParagraphStyle type.");
            }
            BorderStyles.Add(ps);
        }

        public void DeleteBorderStyle(LibraryTreeItem item)
        {
            BorderStyle pg = item.Resources["BorderStyleRef"] as BorderStyle;
            if (pg != null)
                BorderStyles.Remove(pg);
            if (item != null)
            {
                TreeViewBS.Items.Remove(item);
                PropertiesPanel.Children.Clear();
                TreeViewBS.IsSelected = true;
            }
        }

        private void CtxMenuBorderStyle_Click(object sender, RoutedEventArgs e)
        {
            AddNewBorderStyle();
        }

        public TableStyles TableStyles = new TableStyles();

        private TvItemElement<TableStyle> GenerateTvTableStyleElement(TableStyle ps)
        {
            var element = GenerateTvItemElement<TableStyle>("Tables", TvItemCategory.TABLES);
            element.label.Content = ps.Id;
            element.Resources.Add("TableStyleRef", ps);
            //element.MouseMove -= TreeViewItem_MouseMove;
            //element.MouseDoubleClick += TvPageElement_MouseDoubleClick;
            element.Selected += TvTableStyleElement_Selected;
            return element;
        }

        private void TvTableStyleElement_Selected(object sender, RoutedEventArgs e)
        {
            TableStyle pg = (sender as LibraryTreeItem).Resources["TableStyleRef"] as TableStyle;
            SetActiveProperties(pg.GetPropertyLayout());
            e.Handled = true;
        }

        private void AddNewTableStyle()
        {
            TableStyle ps = new TableStyle(_table_id);
            ps.PropertyChanged += Styles_PropertyChanged;
            _table_id++;

            if (ps is TableStyle p)
            {
                var item = GenerateTvTableStyleElement(p);
                TreeViewTables.Items.Add(item);
                TreeViewTables.IsExpanded = true;
                item.IsSelected = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add ParagraphStyle to Layout Tree: Unhandled ParagraphStyle type.");
            }
            TableStyles.Add(ps);
        }

        public void DeleteTable(LibraryTreeItem item)
        {
            TableStyle pg = item.Resources["TableStyleRef"] as TableStyle;
            if (pg != null)
                TableStyles.Remove(pg);
            if (item != null)
            {
                TreeViewTables.Items.Remove(item);
                PropertiesPanel.Children.Clear();
                TreeViewTables.IsSelected = true;
            }
        }

        private void CtxMenuTable_Click(object sender, RoutedEventArgs e)
        {
            AddNewTableStyle();
        }

        public FontStyles FontStyles = new FontStyles();

        private TvItemElement<FontStyle> GenerateTvFontStyleElement(FontStyle ps)
        {
            var element = GenerateTvItemElement<FontStyle>("Fonts", TvItemCategory.FONTS);
            element.label.Content = ps.Id;
            element.Resources.Add("FontStyleRef", ps);
            //element.MouseMove -= TreeViewItem_MouseMove;
            //element.MouseDoubleClick += TvPageElement_MouseDoubleClick;
            element.Selected += TvFontStyleElement_Selected;
            return element;
        }

        private void TvFontStyleElement_Selected(object sender, RoutedEventArgs e)
        {
            FontStyle pg = (sender as LibraryTreeItem).Resources["FontStyleRef"] as FontStyle;
            SetActiveProperties(pg.GetPropertyLayout());
            e.Handled = true;
        }

        private void AddNewFontStyle()
        {
            FontStyle ps = new FontStyle(_font_id);
            ps.PropertyChanged += Styles_PropertyChanged;
            _font_id++;

            if (ps is FontStyle p)
            {
                var item = GenerateTvFontStyleElement(p);
                TreeViewFonts.Items.Add(item);
                TreeViewFonts.IsExpanded = true;
                item.IsSelected = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add ParagraphStyle to Layout Tree: Unhandled ParagraphStyle type.");
            }
            FontStyles.Add(ps);

            //ps.SetFont(cmbFontName.SelectedItem as string);
            SetActiveProperties(ps.GetPropertyLayout());
        }

        public void DeleteFont(LibraryTreeItem item)
        {
            FontStyle pg = item.Resources["FontStyleRef"] as FontStyle;
            if (pg != null)
                FontStyles.Remove(pg);
            if (item != null)
            {
                TreeViewFonts.Items.Remove(item);
                PropertiesPanel.Children.Clear();
                TreeViewFonts.IsSelected = true;
            }
        }

        private void CtxMenuFont_Click(object sender, RoutedEventArgs e)
        {
            menuInsertFont_Click(null, null);
        }

        private void CtxMenuPSDel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CtxMenuPSRen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void XtxMenuTSDel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CtxMenuTSRen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CtxMenuFontDel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CtxMenuFontRen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CtxMenuColorsDel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CtxMenuColorsRen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CtxMenuImagesDel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CtxMenuImagesRen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CtxMenuTableDel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CtxMenuTableRen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CtxMenuBSDel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CtxMenuBSRen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CtxMenuLSDel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CtxMenuLSRen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CtxMenuFSDel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CtxMenuFSRen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TreeViewPages_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {

        }

        private void TreeViewPages_Expanded(object sender, RoutedEventArgs e)
        {

        }

        private void TreeViewPages_Collapsed(object sender, RoutedEventArgs e)
        {

        }

        private void TreeViewPages_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void TreeViewPS_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {

        }

        private void TreeViewPS_Expanded(object sender, RoutedEventArgs e)
        {

        }

        private void TreeViewPS_Collapsed(object sender, RoutedEventArgs e)
        {

        }

        private void TreeViewPS_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void CtxMenuTSDel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CtxMenuColor_Click(object sender, RoutedEventArgs e)
        {

        }

        public Dictionary<string, TypographicFontFamily> FontList { get; set; }
        public Dictionary<string, string> FontPath { get; set; }
        public void InitFont()
        {
            FontList = new Dictionary<string, TypographicFontFamily>();
            FontPath = new Dictionary<string, string>();
            var fonts = TypographicFontFamily.InstalledFamilies;
            foreach (var font in fonts)
            {
                if (font.Name.Equals("Arial"))
                {
                    FontList.Add(font.Name, font);
                    FontPath.Add(font.Name, (font.Fonts[0].FileName));
                    break;
                }
            }


            RefreshFontComboBox();

            cmbFontSize.Items.Add("8");
            cmbFontSize.Items.Add("9");
            cmbFontSize.Items.Add("10");
            cmbFontSize.Items.Add("11");
            cmbFontSize.Items.Add("12");
            cmbFontSize.Items.Add("14");
            cmbFontSize.Items.Add("16");
            cmbFontSize.Items.Add("18");
            cmbFontSize.Items.Add("20");
            cmbFontSize.Items.Add("22");
            cmbFontSize.Items.Add("24");
            cmbFontSize.Items.Add("26");
            cmbFontSize.Items.Add("28");
            cmbFontSize.Items.Add("30");
            cmbFontSize.Items.Add("34");
            cmbFontSize.Items.Add("36");
            cmbFontSize.Items.Add("48");
            cmbFontSize.Items.Add("72");
        }

        public void RefreshFontComboBox()
        {
            cmbFontName.Items.Clear();
            var keys = FontList.Keys;
            foreach (var key in keys)
            {
                cmbFontName.Items.Add(key);
            }

            cmbFontName.SelectedIndex = cmbFontName.Items.Count - 1;


        }

        public TreeViewItem GetSelectedTreeItem()
        {
            return TreeViewRoot.SelectedItem as TreeViewItem;
        }

        private void cmbFontName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontName.SelectedItem != null)
            {
                string fontName = cmbFontName.SelectedItem as string;
                TypographicFontFamily list;
                if (FontList.TryGetValue(fontName, out list))
                {
                    cmbFontCateg.Items.Clear();
                    foreach (var subFamily in list.Fonts)
                    {
                        cmbFontCateg.Items.Add(subFamily.SubFamily);
                    }
                    cmbFontCateg.SelectedIndex = 0;
                }

                FlowEx flow = ActivePage as FlowEx;
                if (flow == null) return;

                try
                {
                    if (!this.cmbFontName.IsDropDownOpen) return;
                    string fontName2 = cmbFontName.SelectedValue as string;
                    string path = FontPath[fontName2];
                    System.Drawing.FontFamily[] familys = UseCustomFont(path);
                    string fontSize2 = cmbFontSize.SelectedValue as string;
                    flow.Flow.TextEditor.SelectionFont2 = new Font(familys[0], Convert.ToInt32(fontSize2));
                }
                catch (Exception ex)
                {
                }
            }
        }

        private System.Drawing.FontFamily[] UseCustomFont(string name)
        {

            PrivateFontCollection modernFont = new PrivateFontCollection();

            modernFont.AddFontFile(name);
            return modernFont.Families;
        }

        public void LoadTreeItems(XmlReader reader)
        {
            reader.ReadStartElement();
            while (reader.NodeType == XmlNodeType.Element)
            {

                var count = int.Parse(reader.GetAttribute("Count"));
                var id_counter = 1;
                try
                {
                    string strtmp = reader.GetAttribute("Counter");
                    id_counter = int.Parse(strtmp);
                }
                catch (Exception e)
                {
                    id_counter = 1;
                }

                if (reader.Name == "Fonts")
                    _font_id = id_counter;
                else if (reader.Name == "Images")
                    _image_id = id_counter;
                else if (reader.Name == "Colors")
                    _color_id = id_counter;
                else if (reader.Name == "Paragraphs")
                    _para_id = id_counter;
                else if (reader.Name == "Lines")
                    _line_id = id_counter;
                else if (reader.Name == "Texts")
                    _text_id = id_counter;
                else if (reader.Name == "Borders")
                    _border_id = id_counter;
                else if (reader.Name == "Containers")
                    _container_id = id_counter;
                else if (reader.Name == "Tables")
                    _table_id = id_counter;
                else if (reader.Name == "Blocks")
                    _table_id = id_counter;

                if (count <= 0)
                {
                    reader.Skip(); // skip Document
                }
                else
                {
                    reader.ReadStartElement();
                    for (int i = 0; i < count; i++)
                    {
                        var type = Type.GetType(reader.Name);
                        if (type.Name == "ImageStyle")
                        {
                            var inst = (ImageStyle)Activator.CreateInstance(type);
                            inst.PropertyChanged += Styles_PropertyChanged;
                            inst.ReadXml(reader);
                            AddNewImage(inst);
                        }
                        else if (type.Name == "FontStyle")
                        {
                            var inst = (FontStyle)Activator.CreateInstance(type);
                            inst.PropertyChanged += Styles_PropertyChanged;
                            inst.ReadXml(reader);
                            AddNewFont(inst);
                            AddFontToList(inst.FileName);
                        }
                        else if (type.Name == "ColorStyle")
                        {
                            var inst = (ColorStyle)Activator.CreateInstance(type);
                            inst.PropertyChanged += Styles_PropertyChanged;
                            inst.ReadXml(reader);
                            AddNewColorStyle(inst);
                        }
                        else if (type.Name == "LineStyle")
                        {
                            var inst = (LineStyle)Activator.CreateInstance(type);
                            inst.PropertyChanged += Styles_PropertyChanged;
                            inst.ReadXml(reader);
                            AddNewLineStyle(inst);
                        }
                        else if (type.Name == "BorderStyle")
                        {
                            var inst = (BorderStyle)Activator.CreateInstance(type);
                            inst.PropertyChanged += Styles_PropertyChanged;
                            inst.ReadXml(reader);
                            AddNewBorderStyle(inst);
                        }
                        else if (type.Name == "ParagraphStyle")
                        {
                            var inst = (ParagraphStyle)Activator.CreateInstance(type);
                            inst.PropertyChanged += Styles_PropertyChanged;
                            inst.ReadXml(reader);
                            AddNewParagraphStyle(inst);
                        }
                        else if (type.Name == "TextStyle")
                        {
                            var inst = (TextStyle)Activator.CreateInstance(type);
                            inst.PropertyChanged += Styles_PropertyChanged;
                            inst.ReadXml(reader);
                            AddNewTextStyle(inst);
                        }
                        else if (type.Name == "BlockStyle")
                        {
                            var inst = (BlockStyle)Activator.CreateInstance(type);
                            inst.PropertyChanged += Styles_PropertyChanged;
                            inst.ReadXml(reader);
                            AddNewBlock(inst, inst.PageId);
                        }
                        //Add(instance);
                        reader.Skip();
                    }

                    reader.ReadEndElement(); // end of Document
                }
            }
            //LayoutProperties.ReadXml(reader);*/
            if (reader.IsEmptyElement)
            {
                reader.Skip();
            }
            else
            {
                reader.ReadEndElement();
            }
        }

        private void AddNewTextStyle(TextStyle inst)
        {
            if (inst is TextStyle p)
            {
                var item = GenerateTvTextStyleElement(p);
                TreeViewTS.Items.Add(item);
                TreeViewTS.IsExpanded = true;
                item.IsSelected = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add Text to Layout Tree: Unhandled Text type.");
            }
            TextStyles.Add(inst);
            cmbTextStyle.Items.Add(inst);
        }

        private void AddNewParagraphStyle(ParagraphStyle inst)
        {
            if (inst is ParagraphStyle p)
            {
                var item = GenerateTvParagraphStyleElement(p);
                TreeViewPS.Items.Add(item);
                TreeViewPS.IsExpanded = true;
                item.IsSelected = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add Paragraph to Layout Tree: Unhandled Paragraph type.");
            }
            ParagraphStyles.Add(inst);
            cmbParagraphStyle.Items.Add(inst);
        }

        private void AddNewBorderStyle(BorderStyle inst)
        {
            if (inst is BorderStyle p)
            {
                var item = GenerateTvBorderStyleElement(p);
                TreeViewBS.Items.Add(item);
                TreeViewBS.IsExpanded = true;
                item.IsSelected = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add Border to Layout Tree: Unhandled Border type.");
            }
            BorderStyles.Add(inst);
        }

        private void AddNewLineStyle(LineStyle inst)
        {
            if (inst is LineStyle p)
            {
                var item = GenerateTvLineStyleElement(p);
                TreeViewLS.Items.Add(item);
                TreeViewLS.IsExpanded = true;
                item.IsSelected = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add Line to Layout Tree: Unhandled Line type.");
            }
            LineStyles.Add(inst);
        }

        public class ColorInfo
        {
            public System.Windows.Media.Color Color { get; set; }
            public string Name { get; set; }
            public bool IsUser { get; set; }
        }

        private void InitColorComboBox()
        {

            cmbColorName.Items.Clear();
            PropertyInfo[] infos = typeof(Colors).GetProperties();
            foreach (PropertyInfo info in infos)
            {
                System.Windows.Media.Color clr = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(info.Name);
                cmbColorName.Items.Add(
                    new ColorInfo
                    {
                        Color = clr,
                        Name = info.Name,
                        IsUser = false

                    });
            }
        }

        private void AddNewColorStyle(ColorStyle inst)
        {
            if (inst is ColorStyle p)
            {
                var item = GenerateTvColorStyleElement(p);
                TreeViewColors.Items.Add(item);
                TreeViewColors.IsExpanded = true;
                item.IsSelected = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add Color to Layout Tree: Unhandled Image type.");
            }
            ColorStyles.Add(inst);
            cmbColorName.Items.Insert(0,
                new ColorInfo
                {
                    Color = inst.Color,
                    Name = inst.Id,
                    IsUser = true
                });
        }

        private void AddNewFont(FontStyle inst)
        {
            if (inst is FontStyle p)
            {
                var item = GenerateTvFontStyleElement(p);
                TreeViewFonts.Items.Add(item);
                TreeViewFonts.IsExpanded = true;
                item.IsSelected = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add Font to Layout Tree: Unhandled Image type.");
            }
            FontStyles.Add(inst);
        }

        private void AddFontToList(string filename)
        {
            TypographicFont[] fonts = TypographicFont.FromFile(filename);
            foreach (var font in fonts)
            {
                TypographicFontFamily list;
                if (!FontList.TryGetValue(font.Family, out list))
                {
                    FontList.Add(font.Family, list = new TypographicFontFamily(font.Family, fonts.ToList()));
                    FontPath.Add(font.Family, font.FileName);
                }
            }
            RefreshFontComboBox();
        }

        private void menuInsertFont_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFiledialog = new System.Windows.Forms.OpenFileDialog();
            openFiledialog.Filter = "True Type Fonts|*.ttf;*.ttc;*.otf|Windows Raster Fonts|*.fon|Type 1Fonts|*.pfb;*.pfm|All Files|*.*";
            openFiledialog.Title = "Select a Font File";

            if (openFiledialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filename = openFiledialog.FileName;
                AddFontToList(filename);

                AddNewFontStyle();
                FontStyle fs = FontStyles[FontStyles.Count - 1];
                fs.SetFont(filename);

                RefreshFontComboBox();
                FontStyles.RefreshAll();

            }
        }

        private void AddNewImage(ImageStyle image)
        {
            if (image is ImageStyle p)
            {
                var item = GenerateTvImageStyleElement(p);
                TreeViewImages.Items.Add(item);
                TreeViewImages.IsExpanded = true;
                item.IsSelected = true;
            }
            else
            {
                Logger.Log(Severity.WARN, LogCategory.APP, "Unable to add Image to Layout Tree: Unhandled Image type.");
            }
            ImageStyles.Add(image);
        }

        private void menuInsertImage_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.bmp;*.gif;*.jpg;*.jpeg;*.jif;*.img;*.png;*.tif;*.wmf;*.emf|PDF File|*.pdf|All Files|*.*";
            openFileDialog.Title = "Select an Image File";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                AddNewImageStyle();

                ImageStyle imgStyle = ImageStyles[ImageStyles.Count - 1];
                imgStyle.SetImage(fileName);
            }
        }

        private void chkNormalArrow_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void chkNormalArrow_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void chkBlockArrow_Checked(object sender, RoutedEventArgs e)
        {
            if (TreeViewRoot == null) return;
            LibraryTreeItem treeItem = TreeViewRoot.SelectedItem as LibraryTreeItem;
            if (treeItem == null) return;

            if (treeItem.ItemCategory == TvItemCategory.BLOCK_STYLES)
            {
                BlockStyle bs = treeItem.Resources["BlockStyleRef"] as BlockStyle;
                bs.ShowControlBorder(true);
                GetActiveProperties().IsEnabled = true;
            }
        }

        private void chkBlockArrow_Unchecked(object sender, RoutedEventArgs e)
        {
            if (TreeViewRoot == null) return;
            LibraryTreeItem treeItem = TreeViewRoot.SelectedItem as LibraryTreeItem;
            if (treeItem == null) return;

            if (treeItem.ItemCategory == TvItemCategory.BLOCK_STYLES)
            {
                BlockStyle bs = treeItem.Resources["BlockStyleRef"] as BlockStyle;
                bs.ShowControlBorder(false);
                GetActiveProperties().IsEnabled = false;
            }
        }

        private void btnBold_Click(object sender, RoutedEventArgs e)
        {
            FlowEx flow = ActivePage as FlowEx;
            if (flow == null) return;

            if (flow.Flow.TextEditor.SelectionCharStyle.Bold == true)
            {
                this.btnBold.IsChecked = false;
                ExtendedRichTextBox.CharStyle cs = flow.Flow.TextEditor.SelectionCharStyle;
                cs.Bold = false;
                flow.Flow.TextEditor.SelectionCharStyle = cs;
                cs = null;
            }
            else
            {
                this.btnBold.IsChecked = true;
                ExtendedRichTextBox.CharStyle cs = flow.Flow.TextEditor.SelectionCharStyle;
                cs.Bold = true;
                flow.Flow.TextEditor.SelectionCharStyle = cs;
                cs = null;
            }
        }

        private void btnAlignLeft_Click(object sender, RoutedEventArgs e)
        {
            FlowEx bs = ActivePage as FlowEx;
            if (bs == null) return;

            bs.Flow.TextEditor.SelectionAlignment = ExtendedRichTextBox.RichTextAlign.Left;
            this.btnAlignLeft.IsChecked = true;
            this.btnAlignRight.IsChecked = false;
            this.btnAlignCenter.IsChecked = false;
            this.btnJustify.IsChecked = false;
        }

        private void btnAlignCenter_Click(object sender, RoutedEventArgs e)
        {
            FlowEx bs = ActivePage as FlowEx;
            if (bs == null) return;

            bs.Flow.TextEditor.SelectionAlignment = ExtendedRichTextBox.RichTextAlign.Center;
            this.btnAlignLeft.IsChecked = false;
            this.btnAlignRight.IsChecked = false;
            this.btnAlignCenter.IsChecked = true;
            this.btnJustify.IsChecked = false;
        }

        private void btnAlignRight_Click(object sender, RoutedEventArgs e)
        {
            FlowEx bs = ActivePage as FlowEx;
            if (bs == null) return;

            bs.Flow.TextEditor.SelectionAlignment = ExtendedRichTextBox.RichTextAlign.Right;
            this.btnAlignLeft.IsChecked = false;
            this.btnAlignRight.IsChecked = true;
            this.btnAlignCenter.IsChecked = false;
            this.btnJustify.IsChecked = false;
        }

        private void cmbFontName_KeyUp(object sender, KeyEventArgs e)
        {
            FlowEx bs = ActivePage as FlowEx;
            if (bs == null) return;

            try
            {
                if (e.Key == Key.Enter)
                {
                    bs.Flow.TextEditor.SelectionFont2 = new Font(this.cmbFontName.Text, Convert.ToInt32(this.cmbFontSize.Text));
                    bs.Flow.TextEditor.Focus();
                }
            }
            catch (Exception)
            {
            }
        }

        private void cmbFontSize_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void btnItalic_Click(object sender, RoutedEventArgs e)
        {
            FlowEx bs = ActivePage as FlowEx;
            if (bs == null) return;

            try
            {
                if (bs.Flow.TextEditor.SelectionCharStyle.Italic == true)
                {
                    this.btnItalic.IsChecked = false;
                    ExtendedRichTextBox.CharStyle cs = bs.Flow.TextEditor.SelectionCharStyle;
                    cs.Italic = false;
                    bs.Flow.TextEditor.SelectionCharStyle = cs;
                    cs = null;
                }
                else
                {
                    this.btnItalic.IsChecked = true;
                    ExtendedRichTextBox.CharStyle cs = bs.Flow.TextEditor.SelectionCharStyle;
                    cs.Italic = true;
                    bs.Flow.TextEditor.SelectionCharStyle = cs;
                    cs = null;
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnUnderline_Click(object sender, RoutedEventArgs e)
        {
            FlowEx bs = ActivePage as FlowEx;
            if (bs == null) return;

            try
            {
                if (bs.Flow.TextEditor.SelectionCharStyle.Underline == true)
                {
                    this.btnUnderline.IsChecked = false;
                    ExtendedRichTextBox.CharStyle cs = bs.Flow.TextEditor.SelectionCharStyle;
                    cs.Underline = false;
                    bs.Flow.TextEditor.SelectionCharStyle = cs;
                    cs = null;
                }
                else
                {
                    this.btnUnderline.IsChecked = true;
                    ExtendedRichTextBox.CharStyle cs = bs.Flow.TextEditor.SelectionCharStyle;
                    cs.Underline = true;
                    bs.Flow.TextEditor.SelectionCharStyle = cs;
                    cs = null;
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnStrikeThrough_Click(object sender, RoutedEventArgs e)
        {
            FlowEx bs = ActivePage as FlowEx;
            if (bs == null) return;

            try
            {
                if (bs.Flow.TextEditor.SelectionCharStyle.Strikeout == true)
                {
                    this.btnStrikeThrough.IsChecked = false;
                    ExtendedRichTextBox.CharStyle cs = bs.Flow.TextEditor.SelectionCharStyle;
                    cs.Strikeout = false;
                    bs.Flow.TextEditor.SelectionCharStyle = cs;
                    cs = null;
                }
                else
                {
                    this.btnStrikeThrough.IsChecked = true;
                    ExtendedRichTextBox.CharStyle cs = bs.Flow.TextEditor.SelectionCharStyle;
                    cs.Strikeout = true;
                    bs.Flow.TextEditor.SelectionCharStyle = cs;
                    cs = null;
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnJustify_Click(object sender, RoutedEventArgs e)
        {
            FlowEx bs = ActivePage as FlowEx;
            if (bs == null) return;

            bs.Flow.TextEditor.SelectionAlignment = ExtendedRichTextBox.RichTextAlign.Justify;
            this.btnAlignLeft.IsChecked = false;
            this.btnAlignRight.IsChecked = false;
            this.btnAlignCenter.IsChecked = false;
            this.btnJustify.IsChecked = true;
        }

        private void btnNumberedList_Click(object sender, RoutedEventArgs e)
        {
            FlowEx bs = ActivePage as FlowEx;
            if (bs == null) return;
            try
            {
                if (this.btnNumberedList.IsChecked == false)
                {
                    this.btnBulletedList.IsChecked = false;
                    this.btnNumberedList.IsChecked = false;
                    ExtendedRichTextBox.ParaListStyle pls = new ExtendedRichTextBox.ParaListStyle();

                    pls.Type = ExtendedRichTextBox.ParaListStyle.ListType.None;
                    pls.Style = ExtendedRichTextBox.ParaListStyle.ListStyle.NumberAndParenthesis;

                    bs.Flow.TextEditor.SelectionListType = pls;
                }
                else
                {
                    this.btnBulletedList.IsChecked = false;
                    this.btnNumberedList.IsChecked = true;
                    ExtendedRichTextBox.ParaListStyle pls = new ExtendedRichTextBox.ParaListStyle();

                    pls.Type = ExtendedRichTextBox.ParaListStyle.ListType.Numbers;
                    pls.Style = ExtendedRichTextBox.ParaListStyle.ListStyle.NumberInPar;

                    bs.Flow.TextEditor.SelectionListType = pls;
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnBulletedList_Click(object sender, RoutedEventArgs e)
        {
            FlowEx bs = ActivePage as FlowEx;
            if (bs == null) return;

            try
            {
                if (this.btnBulletedList.IsChecked == false)
                {
                    this.btnBulletedList.IsChecked = false;
                    this.btnNumberedList.IsChecked = false;
                    ExtendedRichTextBox.ParaListStyle pls = new ExtendedRichTextBox.ParaListStyle();

                    pls.Type = ExtendedRichTextBox.ParaListStyle.ListType.None;
                    pls.Style = ExtendedRichTextBox.ParaListStyle.ListStyle.NumberAndParenthesis;

                    bs.Flow.TextEditor.SelectionListType = pls;
                }
                else
                {
                    this.btnBulletedList.IsChecked = true;
                    this.btnNumberedList.IsChecked = false;
                    ExtendedRichTextBox.ParaListStyle pls = new ExtendedRichTextBox.ParaListStyle();

                    pls.Type = ExtendedRichTextBox.ParaListStyle.ListType.Bullet;
                    pls.Style = ExtendedRichTextBox.ParaListStyle.ListStyle.NumberAndParenthesis;

                    bs.Flow.TextEditor.SelectionListType = pls;
                }
            }
            catch (Exception)
            {
            }
        }

        private void cmbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FlowEx bs = ActivePage as FlowEx;
            if (bs == null) return;

            try
            {
                if (!this.cmbFontSize.IsDropDownOpen) return;
                string fontName2 = cmbFontName.SelectedValue as string;
                string path = FontPath[fontName2];
                System.Drawing.FontFamily[] familys = UseCustomFont(path);
                string fontSize2 = cmbFontSize.SelectedValue as string;
                bs.Flow.TextEditor.SelectionFont2 = new Font(familys[0], Convert.ToInt32(fontSize2));

            }
            catch (Exception)
            {

            }
        }

        private void cmbFontSize_KeyUp(object sender, KeyEventArgs e)
        {
            FlowEx bs = ActivePage as FlowEx;
            if (bs == null) return;
        }

        private void cmbColorName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FlowEx bs = ActivePage as FlowEx;
            if (bs == null) return;
            try
            {
                System.Windows.Media.Color selColor = (cmbColorName.SelectedItem as ColorInfo).Color;
                bs.Flow.TextEditor.SelectionColor2 = System.Drawing.Color.FromArgb(selColor.A, selColor.R, selColor.G, selColor.B);
            }
            catch (Exception)
            {
            }
        }

        public static int ConvertPixelsToTwips(int value)
        {
            int RetVal;

            const int TwipsPerInch = 1440;

            RetVal = (int)((value / ImageHelper.DPI) * TwipsPerInch);

            return RetVal;
        }

        private void cmbParagraphStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ParagraphStyle para = cmbParagraphStyle.SelectedItem as ParagraphStyle;
            if (para == null) return;

            FlowEx bs = ActivePage as FlowEx;
            if (bs == null) return;

            para.LayoutProperties.GetProperty();
            bs.Flow.TextEditor.SelectionAlignment = (ExtendedRichTextBox.RichTextAlign)(para.LayoutProperties.Alignment + 1);

            try
            {
                bs.Flow.TextEditor.SelectionIndent = (int)(para.LayoutProperties.FirstLineLeftIndent * bs.Flow.Ruler.DotsPerMillimeter);
                bs.Flow.TextEditor.SelectionHangingIndent = (int)(para.LayoutProperties.LeftIndent * bs.Flow.Ruler.DotsPerMillimeter) - (int)(para.LayoutProperties.FirstLineLeftIndent * bs.Flow.Ruler.DotsPerMillimeter);
                bs.Flow.TextEditor.SelectionRightIndent = (int)(para.LayoutProperties.RightIndent * bs.Flow.Ruler.DotsPerMillimeter);

                if (para.LayoutProperties.SpaceBeforeOnFirst == 1)
                {
                    bs.Flow.TextEditor.SelectionSpaceBefore = ConvertPixelsToTwips((int)(para.LayoutProperties.SpaceBefore * bs.Flow.Ruler.DotsPerMillimeter));
                    bs.Flow.TextEditor.SelectionSpaceAfter = ConvertPixelsToTwips((int)(para.LayoutProperties.SpaceAfter * bs.Flow.Ruler.DotsPerMillimeter));
                }
                else
                {
                    bs.Flow.TextEditor.SelectionSpaceBefore = 0;
                    bs.Flow.TextEditor.SelectionSpaceAfter = 0;
                }

                ExtendedRichTextBox.ParaLineSpacing para1 = new ExtendedRichTextBox.ParaLineSpacing();
                ExtendedRichTextBox.ParaLineSpacing.LineSpacingStyle spacingStyle = ExtendedRichTextBox.ParaLineSpacing.LineSpacingStyle.Single;
                if (para.LayoutProperties.LineSpacing == 0)
                    spacingStyle = ExtendedRichTextBox.ParaLineSpacing.LineSpacingStyle.Relative;
                else if (para.LayoutProperties.LineSpacing == 2)
                    spacingStyle = ExtendedRichTextBox.ParaLineSpacing.LineSpacingStyle.ExactFree;

                para1.SpacingStyle = spacingStyle;
                para1.ExactSpacing = ConvertPixelsToTwips((int)(para.LayoutProperties.LineSpacingValue * bs.Flow.Ruler.DotsPerMillimeter));
                bs.Flow.TextEditor.SelectionLineSpacing = para1;
            }
            catch (Exception)
            {
            }
        }

        public XDocument Document;
        private void menuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".flo";
            dlg.Filter = "FLO Files (*.flo)|*.flo";

            // Display OpenFileDialog by calling ShowDialog method
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string fileName = dlg.FileName;
                try
                {
                    Document = XDocument.Load(fileName);
                    var element = Document?.Root.Elements("Designer")?.FirstOrDefault();
                    if (element != null)
                    {
                        using (var reader = element.CreateReader())
                        {
                            ReadXml(reader);
                            return;
                        }
                    }
                }
                catch (Exception err)
                {
                    Logger.Log(Severity.ERROR, LogCategory.APP, "Could not load workflow: " + err.Message);
                }
            }


        }

        public void ReadXml(XmlReader reader)
        {
            TreeViewBS.Items.Clear();
            TreeViewColors.Items.Clear();
            TreeViewData.Items.Clear();
            TreeViewElements.Items.Clear();
            TreeViewFillStyle.Items.Clear();
            TreeViewFlows.Items.Clear();
            TreeViewFonts.Items.Clear();
            TreeViewImages.Items.Clear();
            TreeViewLS.Items.Clear();
            TreeViewPages.Items.Clear();
            TreeViewPS.Items.Clear();
            TreeViewSystemVars.Items.Clear();
            TreeViewTS.Items.Clear();
            TreeViewTables.Items.Clear();

            Pages.Clear();
            BorderStyles.Clear();
            ColorStyles.Clear();
            Containers.Clear();
            FlowStyles.Clear();
            FontStyles.Clear();
            ImageStyles.Clear();
            LineStyles.Clear();
            ParagraphStyles.Clear();
            TextStyles.Clear();
            TableStyles.Clear();



            reader.ReadStartElement();
            var serializer = new XmlSerializer(typeof(Pages));
            Load((Pages)serializer.Deserialize(reader));
            if (reader.NodeType != XmlNodeType.EndElement)
                LoadTreeItems(reader);

            reader.ReadEndElement();

        }

        private void menuFileClose_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuFileSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuFileImport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuFileExport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuFileOptions_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuFileSaveAs_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    /*
    public abstract class TvItemElement : TreeViewItem
	{
		private TvItemCategory _ItemCategory;
		private Type _ElementType;

		public Type ElementType => _ElementType;
		public TvItemCategory ItemCategory => _ItemCategory; 

		protected TvItemElement(Type type, TvItemCategory category)
		{
			_ElementType = type;
			_ItemCategory = category;
		}
	}
    */

    public class TvItemElement<T> : LibraryTreeItem where T : ILayoutNode
    {
        public TvItemElement(TvItemCategory category) : base(typeof(T), category) { }
    }
}