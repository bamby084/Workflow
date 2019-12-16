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

        public TableProperties Properties { get; private set; }

        public void Build(TableProperties tableProperties)
        {
            this.Properties = tableProperties;

            CreateContextMenus();
            SetCellSpacing();
            BindIsSelected();
            CreateColumns();
            CreateRows();
            RegisterEvents();
        }

        private void CreateColumns()
        {
            foreach(var columnDef in Properties.ColumnDefinitions)
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
            foreach(var rowSet in Properties.RowSets)
            {
                AddNewRowGroup(rowSet);
            }
        }

        private void RegisterEvents()
        {
            this.Properties.OnRowSetAdded += OnAddNewRowSet;
            this.Properties.OnRowSetDeleted += OnRowSetDeleted;
        }

        private void OnRowSetDeleted(object sender, EventArgs e)
        {
            var rowSet = (RowSet)sender;
            var rowGroup = this.RowGroups.FirstOrDefault(rg => ((DesignerTableRowGroup)rg).Id == rowSet.Id);
            if (rowGroup == null)
                return;

            this.RowGroups.Remove(rowGroup);
        }

        private void OnAddNewRowSet(object sender, EventArgs e)
        {
            var rowSet = (RowSet)sender;
            AddNewRowGroup(rowSet);
        }

        private void AddNewRowGroup(RowSet rowSet)
        {
            var rowGroup = new DesignerTableRowGroup();
            rowGroup.Id = rowSet.Id;
            
            for (int i = 0; i < rowSet.Rows.Count; i++)
            {
                AddNewRow(rowGroup, rowSet.Rows[i]);
            }

            this.RowGroups.Add(rowGroup);
            rowSet.OnAddNewRow += OnAddNewRow;
            rowSet.OnDeleteRow += OnDeleteRow;
        }

        private void OnCellRemoved(object sender, EventArgs e)
        {
            var removedCell = (DesignerTableCell)sender;
            var rowSet = Properties.RowSets.FirstOrDefault(rs => rs.Id == removedCell.ParentRow.GroupId);
            if (rowSet == null)
                return;

            var row = rowSet.Rows.FirstOrDefault(r => r.Id == removedCell.ParentRow.Id);
            if (row == null)
                return;

            row.RemoveCell(removedCell.Id);   
        }

        private void OnCellClicked(object sender, MouseButtonEventArgs e)
        {
            DesignerTableCell cell = (DesignerTableCell)sender;

            if (e.ChangedButton == MouseButton.Left)
            {
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
            else if(e.ChangedButton == MouseButton.Right)
            {
                //is right click on selected cells?
                if (!SelectedCells.Contains(cell))
                {
                    ClearSelectedCells();
                    cell.IsSelected = true;
                    SelectedCells.Add(cell);
                }

                CellContextMenu.IsOpen = true;
            }
        }

        private void OnDeleteRow(object sender, EventArgs e)
        {
            Row row = (Row)sender;
            var rowGroup = (DesignerTableRowGroup)this.RowGroups.FirstOrDefault(rg => ((DesignerTableRowGroup)rg).Id == row.Parent.Id);
            if (rowGroup == null)
                return;

            rowGroup.RemoveRow(row.Id);
        }

        private void OnAddNewRow(object sender, EventArgs e)
        {
            Row row = (Row)sender;
            var rowGroup = (DesignerTableRowGroup)this.RowGroups.FirstOrDefault(rg => ((DesignerTableRowGroup)rg).Id == row.Parent.Id);
            if (rowGroup == null)
                return;

            AddNewRow(rowGroup, row);
        }

        private void AddNewRow(DesignerTableRowGroup rowGroup, Row row)
        {
            var newRow = rowGroup.AddNewRow(row.Cells);
            newRow.Id = row.Id;
            newRow.OnCellClicked += OnCellClicked;
            newRow.OnCellRemoved += OnCellRemoved;
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
            mergeCellsMenuItem.IsEnabled = false;
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
                || SelectedCells.GroupBy(c => c.ParentRow.GroupId).Count() > 1)
                return false;

            var query = (from cell in SelectedCells
                group cell by cell.ParentRow.Index
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
                         group cell by cell.ParentRow.Index
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
                cell.ParentRow.RemoveCell(cell);
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

        public Guid Id { get; set; }

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
        public event MouseButtonEventHandler OnCellClicked;
        public event EventHandler OnCellRemoved;

        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public int Index { get; set; }

        public void AddCells(IList<Cell> cells)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                var cell = new DesignerTableCell()
                {
                    ParentRow = this,
                    ColumnIndex = i,
                    Id = cells[i].Id
                };

                cell.PreviewMouseLeftButtonDown += CellClicked;
                cell.PreviewMouseRightButtonDown += CellClicked;

                this.Cells.Add(cell);
            }
        }

        public void RemoveCell(DesignerTableCell cell)
        {
            this.Cells.Remove(cell);
            OnCellRemoved?.Invoke(cell, new EventArgs());
        }

        private void CellClicked(object sender, MouseButtonEventArgs e)
        {
            OnCellClicked?.Invoke(sender, e);
        }
    }

    public class DesignerTableRowGroup: TableRowGroup
    {
        public Guid Id { get; set; }
        
        public DesignerTableRow AddNewRow(IList<Cell> cells)
        {
            var row = new DesignerTableRow();
            row.GroupId = this.Id;
            row.Index = this.Rows.Count;
            row.AddCells(cells);

            Rows.Add(row);
            return row;
        }

        public void RemoveRow(DesignerTableRow row)
        {
            Rows.Remove(row);
            Invalidate();
        }

        public void RemoveRow(Guid id)
        {
            var row = (DesignerTableRow)Rows.FirstOrDefault(r => ((DesignerTableRow)r).Id == id);
            if (row != null)
                RemoveRow(row);
        }

        public void Invalidate()
        {
            for (int i = 0; i < Rows.Count; i++)
            {
                ((DesignerTableRow)Rows[i]).Index = i;
            }
        }
    }
}
