
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Designer.DesignerItems
{
    public class TableProperties: ControlPropertiesViewModel
    {
        public TableProperties()
        {
            _columnDefinitions = new ObservableCollection<TableColumnDefinition>();
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

        public void AddColumnDefition(TableColumnDefinition column)
        {
            column.PropertyChanged += OnColumnPropertyChanged;
            ColumnDefinitions.Add(column);
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
}
