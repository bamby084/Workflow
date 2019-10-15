using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Designer.Adorners;
using Designer.DesignerItems;
using Designer.ExtensionMethods;

namespace Designer.DesignerTools
{
    public class SelectionTool: DesignerTool
    {
        private bool _isMouseDown;
        private Point _mouseDownPos;
        private bool _isDragging;

        private IList<UIElement> SelectedItems { get; }

        private CanvasSelectionAdorner _selectionAdorner;
        private CanvasSelectionAdorner SelectionAdorner {
            get
            {
                if (_selectionAdorner == null)
                {
                    _selectionAdorner = new CanvasSelectionAdorner(Canvas);
                    var adornerLayer = AdornerLayer.GetAdornerLayer(Canvas);
                    adornerLayer.Add(_selectionAdorner);
                }

                return _selectionAdorner;
            }
        }

        public SelectionTool()
        {
            SelectedItems = new List<UIElement>();
        }

        public override ImageSource Image => new BitmapImage(
            new Uri("pack://application:,,,/Designer;component/Resources/Toolbar/cursor.png"));

        public override void HandleMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var hitObject = VisualTreeHelper.HitTest(Canvas, e.GetPosition(Canvas)).VisualHit as UIElement;
            if (hitObject == null)
                return;

            //click on the canvas?
            if (hitObject.Equals(Canvas))
            {
                ClearSelectedItems();
                _isMouseDown = true;
                _mouseDownPos = e.GetPosition(Canvas);
                Canvas.CaptureMouse();
                SelectionAdorner.Update(_mouseDownPos.X, _mouseDownPos.Y, 0, 0);
            }
            else
            {
                var designerItem = TreeHelperExtensions.FindVisualParent<DesignerItem>(hitObject);
                if (designerItem == null)
                    return;

                var isSelectable = DesignerCanvas.GetIsSelectable(designerItem);
                if (!isSelectable)
                    return;

                if (IsCtrlKeyDown())
                {
                    bool isSelected = DesignerCanvas.GetIsSelected(designerItem);
                    if (isSelected)
                        DeselectItem(designerItem);
                    else
                        SelectItem(designerItem);
                }
                else
                {
                    ClearSelectedItems();
                    SelectItem(designerItem);
                }
            }
        }

        public override void HandleMouseMove(MouseEventArgs e)
        {
            if (!_isMouseDown)
                return;

            Point mousePos = e.GetPosition(Canvas);
            SelectionAdorner.Update(Math.Min(_mouseDownPos.X, mousePos.X), Math.Min(_mouseDownPos.Y, mousePos.Y),
                Math.Abs(_mouseDownPos.X - mousePos.X), Math.Abs(_mouseDownPos.Y - mousePos.Y));
            _isDragging = true;
        }

        public override void HandleMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Canvas.ReleaseMouseCapture();
            SelectionAdorner.Update(0, 0, 0, 0);

            if (_isDragging)
            {
                var mouseUpPos = e.GetPosition(Canvas);
                var selectionRect = new Rect(_mouseDownPos, mouseUpPos);
                foreach (UIElement child in Canvas.Children)
                {
                    if (child is DesignerItem)
                    {
                        var childRect = new Rect(new Point(System.Windows.Controls.Canvas.GetLeft(child),
                            System.Windows.Controls.Canvas.GetTop(child)), child.DesiredSize);

                        if (selectionRect.IntersectsWith(childRect))
                        {
                            SelectItem(child);
                        }
                    }
                }

                Keyboard.Focus(Canvas);
            }

            _isMouseDown = false;
            _isDragging = false;
        }

        public override void HandleKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                foreach (var item in SelectedItems)
                {
                    Canvas.Children.Remove(item);
                }

                SelectedItems.Clear();
            }
        }

        private void ClearSelectedItems()
        {
            foreach (var item in SelectedItems)
            {
                DesignerCanvas.SetIsSelected(item, false);
            }

            SelectedItems.Clear();
        }

        private void SelectItem(UIElement item)
        {
            DesignerCanvas.SetIsSelected(item, true);
            SelectedItems.Add(item);
        }

        private void DeselectItem(UIElement item)
        {
            DesignerCanvas.SetIsSelected(item, false);
            SelectedItems.Remove(item);
        }

        private bool IsCtrlKeyDown()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }
    }
}
