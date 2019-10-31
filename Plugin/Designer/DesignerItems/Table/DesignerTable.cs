﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Designer.DesignerItems
{
    public class DesignerTable: Table
    {
        public event EventHandler SelectedCellChange;
        private ContextMenu SingleCellContextMenu { get; set; }
        private ContextMenu MultipleCellsContextMenu { get; set; }
        private ContextMenu CellContextMenu => SelectedCells.Count == 1 ? SingleCellContextMenu : MultipleCellsContextMenu;

        public DesignerTable()
        {
            SelectedCells = new List<DesignerTableCell>();
            Properties = new DesignerTablePropertiesViewModel();
        }

        public List<DesignerTableCell> SelectedCells { get; }

        public DesignerTablePropertiesViewModel Properties { get; set; }

        public void Build()
        {
            CreateContextMenus();
            var headerGroup = CreateRowGroup(Properties.HeaderRows, Properties.Columns);
            this.RowGroups.Add(headerGroup);

            var bodyGroup = CreateRowGroup(Properties.BodyRows, Properties.Columns);
            this.RowGroups.Add(bodyGroup);

            var footerGroup = CreateRowGroup(Properties.FooterRows, Properties.Columns);
            this.RowGroups.Add(footerGroup);
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
