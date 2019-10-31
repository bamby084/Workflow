using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Designer.DesignerTools;
using Designer.DesignerItems;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;

namespace Designer
{
    public class ItemsChangedEventArgs: EventArgs
    {
        public IList<DesignerItem> Items { get; set; }
    }

    public class ItemAddedEventArgs: EventArgs
    {
        public DesignerItem Item { get; set; }
    }

    public delegate void ItemsChangedEventHandler(object sender, ItemsChangedEventArgs e);
    public delegate void ItemAddedEventHandler(object sender, ItemAddedEventArgs e);

    public class DesignerCanvas : Canvas
    {
        public static readonly DependencyProperty ActiveToolProperty = DependencyProperty.Register(
            "ActiveTool",
            typeof(DesignerTool),
            typeof(DesignerCanvas),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnActiveToolChanged)));

        public static readonly DependencyProperty AllowMultipleSelectionProperty = DependencyProperty.Register(
            "AllowMultipleSelection",
            typeof(bool),
            typeof(DesignerCanvas),
            new FrameworkPropertyMetadata(true)
            );

        public ObservableCollection<DesignerItem> SelectedItems { get; private set; }

        public event ItemsChangedEventHandler SelectedItemsChanged;
        public event ItemAddedEventHandler ItemAdded;
        public event ItemsChangedEventHandler ItemsDeleted;

        public static void OnActiveToolChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        public DesignerCanvas()
        {
            Focusable = true;
            SelectedItems = new ObservableCollection<DesignerItem>();
            SelectedItems.CollectionChanged += OnSelectedItemsChanged;
        }

        private void OnSelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifySelectedItemsChanged(SelectedItems);
        }

        public DesignerTool ActiveTool {
            get => (DesignerTool)GetValue(ActiveToolProperty);
            set => SetValue(ActiveToolProperty, value);
        }

        public bool AllowMultipleSelection
        {
            get => (bool)GetValue(AllowMultipleSelectionProperty);
            set => SetValue(AllowMultipleSelectionProperty, value);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (ActiveTool != null)
            {
                ActiveTool.HandleMouseLeftButtonDown(e);
                Keyboard.Focus(this);
            }

            e.Handled = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (ActiveTool != null)
            {
                ActiveTool.HandleMouseLeftButtonUp(e);
            }

            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (ActiveTool != null)
            {
                ActiveTool.HandleMouseMove(e);
            }

            e.Handled = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (ActiveTool != null)
            {
                ActiveTool.HandleKeyDown(e);
            }
        }

        public void NotifySelectedItemsChanged(IList<DesignerItem> items)
        {
            SelectedItemsChanged?.Invoke(this, new ItemsChangedEventArgs() {Items = items});
        }

        public void NotifyItemAdded(DesignerItem item)
        {
            ItemAdded?.Invoke(this, new ItemAddedEventArgs() { Item = item });
        }

        public void NotifyItemsDeleted(IList<DesignerItem> items)
        {
            ItemsDeleted?.Invoke(this, new ItemsChangedEventArgs() { Items = items });
        }

        public void AddItem(DesignerItem item)
        {
            item.SelectedChanged += OnItemSelectedChanged;
            item.OnDisposed += OnItemDisposed;
            Children.Add(item);
        }

        private void OnItemDisposed(object sender, EventArgs e)
        {
            var designerItem = sender as DesignerItem;
            if (designerItem == null)
                return;

            designerItem.IsSelected = false;
            RemoveItem(designerItem);
        }

        public void RemoveItem(DesignerItem item)
        {
            item.SelectedChanged -= OnItemSelectedChanged;
            item.OnDisposed -= OnItemDisposed;
            Children.Remove(item);
        }

        public void ClearSelectedItems()
        {
            List<DesignerItem> items = SelectedItems.ToList();
            foreach(var item in items)
            {
                item.IsSelected = false;        
            }
        }

        public void RemoveSelectedItems()
        {
            var items = SelectedItems.ToList();
            foreach (var item in items)
                item.Dispose();

            NotifyItemsDeleted(items);
        }

        private void OnItemSelectedChanged(object sender, SelectedChangedEventArgs e)
        {
            var designerItem = sender as DesignerItem;
            if(e.IsSelected)
            {
                if(!AllowMultipleSelection || SelectionTool.SelectionType == SelectionType.Single)
                {
                    ClearSelectedItems();
                }

                SelectedItems.Add(designerItem);
            }
            else
            {
                SelectedItems.Remove(designerItem);
            }
        }
    }
}
