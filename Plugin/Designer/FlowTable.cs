using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Designer
{
    public enum FlowTableAlignment
    {
        Left,
        Right
    }

    public class FlowTablePresenter: Panel
    {
        public event EventHandler SelectedCellChange;

        private ContextMenu SingleCellContextMenu { get; set; }
        private  ContextMenu MultipleCellsContextMenu { get; set; }
        private ContextMenu CellContextMenu => SelectedCells.Count == 1 ? SingleCellContextMenu : MultipleCellsContextMenu;

        public List<FlowTableCell> SelectedCells { get; set; }
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public FrameworkElement Root { get; private set; }
        
        public FlowTablePresenter(FlowTableSettings settings)
        {
            SelectedCells = new List<FlowTableCell>();
            Id = Guid.NewGuid();
            CreateContextMenus();
            Build(settings);
        }

        private void Build(FlowTableSettings settings)
        {
            var table = new Table();
            AddRowGroup(table, settings.HeaderRows + settings.BodyRows + settings.FooterRows, settings.Columns);
            
            var container = new FlowDocumentScrollViewer();
            container.IsSelectionEnabled = false;
            container.ContextMenu = null;

            container.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            var flowDocument = new FlowDocument();
            flowDocument.Blocks.Add(table);

            container.Document = flowDocument;
            Root = container;
            Children.Add(Root);
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

        private void AddRowGroup(Table table, int rows, int columns)
        {
            if (rows == 0)
                return;

            var rowGroup = new TableRowGroup();
            int index = table.GetNewRowGroupIndex();

            for (int i = 0; i < rows; i++)
            {
                var row  = new FlowTableRow();
                row.GroupIndex = index;
                row.Index = index + i;
                AddCells(row, columns);
                
                rowGroup.Rows.Add(row);
            }

            table.RowGroups.Add(rowGroup);
        }

        private void AddCells(FlowTableRow row, int columns)
        {
            for (int i = 0; i < columns; i++)
            {
                var cell = new FlowTableCell
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
            FlowTableCell cell = (FlowTableCell) sender;

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
            FlowTableCell cell = (FlowTableCell) sender;
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
            foreach (FlowTableCell cell in SelectedCells)
            {
                cell.IsSelected = false;
            }

            SelectedCells.Clear();
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

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            return Root;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Root.Measure(availableSize);
            return Root.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Root.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return finalSize;
        }
    }

    public class FlowTable
    {
        public Guid Id { get;}
        public FlowTableSettings Settings { get;}
        public IList<FlowTablePresenter> TablePresenters { get; }

        public FlowTable()
        {
            Id = Guid.NewGuid();
            Settings = new FlowTableSettings();
            TablePresenters = new List<FlowTablePresenter>();
        }

        public FlowTablePresenter NewPresenter()
        {
            var presenter = new FlowTablePresenter(Settings);
            presenter.ParentId = this.Id;
            TablePresenters.Add(presenter);

            return presenter;
        }

        public FlowTablePresenter GetPresenter(Guid id)
        {
            return TablePresenters.FirstOrDefault(table => table.Id == id);
        }

        public static FlowTable Default()
        {
            var table = new FlowTable();
            table.Settings.Columns = 2;
            table.Settings.BodyRows = 2;
            table.Settings.WidthPercentage = 1;

            return table;
        }
    }

    public class FlowTableSettings
    {
        public int Columns { get; set; }
        public int HeaderRows { get; set; }
        public int FooterRows { get; set; }
        public int BodyRows { get; set; }
        public double WidthPercentage { get; set; } = 1.0;
        public double MinWidth { get; set; }
        public FlowTableAlignment Alignment { get; set; }
    }

    public class FlowTableCell : TableCell
    {
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(FlowTableCell), new PropertyMetadata(default(bool)));

        public bool IsSelected
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public bool IsMerged { get; set; }
        public FlowTableRow ParentRow { get; set; }
    }

    public class FlowTableRow : TableRow
    {
        public int Index { get; set; }
        public int GroupIndex { get; set; }
    }

    public static class TableExtensions
    {
        public static int GetNewRowGroupIndex(this Table table)
        {
            var totalRows = table.RowGroups.Sum(rowGroup => rowGroup.Rows.Count);
            return totalRows;
        }
    }
}
