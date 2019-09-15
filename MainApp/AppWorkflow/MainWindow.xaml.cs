using AppWorkflow.Controls;
using JdSuite.Common;
using JdSuite.Common.Logging;
using JdSuite.Common.Logging.Enums;
using JdSuite.Common.Module;
using JdSuite.Common.Module.MefMetadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace AppWorkflow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string APP_NAME = "AppWorkflow";
        private Color ColorBackground = (Color)ColorConverter.ConvertFromString("#FFEEF5FD");

        NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(MainWindow));

        private Dictionary<string, IModule> ModuleMap = new Dictionary<string, IModule>();

        private WorkflowCanvas WFCanvas;

        public Color GetColor(String configName)
        {
            return (Color)ColorConverter.ConvertFromString(ConfigurationManager.ConnectionStrings[configName].ConnectionString);
        }

        public MainWindow(string filePath = null)
        {
            InitializeComponent();



            this.Closed += delegate
            {
                Logger.OnExit();
            };

            //this.KeyDown += MainWindow_KeyDown;


            WFCanvas = (WorkflowCanvas)FindName("WorkflowCanvas");
            // Load module plugins
            this.Loaded += delegate
            {
                foreach (var moduleMEF in ((App)Application.Current)._modules)
                {
                    BaseModule module = (BaseModule)moduleMEF.CreateExport().Value;
                    logger.Info("Loading module {0}", moduleMEF.Metadata["ModuleCategory"]);

                    switch (moduleMEF.Metadata["ModuleCategory"])
                    {
                        case IModuleCategory.DATA_INPUTS:
                            this.AppendSidebarModule("ExpanderDataInputs", module, ColorBackground);
                            break;

                        case IModuleCategory.DATA_MANIPULATION:
                            this.AppendSidebarModule("ExpanderDataManipulation", module, ColorBackground);
                            break;

                        case IModuleCategory.DESIGNS:
                            this.AppendSidebarModule("ExpanderDesigns", module, ColorBackground);
                            break;

                        case IModuleCategory.IMPOSITION:
                            this.AppendSidebarModule("ExpanderImposition", module, ColorBackground);
                            break;

                        case IModuleCategory.OUTPUTS:
                            this.AppendSidebarModule("ExpanderOutputs", module, ColorBackground);
                            break;

                        case IModuleCategory.MISC:
                            this.AppendSidebarModule("ExpanderMisc", module, ColorBackground);
                            break;

                        default:
                            Logger.Log(
                                Severity.ERROR, LogCategory.APP,
                                "Could not find the appropriate " +
                                "module category for module: " +
                                module.DisplayName
                            );
                            break;
                    }
                }
            };

            TabControl.SelectionChanged += TabControl_SelectionChanged;

            // Set up Category colors
            ExpanderDataInputs.Background = new SolidColorBrush(GetColor("ColorDataInputs"));
            ExpanderDataManipulation.Background = new SolidColorBrush(GetColor("ColorDataManipulation"));
            ExpanderDesigns.Background = new SolidColorBrush(GetColor("ColorDesigns"));
            ExpanderOutputs.Background = new SolidColorBrush(GetColor("ColorOutputs"));
            ExpanderImposition.Background = new SolidColorBrush(GetColor("ColorImposition"));
            ExpanderMisc.Background = new SolidColorBrush(GetColor("ColorMisc"));
            try
            {
                LoadWorkflow(filePath);
            }
            catch (Exception e)
            {
                Logger.Log(Severity.ERROR, LogCategory.APP, "Could not load workflow: " + e.Message);
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            logger.Info("Key={0},OriginalSource={1}", e.Key, e.OriginalSource);
        }

        public void CacheCurrentWorkflow()
        {
            CacheWorkflow((WorkflowScrollViewer)TabControl.SelectedContent);
        }

        /// <summary>
        /// Caches the current changes to the specified workflow.
        /// </summary>
        /// <param name="workflow">The workflow.</param>
        public void CacheWorkflow(WorkflowScrollViewer scrollViewer)
        {
            var doc = new XDocument();
            using (XmlWriter writer = doc.CreateWriter())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(WorkflowScrollViewer));
                serializer.Serialize(writer, scrollViewer);
            }
            scrollViewer.ActiveWorkflow.SetAppState(APP_NAME, doc.Root, Guid.Empty);
        }

        /// <summary>
        /// Loads a workflow instance
        /// </summary>
        /// <param name="fileName">path to .flo file. If null, create new temporary Workflow.</param>
        public void LoadWorkflow(string fileName = null)
        {
            TabItem tabItem = new TabItem();
            Brush unselectedBrush = new SolidColorBrush(ColorBackground);
            tabItem.Background = unselectedBrush;
            string headerName = "";

            logger.Info("Creating new workflow object");

            var workflow = new Workflow();

            if (fileName != null)
            {
                try
                {
                    workflow.TryLoad(fileName);
                    var elem = workflow.GetAppState(APP_NAME, Guid.Empty);
                    using (var reader = elem.CreateReader())
                    {
                        reader.ReadStartElement();
                        XmlSerializer serializer = new XmlSerializer(typeof(WorkflowScrollViewer));
                        tabItem.Content = serializer.Deserialize(reader);
                        headerName = ((WorkflowScrollViewer)tabItem.Content).XmlTabHeader;

                        void OnSelectionChanged(object obj, SelectionChangedEventArgs e)
                        {
                            if (e.AddedItems.Contains(tabItem))
                            {
                                TabControl.SelectionChanged -= OnSelectionChanged;

                                // Update Xml provided values pertaining to layout based on when
                                // the application idles after unpacking all of the controls
                                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                                    new Action(() =>
                                    {
                                        // Run first configuration pass
                                        ((WorkflowScrollViewer)tabItem.Content).OnConfigureLayout();
                                        // Schedule rendering lines after the first pass layout adjustment is finished
                                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                                                new Action(() =>
                                                {
                                                    ((WorkflowScrollViewer)tabItem.Content).OnConfigureLayout();
                                                }));
                                    }));
                            }
                        }
                        TabControl.SelectionChanged += OnSelectionChanged;
                    }
                    ((WorkflowScrollViewer)tabItem.Content).ActiveWorkflow = workflow;
                }
                catch (Exception e)
                {
                    var msg = $"Could not load workflow: {e.Message}";
                    if (e.InnerException != null)
                    {
                        msg += " " + e.InnerException.Message;
                    }
                    Logger.Log(Severity.ERROR, LogCategory.APP, msg);
                    MessageBox.Show(msg, "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                workflow.SetAsNew();
                workflow.Title = $"Workflow {TabControl.Items.Count + 1}";
                headerName = workflow.Title;
                var content = new WorkflowScrollViewer();
                content.ActiveWorkflow = workflow;
                tabItem.Content = content;
            }

            TabControl.SelectionChanged += (obj, e) =>
            {
                if (e.AddedItems.Contains(tabItem))
                {
                    tabItem.Background = Brushes.White;
                }
                else
                {
                    tabItem.Background = unselectedBrush;
                }
            };

            tabItem.Header = new TabHeader(headerName, OnTabClosed);


            TabControl.Items.Add(tabItem);
            TabControl.SelectedItem = tabItem;
            UpdateLayout();
        }

        private void OnTabClosed(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = sender as TabItem;

            var scrollViewer = (WorkflowScrollViewer)tabItem.Content;
            CacheWorkflow(scrollViewer);
            QuerySave(tabItem);
            TabControl.Items.Remove(tabItem);


        }

        public void QuerySave(TabItem tab)
        {
            var scrollViewer = (WorkflowScrollViewer)tab.Content;
            var workflow = scrollViewer.ActiveWorkflow;

            if (!workflow.HasUnsavedChanges())
            {
                return;
            }

            var result = MessageBox.Show($"{workflow.Title} has unsaved changes. Would you like to save?", "", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.Yes)
            {
                SavePrompt(tab);
            }
        }

        /// <summary>
        /// Saves the specified tab's workflow.
        /// </summary>
        /// <param name="tabItem">The tab item.</param>
        /// <returns>The result of the dialog action. True == file saved, false == cancelled action</returns>
        public bool SavePrompt(TabItem tabItem)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".flo";
            dlg.Filter = "FLO Files (*.flo)|*.flo";

            var scrollViewer = (WorkflowScrollViewer)tabItem.Content;
            var workflow = scrollViewer.ActiveWorkflow;
            if (workflow.HasDirectory())
            {
                var directory = Path.GetDirectoryName(workflow.GetFullPath());
                if (Directory.Exists(directory))
                {
                    dlg.InitialDirectory = directory;
                }
            }

            // Display SaveFileDialog by calling ShowDialog method
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string fileName = dlg.FileName;
                workflow.SetDirectory(Path.GetDirectoryName(fileName));
                workflow.Title = Path.GetFileNameWithoutExtension(fileName);
                ((TabHeader)tabItem.Header).NameLabel.Content = workflow.Title;
                CacheWorkflow(scrollViewer);
                try
                {
                    workflow.Save();
                }
                catch (Exception ex)
                {
                    var oldTitle = workflow.Title;
                    var oldPath = workflow.GetFullPath();
                    if (oldPath.Length <= 0)
                    {
                        workflow.SetDirectory(oldPath);
                    }
                    else
                    {
                        workflow.SetDirectory(Path.GetDirectoryName(oldPath));
                    }
                    workflow.Title = oldTitle;
                    ((TabHeader)tabItem.Header).NameLabel.Content = workflow.Title;
                    var msg = $"Could not save workflow {workflow.Title}: {ex.Message}";
                    Logger.Log(Severity.ERROR, LogCategory.APP, msg);
                    MessageBox.Show(msg, "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return result.GetValueOrDefault(false);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (TabControl.Items.Count <= 0)
            {
                return;
            }
            else if (TabControl.Items.Count == 1)
            {
                // Don't bother prompting if the initially-loaded workflow is blank
                var tabItem = (TabItem)TabControl.Items.GetItemAt(0);
                var curElement = SerializeState((WorkflowScrollViewer)tabItem.Content);
                int count = -1;
                int.TryParse(curElement.Element("WorkflowCanvas").Element("CanvasModules").Attribute("Count").Value, out count);
                if (count == 0 && ((WorkflowScrollViewer)tabItem.Content).ActiveWorkflow.Title == "Workflow 1")
                {
                    return;
                }
            }

            foreach (TabItem tab in TabControl.Items)
            {
                QuerySave(tab);
            }
        }


        /// <summary>
        /// Creates visual grid object for module and adds it to expander
        /// </summary>
        /// <param name="expanderName"></param>
        /// <param name="module"></param>
        /// <param name="color"></param>
        private void AppendSidebarModule(string expanderName, BaseModule module, Color color)
        {
            logger.Trace($"Expander: {expanderName}, Module: {module.DisplayName}");

            // Add module to expander
            var expander = (Expander)this.FindName(expanderName);

            Grid itemGrid = new Grid();
            itemGrid.Background = new SolidColorBrush(color);
            itemGrid.HorizontalAlignment = HorizontalAlignment.Stretch;

            ColumnDefinition iconColumn = new ColumnDefinition();
            iconColumn.Width = GridLength.Auto;
            itemGrid.ColumnDefinitions.Add(iconColumn);

            ColumnDefinition nameColumn = new ColumnDefinition();
            nameColumn.Width = GridLength.Auto;
            itemGrid.ColumnDefinitions.Add(nameColumn);

            RowDefinition row = new RowDefinition();
            row.Height = GridLength.Auto;
            itemGrid.RowDefinitions.Add(row);

            BitmapImage bmp = module.RetrieveBitmapIcon();

            Image icon = new Image();
            icon.Source = new TransformedBitmap(bmp,
                VisualHelper.GetImageScaleWithBounds(100, bmp)
            );
            Grid.SetRow(icon, 0);
            Grid.SetColumn(icon, 0);
            icon.HorizontalAlignment = HorizontalAlignment.Center;
            icon.VerticalAlignment = VerticalAlignment.Center;
            itemGrid.Children.Add(icon);

            Label displayName = new Label();
            displayName.Content = module.DisplayName;
            Grid.SetRow(displayName, 0);
            Grid.SetColumn(displayName, 1);
            displayName.HorizontalAlignment = HorizontalAlignment.Center;
            displayName.VerticalAlignment = VerticalAlignment.Center;
            itemGrid.Children.Add(displayName);

            ToolTip tooltip = new ToolTip();
            tooltip.Content = module.GetToolTipDescription();
            displayName.ToolTip = tooltip;
            icon.ToolTip = tooltip;

            logger.Trace($"Adding module to ModuleMap {module.DisplayName}");

            ModuleMap.Add(module.DisplayName, module);
            if (expander.Content != null)
            {
                ((ListBox)expander.Content).Items.Add(itemGrid);
            }
            else
            {
                ListBox list = new ListBox();
                list.Background = new SolidColorBrush(color);
                list.Items.Add(itemGrid);
                expander.Content = list;
            }

            expander.InvalidateVisual();

            // Click and drag new module from left side-bar
            itemGrid.Tag = module;
            itemGrid.PreviewMouseLeftButtonDown += delegate (object sender, MouseButtonEventArgs e)
            {
                CreateModuleObject(module);
            };
        }

        /// <summary>
        /// Dragdrop operation for new module object starts from here
        /// </summary>
        /// <param name="module"></param>
        /// <param name="rootCanvas"></param>
        private void CreateModuleObject(BaseModule module)
        {
            //Canvas rootCanvas = (Canvas)FindName("RootCanvas");
            DragCanvas rootCanvas = (DragCanvas)FindName("RootCanvas");
            logger.Trace($"Creating module object of Type: {module.GetType()}");

            //New module creation starts from hee
            var newModule = ((App)App.Current).CreateModule(module.GetType());
            logger.Trace("Creating CanvasModule for module object {0}", newModule.DisplayName);

            var canvasModule = new CanvasModule(((BaseModule)newModule));

            rootCanvas.Children.Add(canvasModule);
            canvasModule.Parent2 = rootCanvas;
        }

        public void Module_RequestStateUpdate(Workflow workflow)
        {
            ((WorkflowScrollViewer)TabControl.SelectedContent).ActiveWorkflow = workflow;
        }

        /// <summary>
        /// Returns the appropriate workflow for the specified module.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <returns>The appropriate workflow, 
        /// else null if the module is not found as a child of any canvases.</returns>
        public Workflow Module_GetWorkflow(BaseModule module)
        {
            foreach (TabItem item in TabControl.Items)
            {
                var scrollView = (WorkflowScrollViewer)item.Content;
                var canvas = scrollView.ChildCanvas;
                if (canvas.ContainsModule(module))
                {
                    return scrollView.ActiveWorkflow;
                }
            }
            return null;
        }

        private void NewWorkflow_Click(object sender, RoutedEventArgs e)
        {
            LoadWorkflow(null);
        }

        private void OpenWorkflow_Click(object sender, RoutedEventArgs e)
        {
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
                    LoadWorkflow(fileName);
                }
                catch (Exception err)
                {
                    Logger.Log(Severity.ERROR, LogCategory.APP, "Could not load workflow: " + err.Message);
                }
            }
        }

        private void SaveWorkflow_Click(object sender, RoutedEventArgs e)
        {
            SavePrompt((TabItem)TabControl.SelectedItem);
        }

        private XElement SerializeState(WorkflowScrollViewer scrollViewer)
        {
            using (var stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(WorkflowScrollViewer));
                serializer.Serialize(stream, scrollViewer);
                stream.Position = 0;

                using (XmlReader reader = XmlReader.Create(stream))
                {
                    return XElement.Load(reader);
                }
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((TabItem)TabControl.SelectedItem)?.Focus();
        }
    }
}