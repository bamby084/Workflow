using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;
using Designer.Adorners;

namespace Designer
{
    public class DesignerCanvas : Canvas
    {
        private bool _isMouseDown;
        private Point _mouseDownPos;
        private bool _isDragging;

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

        private IList<UIElement> SelectedItems { get; }

        private CanvasSelectionAdorner SelectionAdorner { get; set; }

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

        public DesignerCanvas()
        {
            SelectedItems = new List<UIElement>();
            this.Loaded += OnLoaded;
            Focusable = true;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SelectionAdorner = new CanvasSelectionAdorner(this);
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);

            adornerLayer.Add(SelectionAdorner);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var hitTestObject = VisualTreeHelper.HitTest(this, e.GetPosition(this)).VisualHit as UIElement;

            //click on the canvas?
            if (hitTestObject.Equals(this))
            {
                ClearSelectedItems();
                _isMouseDown = true;
                _mouseDownPos = e.GetPosition(this);
                this.CaptureMouse();
                SelectionAdorner.Update(_mouseDownPos.X, _mouseDownPos.Y, 0, 0);
            }
            else
            {
                var isSelectable = GetIsSelectable(hitTestObject);
                if (!isSelectable)
                    return;

                if (IsCtrlKeyDown())
                {
                    bool isSelected = GetIsSelected(hitTestObject);
                    if (isSelected)
                        DeselectItem(hitTestObject);
                    else
                        SelectItem(hitTestObject);
                }
                else
                {
                    ClearSelectedItems();
                    SelectItem(hitTestObject);
                }
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();
            SelectionAdorner.Update(0, 0, 0, 0);

            if (_isDragging)
            {
                var mouseUpPos = e.GetPosition(this);
                var selectionRect = new Rect(_mouseDownPos, mouseUpPos);
                foreach (UIElement child in Children)
                {
                    var selectable = GetIsSelectable(child);
                    if (selectable)
                    {
                        var childRect = new Rect(new Point(GetLeft(child), GetTop(child)), child.DesiredSize);
                        if (selectionRect.IntersectsWith(childRect))
                        {
                            SelectItem(child);
                        }
                    }
                }

                Keyboard.Focus(this);
            }

            _isMouseDown = false;
            _isDragging = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!_isMouseDown)
                return;

            Point mousePos = e.GetPosition(this);
            SelectionAdorner.Update(Math.Min(_mouseDownPos.X, mousePos.X), Math.Min(_mouseDownPos.Y, mousePos.Y),
                Math.Abs(_mouseDownPos.X - mousePos.X), Math.Abs(_mouseDownPos.Y - mousePos.Y));
            _isDragging = true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                foreach (var item in SelectedItems)
                {
                    this.Children.Remove(item);
                }

                SelectedItems.Clear();
            }
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

        private void ClearSelectedItems()
        {
            foreach (var item in SelectedItems)
            {
                HideItemAdorners(item);
                SetIsSelected(item, false);
            }

            SelectedItems.Clear();
        }

        private void SelectItem(UIElement item)
        {
            SetIsSelected(item, true);
            SelectedItems.Add(item);
        }

        private void DeselectItem(UIElement item)
        {
            SetIsSelected(item, false);
            SelectedItems.Remove(item);
        }

        private bool IsCtrlKeyDown()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }
    }
}
