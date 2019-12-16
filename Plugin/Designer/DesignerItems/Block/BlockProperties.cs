
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Designer.DesignerItems
{
    [DisplayName("Block Properties")]
    public class BlockProperties : ControlPropertiesViewModel, ISelectable
    {
        public event EventHandler OnDelete;
        public event EventHandler OnInsertExistingTable;

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

        private bool _isSelectable = true;
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

        private ObservableCollection<IBlockChild> _children;
        public ObservableCollection<IBlockChild> Children
        {
            get => _children;
            set
            {
                if (value != _children)
                {
                    _children = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public BlockProperties()
        {
            Children = new ObservableCollection<IBlockChild>();
            DeleteCommand = new RelayCommand(Delete);
            InsertExistingTableCommand = new RelayCommand(InsertExistingTable);
        }

        public ICommand DeleteCommand { get; set; }
        public void Delete(object param)
        {
            OnDelete?.Invoke(this, new EventArgs());
        }

        public ICommand InsertExistingTableCommand { get; set; }
        public void InsertExistingTable(object param)
        {
            if (param is TableProperties)
            {
                OnInsertExistingTable?.Invoke(param, new EventArgs());
            }
        }
    }
}
