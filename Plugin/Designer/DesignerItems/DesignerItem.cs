using Designer.Adorners;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

    public abstract class DesignerItem: ContentControl, ISelectable, IDisposable, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected",
            typeof(bool),
            typeof(DesignerItem),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged))
            );

        public static readonly DependencyProperty IsSelectableProperty = DependencyProperty.Register(
            "IsSelectable",
            typeof(bool),
            typeof(DesignerItem),
            new FrameworkPropertyMetadata(true)
            );

        public event EventHandler OnDisposed;
        public event SelectedChangedEventHandler SelectedChanged;
        public event PropertyChangedEventHandler PropertyChanged;

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

        private static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var designerItem = sender as DesignerItem;
            bool isSelected = (bool)e.NewValue;

            designerItem.ShowAdorner(isSelected);
            designerItem.NotifySelectedChanged(isSelected);
        }


        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
