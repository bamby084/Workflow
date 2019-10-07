using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;

namespace Designer
{
    public class DesignerCanvas: Canvas
    {
        private UIElement _selectedItem;

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


        public static void SetIsSelected(UIElement element, bool value)
        {
            element.SetValue(IsSelectedProperty, value);
            if (value)
                SelectElement(element);
            else
                DeselectElement(element);
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

        public DesignerCanvas()
        {
        }

        private bool _isMouseDown;
        private Point _mouseDownPos;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var hitTestObject = VisualTreeHelper.HitTest(this, e.GetPosition(this)).VisualHit as UIElement;

            if(hitTestObject == this)
            {
                if (_selectedItem != null)
                {
                    DeselectElement(_selectedItem);
                    _selectedItem = null;
                }

                _isMouseDown = true;
                _mouseDownPos = e.GetPosition(this);
                this.CaptureMouse();

                return;
            }

            var isSelectable = GetIsSelectable(hitTestObject);
            if (!isSelectable)
                return;

            if (_selectedItem != null)
                SetIsSelected(_selectedItem, false);

            SetIsSelected(hitTestObject, true);
            _selectedItem = hitTestObject;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            this.ReleaseMouseCapture();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if(_isMouseDown)
            {

            }
        }

        private static void SelectElement(UIElement element)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            var adorners = adornerLayer.GetAdorners(element);

            if(adorners == null || !adorners.Any(adorner => adorner.GetType() == typeof(DesignerItemAdorner)))
            {
                adornerLayer.Add(new DesignerItemAdorner(element));
            }
        }

        private static void DeselectElement(UIElement element)
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
