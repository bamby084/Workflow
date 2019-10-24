using Designer.Adorners;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Designer.DesignerItems
{
    public abstract class DesignerItem: ContentControl, IControlPropertyProvider
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

        private void SetSelected(bool isSelected)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            var adorners = adornerLayer.GetAdorners(this);

            if(isSelected)
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

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.Property.Name == "IsSelected")
            {
                ((DesignerItem)sender).SetSelected((bool)e.NewValue);
            }
        }
    }
}
