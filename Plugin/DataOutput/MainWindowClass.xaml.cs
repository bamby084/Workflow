using System;
using System.Windows;
using System.Windows.Input;
using DataOutput.ViewModel;
using System.Collections.ObjectModel;
using JdSuite.Common.Module;
using System.Xml.Linq;
using JdSuite.Common.TreeListView;

namespace DataOutput
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowClass : Window
    {
        NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(MainWindowClass));

        private Point LastPos = new Point(0.0, 0.0);

        private ObservableCollection<OutputViewModel> Items { get; set; }

        public MainWindowClass()
        {
            Items = new ObservableCollection<OutputViewModel>();
            InitializeComponent();
            // dataGrid.ItemsSource = Items;
        }

        public void AddItem(OutputViewModel item)
        {
            Items.Add(item);
        }

        Field _schema;
        public Field Schema
        {
            get { return _schema; }
            set { _schema = value; }
        }

        

        public string DataFile
        {
            get; set;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            logger.Info("Loading Output Module Main Window");

            RefreshData();

        }


        public void RefreshData()
        {
            try
            {
                DataItemFactory df = new DataItemFactory();
                df.LoadData(DataFile, Schema);

               

                XMLFieldValidator validator = new XMLFieldValidator();
                validator.Validate(df.RootDataNode, df.RootSchemaNode);
                validator.LogEachError();

                treeGrid.LoadData(df);

                //xmlOutputControl.RefreshData();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageService.ShowError("Data Loading Error", ex.Message);
            }
        }

        private void Splitter_MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    var pos2 = Mouse.GetPosition(dockPanelParent);
            //    double ydelta = pos2.Y - LastPos.Y;

            //    if (ydelta > 0)
            //    {
            //        xmlOutputControl.Height += ydelta;


            //        var top = Canvas.GetTop(this.splitter);
            //        Canvas.SetTop(this.splitter, top + ydelta);
            //    }else
            //    {
            //        xmlOutputControl.Height += ydelta;
            //        var top = Canvas.GetTop(this.splitter);
            //        Canvas.SetTop(this.splitter, top + ydelta);
            //    }
            //    LastPos = pos2;
            //}
        }

        private void Splitter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                LastPos = Mouse.GetPosition(dockPanelParent);
            }
        }
    }


}
