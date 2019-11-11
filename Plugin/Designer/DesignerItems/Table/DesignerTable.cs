using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GridLengthConverter = Designer.Converters.GridLengthConverter;

namespace Designer.DesignerItems
{
    public class DesignerTable: Table, ISelectable, IDisposable
    {
        private static readonly object LockObject = new object();
        private static int CurrentIndex;

        public event EventHandler SelectedCellChange;
        private ContextMenu SingleCellContextMenu { get; set; }
        private ContextMenu MultipleCellsContextMenu { get; set; }
        private ContextMenu CellContextMenu => SelectedCells.Count == 1 ? SingleCellContextMenu : MultipleCellsContextMenu;

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
           "IsSelected",
           typeof(bool),
           typeof(DesignerTable),
           new FrameworkPropertyMetadata(false)
           );

        public static readonly DependencyProperty IsSelectableProperty = DependencyProperty.Register(
           "IsSelectable",
           typeof(bool),
           typeof(DesignerTable),
           new FrameworkPropertyMetadata(true)
           );

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public bool IsSelectable
        {
            get => (bool)GetValue(IsSelectableProperty);
            set => SetValue(IsSelectableProperty, value);
        }

        public virtual void Dispose()
        {

        }

        public DesignerTable()
        {
            SelectedCells = new List<DesignerTableCell>();
        }

        public List<DesignerTableCell> SelectedCells { get; }

        public ControlPropertiesViewModel Properties { get; private set; }

        public void Build(TableProperties tableProperties)
        {
            tableProperties.Name = $"Table {GetNextIndex()}";
            this.Properties = tableProperties;

            CreateContextMenus();
            SetCellSpacing();
            BindIsSelected();
            CreateColumns();
            CreateRows();
        }

        private void CreateColumns()
        {
            var tableProperies = (TableProperties)this.Properties;
            foreach(var columnDef in tableProperies.ColumnDefinitions)
            {
                var binding = new Binding("Width");
                binding.Source = columnDef;
                binding.Converter = new GridLengthConverter();
                binding.ConverterParameter = GridUnitType.Star;

                var column = new TableColumn();
                column.SetBinding(TableColumn.WidthProperty, binding);
                this.Columns.Add(column);
            }
        }

        private void CreateRows()
        {
            var tableProperties = (TableProperties)this.Properties;
            foreach(var rowSet in tableProperties.RowSets)
            {
                int index = this.GetNewRowGroupIndex();
                var rowGroup = new DesignerTableRowGroup();
                rowGroup.Id = rowSet.Id;
                rowGroup.Index = index;

                for (int i = 0; i < rowSet.Rows.Count; i++)
                {
                    var row = new DesignerTableRow()
                    {
                        GroupIndex = index,
                        Index = index + i
                    };
                    
                    AddCells(row, rowSet.Rows[i].Cells);
                    rowGroup.Rows.Add(row);
                }

                this.RowGroups.Add(rowGroup);
                rowSet.OnAddNewRow += OnAddNewRow;
            }
        }

        private void OnAddNewRow(object sender, EventArgs e)
        {
            RowSet rowSet = (RowSet)sender;
            var rowGroup = (DesignerTableRowGroup)this.RowGroups.FirstOrDefault(rg => ((DesignerTableRowGroup)rg).Id == rowSet.Id);
            if (rowGroup == null)
                return;

            var row = new DesignerTableRow()
            {
                Index = rowGroup.Rows.Count + 1,
                GroupIndex = rowGroup.Index
            };

            AddCells(row, rowSet.Rows.Last().Cells);
            rowGroup.Rows.Add(row);
        }

        private void AddCells(DesignerTableRow row, IList<Cell> cells)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                var cell = new DesignerTableCell
                {
                    ParentRow = row,
                    ColumnIndex = i,
                    RowIndex = row.Index
                };

                cell.PreviewMouseLeftButtonDown += OnCellLeftMouseDown;
                cell.PreviewMouseRightButtonDown += OnCellRightMouseDown;

                //var backgroundBinding = new Binding("Background");
                //backgroundBinding.Source = cells[i];
                //backgroundBinding.Mode = BindingMode.OneWay;
                //cell.SetBinding(BackgroundProperty, backgroundBinding);

                row.Cells.Add(cell);
            }
        }

        private void OnCellRightMouseDown(object sender, MouseButtonEventArgs e)
        {
            DesignerTableCell cell = (DesignerTableCell)sender;

            //is right click on selected cells?
            if (!SelectedCells.Contains(cell))
            {
                ClearSelectedCells();
                cell.IsSelected = true;
                SelectedCells.Add(cell);
            }

            CellContextMenu.IsOpen = true;
        }

        private void OnCellLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            DesignerTableCell cell = (DesignerTableCell)sender;
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                cell.IsSelected = !cell.IsSelected;
                if (cell.IsSelected)
                    SelectedCells.Add(cell);
                else
                    SelectedCells.Remove(cell);
            }
            else
            {
                ClearSelectedCells();
                cell.IsSelected = true;
                SelectedCells.Add(cell);
            }

            SelectedCellChange?.Invoke(cell, new EventArgs());
        }

        private void ClearSelectedCells()
        {
            foreach (DesignerTableCell cell in SelectedCells)
            {
                cell.IsSelected = false;
            }

            SelectedCells.Clear();
        }

        private void CreateContextMenus()
        {
            SingleCellContextMenu = new ContextMenu();
            SingleCellContextMenu.Items.Add(new MenuItem()
            {
                Header = "Insert Table"
            });
            SingleCellContextMenu.Items.Add(new MenuItem()
            {
                Header = "Insert Image"
            });
            SingleCellContextMenu.Items.Add(new MenuItem()
            {
                Header = "Insert Text"
            });

            MultipleCellsContextMenu = new ContextMenu();
            var mergeCellsMenuItem = new MenuItem();
            mergeCellsMenuItem.Header = "Merge Cells";
            mergeCellsMenuItem.Click += (sender, e) =>
            {
                MergeCells();
            };
            MultipleCellsContextMenu.ContextMenuOpening += (sender, e) =>
            {
                mergeCellsMenuItem.IsEnabled = CanMergeCells();
            };

            MultipleCellsContextMenu.Items.Add(mergeCellsMenuItem);
        }

        private bool CanMergeCells()
        {
            if (SelectedCells.Count < 2 || SelectedCells.Any(cell => cell.IsMerged)
                || SelectedCells.GroupBy(c => c.ParentRow.GroupIndex).Count() > 1)
                return false;

            var query = (from cell in SelectedCells
                group cell by cell.RowIndex
                into g
                select new
                {
                    Row = g.Key,
                    Columns = g.Select(c => c.ColumnIndex).OrderBy(col => col).ToArray()
                }).OrderBy(g => g.Row).ToArray();

            if (!IsContinousArray(query[0].Columns))
                return false;

            if (query.Length > 1)
            {
                for (int i = 1; i < query.Length; i++)
                {
                    if (!(query[i].Row == query[0].Row + i && IsContinousArray(query[i].Columns)
                          && query[i].Columns.Length == query[0].Columns.Length
                          && query[i].Columns[0] == query[0].Columns[0]))
                        return false;
                }
            }

            return true;
        }

        private void MergeCells()
        {
            var query = (from cell in SelectedCells
                group cell by cell.RowIndex
                into g
                select new
                {
                    Row = g.Key,
                    Cells = g.OrderBy(c => c.ColumnIndex).ToList()
                }).OrderBy(g => g.Row).ToList();

            var mergedCell = query[0].Cells[0];
            mergedCell.ColumnSpan = query[0].Cells.Count;
            mergedCell.RowSpan = query.Count;
            mergedCell.IsMerged = true;
            SelectedCells.Remove(mergedCell);
            foreach (var cell in SelectedCells)
            {
                cell.ParentRow.Cells.Remove(cell);
            }

            SelectedCells.Clear();
            SelectedCells.Add(mergedCell);
        }

        private bool IsContinousArray(int[] array)
        {
            if (array.Length < 2)
                return true;

            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] != array[0] + i)
                    return false;
            }

            return true;
        }

        private int GetNextIndex()
        {
            lock (LockObject)
            {
                CurrentIndex++;
                return CurrentIndex;
            }
        }

        private void SetCellSpacing()
        {
            var cellSpacingBinding = new Binding("CellSpacing");
            cellSpacingBinding.Source = Properties;
            cellSpacingBinding.Mode = BindingMode.TwoWay;
            this.SetBinding(CellSpacingProperty, cellSpacingBinding);
        }

        private void BindIsSelected()
        {
            var binding = new Binding("IsSelected");
            binding.Source = this.Properties;
            binding.Mode = BindingMode.TwoWay;
            SetBinding(IsSelectedProperty, binding);
        }
    }

    public class DesignerTableCell : TableCell, ISelectable
    {
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(DesignerTableCell), new PropertyMetadata(default(bool)));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public int RowIndex { get; set; }

        public int ColumnIndex { get; set; }

        public bool IsMerged { get; set; }

        public DesignerTableRow ParentRow { get; set; }

        public bool IsSelectable { get; set; } = true;

        public DesignerTableCell()
        {
        }

        public DesignerTableCell(Block item)
            :base(item)
        {
            
        }

    }

    public class DesignerTableRow : TableRow
    {
        public int Index { get; set; }

        public int GroupIndex { get; set; }
    }

    public class DesignerTableRowGroup: TableRowGroup
    {
        public Guid Id { get; set; }
        public int Index { get; set; }
    }

}
