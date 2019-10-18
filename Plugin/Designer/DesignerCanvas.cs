using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Linq;
using Designer.Adorners;
using Designer.DesignerTools;
using Designer.DesignerItems;
using System.Collections.Generic;

namespace Designer
{
    public struct DesignerItemsSelectedEventArgs
    {
        public IList<DesignerItem> SelectedItems { get; set; }
    }

    public delegate void DesignerItemsSelectedEventHandler(object sender, DesignerItemsSelectedEventArgs e);

    public class DesignerCanvas : Canvas
    {
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.RegisterAttached(
            "IsSelected",
            typeof(bool),
            typeof(DesignerCanvas),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IsSelectableProperty = DependencyProperty.RegisterAttached(
           "IsSelectable",
           typeof(bool),
           typeof(DesignerCanvas),
           new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ActiveToolProperty = DependencyProperty.Register(
            "ActiveTool",
            typeof(DesignerTool),
            typeof(DesignerCanvas),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnActiveToolChanged)));

        public DesignerItemsSelectedEventHandler ItemsSelected;

        public static void SetIsSelected(UIElement element, bool value)
        {
            element.SetValue(IsSelectedProperty, value);
            if (value)
                ShowItemAdorners(element);
            else
                HideItemAdorners(element);
        }

        public static bool GetIsSelected(UIElement element)
        {
            return (bool)element.GetValue(IsSelectedProperty);
        }

        public static void SetIsSelectable(UIElement element, bool value)
        {
            element.SetValue(IsSelectableProperty, value);
        }

        public static bool GetIsSelectable(UIElement element)
        {
            return (bool)element.GetValue(IsSelectableProperty);
        }

        public static void OnActiveToolChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var tool = e.NewValue as DesignerTool;
            if (tool != null)
                tool.Canvas = (DesignerCanvas)sender;
        }

        public DesignerCanvas()
        {
            Focusable = true;
        }

        public DesignerTool ActiveTool {
            get => (DesignerTool)GetValue(ActiveToolProperty);
            set => SetValue(ActiveToolProperty, value);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (ActiveTool != null)
            {
                ActiveTool.HandleMouseLeftButtonDown(e);
                e.Handled = true;
                Keyboard.Focus(this);
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (ActiveTool != null)
            {
                ActiveTool.HandleMouseLeftButtonUp(e);
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (ActiveTool != null)
            {
                ActiveTool.HandleMouseMove(e);
                e.Handled = true;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (ActiveTool != null)
                ActiveTool.HandleKeyDown(e);
        }

        private static void ShowItemAdorners(UIElement element)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            var adorners = adornerLayer.GetAdorners(element);

            if (adorners == null || adorners.All(adorner => adorner.GetType() != typeof(DesignerItemAdorner)))
            {
                adornerLayer.Add(new DesignerItemAdorner(element));
            }
        }

        private static void HideItemAdorners(UIElement element)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            var adorners = adornerLayer.GetAdorners(element);

            if (adorners == null)
                return;

            var designerItemAdorner = adorners.FirstOrDefault(adorner => adorner.GetType() == typeof(DesignerItemAdorner));
            if (designerItemAdorner != null)
                adornerLayer.Remove(designerItemAdorner);
        }
    }
}
