using Designer.Adorners;
using Designer.DesignerItems;
using Designer.ExtensionMethods;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Designer.DesignerTools
{
    public class DrawingBlockTool : DesignerTool
    {
        private Point _mouseDownPos;
        private bool _isDragging;
        private bool _cancel;
        private Rect _desizedRect;

        private DrawingBlockAdorner _drawingAdorner;
        private DrawingBlockAdorner DrawingAdorner
        {
            get
            {
                if (_drawingAdorner == null)
                {
                    _drawingAdorner = new DrawingBlockAdorner(Canvas);
                    var adornerLayer = AdornerLayer.GetAdornerLayer(Canvas);
                    adornerLayer.Add(_drawingAdorner);
                }

                return _drawingAdorner;
            }
        }

        public DrawingBlockTool()
        {

        }

        public override ImageSource Image => new BitmapImage(
            new Uri("pack://application:,,,/Designer;component/Resources/Toolbar/block.png"));

        public override Cursor Cursor => Cursors.Hand;

        public override void HandleMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _cancel = false;
            _mouseDownPos = e.GetPosition(Canvas);
            Canvas.CaptureMouse();
            DrawingAdorner.Update(_mouseDownPos.X, _mouseDownPos.Y, 0, 0);
        }

        public override void HandleMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Canvas.ReleaseMouseCapture();
            DrawingAdorner.Update(0, 0, 0, 0);

            if (_isDragging && _desizedRect.Width != 0 && _desizedRect.Height != 0)
            {
                var block = new DesignerBlock();
                block.Properties.Left = _desizedRect.Left;
                block.Properties.Top = _desizedRect.Top;
                block.Properties.Width = _desizedRect.Width;
                block.Properties.Height = _desizedRect.Height;

                Canvas.AddItem(block);
                Canvas.NotifyItemAdded(block);
            }

            _isDragging = false;
            _cancel = true;
        }

        public override void HandleMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || _cancel)
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

            _desizedRect = new Rect(left, top, width, height);
            DrawingAdorner.Update(_desizedRect);
            _isDragging = true;
        }

        public override void HandleKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape && _isDragging)
            {
                _isDragging = false;
                _cancel = true;
                DrawingAdorner.Update(0, 0, 0, 0);
                Canvas.ReleaseMouseCapture();
            }
        }

        public override void ResetAdorner()
        {
            _drawingAdorner = null;
        }
    }
}
