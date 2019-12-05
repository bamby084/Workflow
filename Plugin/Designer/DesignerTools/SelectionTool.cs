using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
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
    public enum SelectionType
    {
        Single,
        Multiple
    }

    public class SelectionTool: DesignerTool
    {
        private bool _isMouseDown;
        private Point _mouseDownPos;
        private bool _isDragging;

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
            
        }

        public static SelectionType SelectionType { get; set; } = SelectionType.Single;

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
                _isMouseDown = true;
                _mouseDownPos = e.GetPosition(Canvas);
                SelectionAdorner.Update(_mouseDownPos.X, _mouseDownPos.Y, 0, 0);

                Canvas.ClearSelectedItems();
                Canvas.CaptureMouse();
            }
            else
            {
                var designerItem = TreeHelperExtensions.FindVisualParent<DesignerItem>(hitObject);
                if (designerItem == null || !designerItem.IsSelectable)
                    return;

                if(IsCtrlKeyDown())
                {
                    SelectionType = SelectionType.Multiple;
                    designerItem.IsSelected = !designerItem.IsSelected;
                }
                else
                {
                    designerItem.IsSelected = true;
                }
            }
        }

        public override void HandleMouseMove(MouseEventArgs e)
        {
            if (!_isMouseDown || !Canvas.AllowMultipleSelection)
                return;

            Point mousePos = e.GetPosition(Canvas);
            double desizedLeft = Math.Max(0, mousePos.X);
            double desizedTop = Math.Max(0, mousePos.Y);

            double left = Math.Min(_mouseDownPos.X, desizedLeft);
            double top = Math.Min(_mouseDownPos.Y, desizedTop);
            double maxWidth = Canvas.ActualWidth - left;
            double maxHeight = Canvas.ActualHeight - top;
            double width = Math.Abs(_mouseDownPos.X - desizedLeft).Clamp(0, maxWidth);
            double height = Math.Abs(_mouseDownPos.Y - desizedTop).Clamp(0, maxHeight);

            SelectionAdorner.Update(left, top, width, height);
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
                SelectionType = SelectionType.Multiple;

                foreach (UIElement child in Canvas.Children)
                {
                    if (child is DesignerItem designerItem)
                    {
                        var childRect = new Rect(new Point(System.Windows.Controls.Canvas.GetLeft(designerItem),
                            System.Windows.Controls.Canvas.GetTop(designerItem)), designerItem.DesiredSize);

                        if (selectionRect.IntersectsWith(childRect))
                        {
                            designerItem.IsSelected = true;
                        }
                    }
                }
            }

            Keyboard.Focus(Canvas);
            _isMouseDown = false;
            _isDragging = false;
            SelectionType = SelectionType.Single;
        }

        public override void HandleKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                Canvas.RemoveSelectedItems();
            }
        }

        public override void ResetAdorner()
        {
            _selectionAdorner = null;
        }

        private bool IsCtrlKeyDown()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }
    }
}
