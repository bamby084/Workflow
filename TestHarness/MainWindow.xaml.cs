using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Designer;
using Designer.DesignerItems;
using Designer.DesignerTools;

namespace TestHarness
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            UnitOfMeasure.Current.UnitType = UnitType.Centimeter;
            this.DataContext = this;

            Tools = new ObservableCollection<DesignerTool>();
            Tools.Add(new SelectionTool());
            Tools.Add(new DrawingBlockTool());

            this.Loaded += MainWindow_Loaded;
            Canvas.ItemAdded += OnItemAdded;
            //Canvas.ItemsDeleted += OnItemsDeleted;
        }

        private DesignerTool _selectedTool;

        public DesignerTool SelectedTool
        {
            get => _selectedTool;
            set
            {
                _selectedTool = value;
                if (_selectedTool != null)
                {
                    _selectedTool.Canvas = Canvas;
                    _selectedTool.ResetAdorner();
                }

                NotifyPropertyChanged();
            }
        }

        //private void SelectedItemsChanged(object sender, ItemsChangedEventArgs e)
        //{
        //    if (e.Items.Count != 1)
        //    {
        //        SelectedControlProperties = null;
        //        return;
        //    }

        //    SelectedControlProperties = e.Items[0].Properties;
        //}

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Tools[0].IsSelected = true;
        }

        private void OnItemAdded(object sender, ItemAddedEventArgs e)
        {
            var firstPage = Pages.Items[0] as TreeViewItem;
            var blockProperties = (BlockProperties)e.Item.Properties;
            blockProperties.OnDelete += delegate
            {
                firstPage.Items.Remove(blockProperties);
                e.Item.Dispose();
            };

            e.Item.OnDisposed += delegate
            {
                firstPage.Items.Remove(blockProperties);
            };

            firstPage.Items.Add(blockProperties);
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<DesignerTool> Tools { get; set; }

        private ControlPropertiesViewModel _selectedControlProperties;
        public ControlPropertiesViewModel SelectedControlProperties {
            get => _selectedControlProperties;
            set
            {
                _selectedControlProperties = value;
                NotifyPropertyChanged();
            }
        }

        private UnitType _selectedUnit;
        public UnitType SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                _selectedUnit = value;
                NotifyPropertyChanged();
                UnitOfMeasure.Current.UnitType = value;
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(e.OldValue is ISelectable oldSelectableObject)
            {
                oldSelectableObject.IsSelected = false;
            }

            if(e.NewValue is ISelectable newSelectableObject)
            {
                newSelectableObject.IsSelected = true;
            }

            if (e.NewValue is ControlPropertiesViewModel propertiesViewModel)
            {
                SelectedControlProperties = propertiesViewModel;
            }
            else
            {
                SelectedControlProperties = null;
            }
        }
    }


}
