﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace Designer.DesignerItems
{
    public class TableProperties: ControlPropertiesViewModel, ISelectable
    {
        public event EventHandler OnDeleted;
        public TableProperties()
        {
            _columnDefinitions = new ObservableCollection<TableColumnDefinition>();
            _rowSets = new ObservableCollection<RowSet>();

            DeleteCommand = new RelayCommand(DeleteTable);
        }

        #region General
        private double _widthPercentage = 100.0;
        public double WidthPercentage
        {
            get => _widthPercentage;
            set
            {
                if (value != _widthPercentage)
                {
                    _widthPercentage = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private double _minWidth = 1.0;
        public double MinWidth
        {
            get => _minWidth;
            set
            {
                if (_minWidth != value)
                {
                    _minWidth = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<TableColumnDefinition> _columnDefinitions;
        public ObservableCollection<TableColumnDefinition> ColumnDefinitions
        {
            get => _columnDefinitions;
            set
            {
                if (value != _columnDefinitions)
                {
                    _columnDefinitions = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<RowSet> _rowSets;
        public ObservableCollection<RowSet> RowSets
        {
            get => _rowSets;
            set
            {
                if (value != _rowSets)
                {
                    _rowSets = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private DesignerTableAlignment _aligment = DesignerTableAlignment.Left;
        public DesignerTableAlignment Alignment
        {
            get => _aligment;
            set
            {
                if (value != _aligment)
                {
                    _aligment = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isSelectable;
        public bool IsSelectable
        {
            get => _isSelectable;
            set
            {
                if (value != _isSelectable)
                {
                    _isSelectable = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Spacing
        private double _cellSpacing = 2;
        public double CellSpacing
        {
            get => _cellSpacing;
            set
            {
                if (value != _cellSpacing)
                {
                    _cellSpacing = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        private double _spaceLeft;
        public double SpaceLeft
        {
            get => _spaceLeft;
            set
            {
                if (value != _spaceLeft)
                {
                    _spaceLeft = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private double _spaceTop;
        public double SpaceTop
        {
            get => _spaceTop;
            set
            {
                if (value != _spaceTop)
                {
                    _spaceTop = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private double _spaceRight;
        public double SpaceRight
        {
            get => _spaceRight;
            set
            {
                if (value != _spaceRight)
                {
                    _spaceRight = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private double _spaceBottom;
        public double SpaceBottom
        {
            get => _spaceBottom;
            set
            {
                if (value != _spaceBottom)
                {
                    _spaceBottom = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods
        public void AddColumnDefition(TableColumnDefinition column)
        {
            column.PropertyChanged += OnColumnPropertyChanged;
            ColumnDefinitions.Add(column);
        }

        public void AddRowSet(string name, int rowCount)
        {
            var rowSet = new RowSet(name);
            rowSet.Parent = this;

            for (int i = 0; i < rowCount; i++)
            {
                rowSet.AddNewRow(null);
            }

            RowSets.Add(rowSet);
        }

        public static TableProperties Build(int columnCount, int headerRowCount, int bodyRowCount, int footerRowCount)
        {
            var tableProperties = new TableProperties();
            double defaultColumnWidthPercentage = 1.0 / columnCount;
            for (int i = 0; i < columnCount; i++)
            {
                var columnDefition = new TableColumnDefinition();
                columnDefition.Width = defaultColumnWidthPercentage;

                tableProperties.AddColumnDefition(columnDefition);
            }

            tableProperties.AddRowSet("RowSet Header", headerRowCount);
            tableProperties.AddRowSet("RowSet Body", bodyRowCount);
            tableProperties.AddRowSet("RowSet Footer", footerRowCount);

            return tableProperties;
        }

        private void OnColumnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TableColumnDefinition.Width))
            {
                double remainingWidth = 1;
                int index;
                for (index = 0; index < _columnDefinitions.Count; index++)
                {
                    remainingWidth -= _columnDefinitions[index].Width;
                    if (_columnDefinitions[index].Equals(sender))
                    {
                        break;
                    }
                }

                if (remainingWidth <= 0)
                {
                    _columnDefinitions[index].Width = Math.Abs(Math.Abs(remainingWidth) - Math.Abs(_columnDefinitions[index].Width));
                    remainingWidth = 0;
                }

                int remainingColumn = _columnDefinitions.Count - (index + 1);
                double desizedWidth = remainingWidth / remainingColumn;

                for (int i = index + 1; i < _columnDefinitions.Count; i++)
                {
                    _columnDefinitions[i].Width = desizedWidth;
                }
            }
        }
        #endregion

        #region Commands
        public ICommand DeleteCommand { get; set; }
        private void DeleteTable(object param)
        {
            OnDeleted?.Invoke(this, new EventArgs());
        }
        #endregion
    }

    public class TableColumnDefinition:  INotifyPropertyChanged
    {
        private double _width;
        public double Width
        {
            get => _width;
            set
            {
                if(value != _width)
                {
                    _width = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private double _minWidth = 1.0;
        public double MinWidth
        {
            get => _minWidth;
            set
            {
                if (value != _minWidth)
                {
                    _minWidth = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RowSet : ControlPropertiesViewModel
    {
        public event EventHandler OnAddNewRow;

        public TableProperties Parent { get; set; }

        public Guid Id { get; set; }

        public RowSet(string name)
        {
            Name = name;
            Rows = new ObservableCollection<Row>();
            Id = Guid.NewGuid();
            
            AddNewRowCommand = new RelayCommand(AddNewRow);
        }

        public ObservableCollection<Row> Rows { get; set; }

        public void AddRow(Row row)
        {
            row.Parent = this;
            Rows.Add(row);
        }

        #region Commands
        public ICommand AddNewRowCommand { get; set; }
        public void AddNewRow(object param)
        {
            var newRow = new Row($"Row {Rows.Count + 1}", Parent.ColumnDefinitions.Count);
            AddRow(newRow);

            OnAddNewRow?.Invoke(this, new EventArgs());
        }
        #endregion
    }

    public class Row: ControlPropertiesViewModel
    {
        public RowSet Parent { get; set; }

        public Guid Id { get; set; }

        public Row(string name, int columnCount)
        {
            Name = name;
            Cells = new ObservableCollection<Cell>();
            Id = Guid.NewGuid();

            AddCells(columnCount);
        }

        public ObservableCollection<Cell> Cells { get; set; }

        private void AddCells(int columnCount)
        {
            for (int i = 0; i < columnCount; i++)
            {
                var cell = new Cell($"Cell {i + 1}");
                Cells.Add(cell);
            }
        }
    }

    public class Cell: ControlPropertiesViewModel
    {
        public Guid Id { get; set; }

        public Cell(string name)
        {
            Name = name;
            Id = Guid.NewGuid();
        }

        private Brush _background = Brushes.White;
        public Brush Background
        {
            get => _background;
            set
            {
                if (value != _background)
                {
                    _background = value;
                    NotifyPropertyChanged();
                }
            }
        }

    }
}
