using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Designer;
using Designer.Adorners;
using Designer.DesignerItems;
using Designer.DesignerTools;
using Designer.DesignerTreeViewItems;

namespace TestHarness
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private FlowTablePresenter _table;
        public MainWindow()
        {
            InitializeComponent();
            UnitOfMeasure.Current.UnitType = UnitType.Centimeter;
            this.DataContext = this;

            Tools = new ObservableCollection<DesignerTool>();
            Tools.Add(new SelectionTool());
            Tools.Add(new DrawingBlockTool());

            this.Loaded += MainWindow_Loaded;
            Canvas.SelectedItemsChanged += SelectedItemsChanged;
            Canvas.ItemAdded += OnItemAdded;
            Canvas.ItemsDeleted += OnItemsDeleted;
        }

        private void SelectedItemsChanged(object sender, ItemsChangedEventArgs e)
        {
            if (e.Items.Count != 1)
            {
                SelectedControlProperties = null;
                return;
            }

            SelectedControlProperties = e.Items[0].Properties;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Tools[0].IsSelected = true;
        }

        private void OnItemsDeleted(object sender, ItemsChangedEventArgs e)
        {
            var firstPage = Pages.Items[0] as TreeViewItem;
            List<BlockTreeViewItem> itemsToRemove = new List<BlockTreeViewItem>();

            foreach(DesignerItem item in e.Items)
            {
                foreach(BlockTreeViewItem block in firstPage.Items)
                {
                    if(block.AssociatedItem.Equals(item))
                    {
                        itemsToRemove.Add(block);
                    }
                }
            }

            foreach(var item in itemsToRemove)
            {
                firstPage.Items.Remove(item);
            }
        }

        private void OnItemAdded(object sender, ItemAddedEventArgs e)
        {
            var firstPage = Pages.Items[0] as TreeViewItem;
            var block = new BlockTreeViewItem();
            block.AssociatedItem = e.Item;
            block.OnDeleted += OnBlockDeleted;
            firstPage.Items.Add(block);
        }

        private void OnBlockDeleted(object sender, EventArgs e)
        {
            var block = sender as BlockTreeViewItem;
            var firstPage = Pages.Items[0] as TreeViewItem;

            Canvas.RemoveItem(block.AssociatedItem);
            firstPage.Items.Remove(block);
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
    }
}
