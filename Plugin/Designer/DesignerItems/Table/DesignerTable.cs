using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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
            Properties = new TableProperties();

            Properties.Name = $"Table {GetNextIndex()}";
        }

        public List<DesignerTableCell> SelectedCells { get; }

        public TableProperties Properties { get; set; }

        public void Build(int columnCount, int headerRowCount, int bodyRowCount, int footerRowCount)
        {
            CreateContextMenus();
            CreateColumns(columnCount);
            
            var headerGroup = CreateRowGroup(headerRowCount, columnCount);
            this.RowGroups.Add(headerGroup);

            var bodyGroup = CreateRowGroup(bodyRowCount, columnCount);
            this.RowGroups.Add(bodyGroup);

            var footerGroup = CreateRowGroup(footerRowCount, columnCount);
            this.RowGroups.Add(footerGroup);
        }

        private void CreateColumns(int columnCount)
        {
            double defaultColumnWidthPercentage = 1.0 / columnCount;

            for (int i = 0; i < columnCount; i++)
            {
                var column = new TableColumn();
                var columnDefition = new ColumnDefinition();
                columnDefition.Width = defaultColumnWidthPercentage;

                var widthBinding = new Binding("Width");
                widthBinding.Source = columnDefition;
                widthBinding.Mode = BindingMode.TwoWay;
                widthBinding.Converter = new GridLengthConverter();
                widthBinding.ConverterParameter = GridUnitType.Star;
                column.SetBinding(TableColumn.WidthProperty, widthBinding);

                this.Properties.AddColumnDefition(columnDefition);
                this.Columns.Add(column);
            }
        }

        private TableRowGroup CreateRowGroup(int rows, int columns)
        {
            if (rows == 0)
                return new TableRowGroup();

            var rowGroup = new TableRowGroup();
            int index = this.GetNewRowGroupIndex();

            for (int i = 0; i < rows; i++)
            {
                var row = new DesignerTableRow
                {
                    GroupIndex = index,
                    Index = index + i
                };

                AddCells(row, columns);
                rowGroup.Rows.Add(row);
            }

            return rowGroup;
        }

        private void AddCells(DesignerTableRow row, int columns)
        {
            for (int i = 0; i < columns; i++)
            {
                var cell = new DesignerTableCell
                {
                    ParentRow = row,
                    ColumnIndex = i,
                    RowIndex = row.Index
                };

                cell.PreviewMouseLeftButtonDown += OnCellLeftMouseDown;
                cell.PreviewMouseRightButtonDown += OnCellRightMouseDown;

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
    }

    public class DesignerTableCell : TableCell
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
}
