using Designer.Adorners;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Designer.DesignerItems
{
    public class SelectedChangedEventArgs: EventArgs
    {
        public SelectedChangedEventArgs()
        {

        }

        public SelectedChangedEventArgs(bool isSelected)
        {
            IsSelected = isSelected;
        }

        public bool IsSelected { get; set; }
    }

    public delegate void SelectedChangedEventHandler(object sender, SelectedChangedEventArgs e);

    public abstract class DesignerItem: ContentControl, IControlPropertyProvider, ISelectable, IDisposable
    {
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected",
            typeof(bool),
            typeof(DesignerItem),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnPropertyChanged))
            );

        public static readonly DependencyProperty IsSelectableProperty = DependencyProperty.Register(
            "IsSelectable",
            typeof(bool),
            typeof(DesignerItem),
            new FrameworkPropertyMetadata(true)
            );

        public event SelectedChangedEventHandler SelectedChanged;
        public event EventHandler OnDisposed;

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

        public DesignerItem()
        {
        }

        public virtual ControlPropertiesViewModel Properties => null;

        public virtual void Dispose()
        {
            OnDisposed?.Invoke(this, new EventArgs());
        }

        private void ShowAdorner(bool value)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            if (adornerLayer == null)
                return;
            
            var adorners = adornerLayer.GetAdorners(this);

            if(value)
            {
                if (adorners == null || !adorners.Any(adorner => adorner.GetType() == typeof(DesignerItemAdorner)))
                {
                    adornerLayer.Add(new DesignerItemAdorner(this));
                }
            }
            else
            {
                if (adorners == null)
                    return;

                var designerItemAdorner = adorners.FirstOrDefault(adorner => adorner.GetType() == typeof(DesignerItemAdorner));
                if (designerItemAdorner != null)
                    adornerLayer.Remove(designerItemAdorner);
            }
        }

        private void NotifySelectedChanged(bool isSelected)
        {
            if (SelectedChanged != null)
                SelectedChanged(this, new SelectedChangedEventArgs(isSelected));
        }

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.Property.Name == "IsSelected")
            {
                var designerItem = sender as DesignerItem;
                bool isSelected = (bool)e.NewValue;

                designerItem.ShowAdorner(isSelected);
                designerItem.NotifySelectedChanged(isSelected);
            }
        }
    }
}
