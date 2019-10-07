using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Shapes;
using Designer.Adorners;

namespace Designer
{
    public class DesignerCanvas: Canvas
    {
        private UIElement _selectedItem;
        private Rectangle _selectionBox;
        private bool _isMouseDown;
        private Point _mouseDownPos;

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
            _selectionBox = new Rectangle();
            _selectionBox.StrokeThickness = 0.5;
            _selectionBox.Visibility = Visibility.Hidden;
            _selectionBox.Stroke = Brushes.Black;
            _selectionBox.StrokeDashArray = new DoubleCollection(new List<double>() {3, 3});

            Children.Add(_selectionBox);
        }

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
                Canvas.SetLeft(_selectionBox, _mouseDownPos.X);
                Canvas.SetTop(_selectionBox, _mouseDownPos.Y);
                _selectionBox.Width = 0;
                _selectionBox.Height = 0;
                _selectionBox.Visibility = Visibility.Visible;

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
            _selectionBox.Visibility = Visibility.Hidden;

            var mouseUpPos = e.GetPosition(this);

            var selectionRect = new Rect(_mouseDownPos, mouseUpPos);
            foreach (UIElement child in Children)
            {
                var selectable = GetIsSelectable(child);
                if (selectable)
                {
                    var childRect = new Rect(new Point(GetLeft(child), GetTop(child)), child.DesiredSize);
                    if (selectionRect.Contains(childRect))
                    {
                        SetIsSelected(child,  true);
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!_isMouseDown)
                return;

            Point mousePos = e.GetPosition(this);
            SetLeft(_selectionBox, Math.Min(_mouseDownPos.X, mousePos.X));
            SetTop(_selectionBox, Math.Min(_mouseDownPos.Y, mousePos.Y));
            _selectionBox.Width = Math.Abs(_mouseDownPos.X - mousePos.X);
            _selectionBox.Height = Math.Abs(_mouseDownPos.Y - mousePos.Y);
        }

        private static void SelectElement(UIElement element)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);
            var adorners = adornerLayer.GetAdorners(element);

            if(adorners == null || adorners.All(adorner => adorner.GetType() != typeof(DesignerItemAdorner)))
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
