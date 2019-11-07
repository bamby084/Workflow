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
        public MainWindow()
        {
            InitializeComponent();
            UnitOfMeasure.Current.UnitType = UnitType.Centimeter;
            InitTables();
            this.DataContext = this;

            Tools = new ObservableCollection<DesignerTool>();
            Tools.Add(new SelectionTool());
            Tools.Add(new DrawingBlockTool());

            this.Loaded += MainWindow_Loaded;
            Canvas.SelectedItemsChanged += SelectedItemsChanged;
            Canvas.ItemAdded += OnItemAdded;
            Canvas.ItemsDeleted += OnItemsDeleted;
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

            Canvas.RemoveItem((DesignerItem)block.AssociatedItem);
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

        private void InitTables()
        {
            Tables = new ObservableCollection<TableTreeViewItem>();
            var table1 = new TableTreeViewItem();
            table1.Name = "Table 1";
            table1.RowSets.Add(new RowSetTreeViewItem() { Name = "Header RowSet" });
            table1.RowSets.Add(new RowSetTreeViewItem() { Name = "Body RowSet" });

            table1.RowSets[0].Rows.Add(new RowTreeViewItem() { Name = "Row 1" });
            table1.RowSets[0].Rows.Add(new RowTreeViewItem() { Name = "Row 2" });
            table1.RowSets[0].Rows.Add(new RowTreeViewItem() { Name = "Row 3" });
            table1.RowSets[0].Rows[0].Cells.Add(new CellTreeViewItem() { Name = "Cell 1" });
            table1.RowSets[0].Rows[0].Cells.Add(new CellTreeViewItem() { Name = "Cell 2" });
            table1.RowSets[0].Rows[0].Cells.Add(new CellTreeViewItem() { Name = "Cell 3" });
            table1.RowSets[0].Rows[1].Cells.Add(new CellTreeViewItem() { Name = "Cell 4" });
            table1.RowSets[0].Rows[1].Cells.Add(new CellTreeViewItem() { Name = "Cell 5" });
            table1.RowSets[0].Rows[1].Cells.Add(new CellTreeViewItem() { Name = "Cell 6" });

            var table2 = new TableTreeViewItem();
            table2.Name = "Table 2";
            table2.RowSets.Add(new RowSetTreeViewItem() { Name = "Header RowSet" });
            table2.RowSets.Add(new RowSetTreeViewItem() { Name = "Body RowSet" });

            table2.RowSets[0].Rows.Add(new RowTreeViewItem() { Name = "Row 4" });
            table2.RowSets[0].Rows.Add(new RowTreeViewItem() { Name = "Row 5" });
            table2.RowSets[0].Rows.Add(new RowTreeViewItem() { Name = "Row 6" });
            table2.RowSets[0].Rows[0].Cells.Add(new CellTreeViewItem() { Name = "Cell 1" });
            table2.RowSets[0].Rows[0].Cells.Add(new CellTreeViewItem() { Name = "Cell 2" });
            table2.RowSets[0].Rows[0].Cells.Add(new CellTreeViewItem() { Name = "Cell 3" });
            table2.RowSets[0].Rows[1].Cells.Add(new CellTreeViewItem() { Name = "Cell 4" });
            table2.RowSets[0].Rows[1].Cells.Add(new CellTreeViewItem() { Name = "Cell 5" });
            table2.RowSets[0].Rows[1].Cells.Add(new CellTreeViewItem() { Name = "Cell 6" });

            Tables.Add(table1);
            Tables.Add(table2);
        }

        public ObservableCollection<TableTreeViewItem> Tables
        {
            get; set;
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

            if(e.NewValue is IControlPropertyProvider propertyProvider)
            {
                SelectedControlProperties = propertyProvider.Properties;
            }
            else
            {
                SelectedControlProperties = null;
            }
        }
    }

    public class TableTreeViewItem
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public ObservableCollection<RowSetTreeViewItem> RowSets { get; set; }

        public TableTreeViewItem()
        {
            RowSets = new ObservableCollection<RowSetTreeViewItem>();
        }
    }

    public class RowSetTreeViewItem
    {
        public string Name { get; set; }
        public string Icon { get; set; }

        public ObservableCollection<RowTreeViewItem> Rows { get; set; }

        public RowSetTreeViewItem()
        {
            Rows = new ObservableCollection<RowTreeViewItem>();
        }
    }

    public class RowTreeViewItem
    {
        public string Name { get; set; }
        public ObservableCollection<CellTreeViewItem> Cells { get; set; }

        public RowTreeViewItem()
        {
            Cells = new ObservableCollection<CellTreeViewItem>();
        }
    }

    public class CellTreeViewItem
    {
        public string Name { get; set; }
    }

}
