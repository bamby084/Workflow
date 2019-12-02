using Designer.Adorners;
using Designer.DesignerItems;
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
        private bool _isMouseDown;
        private Point _mouseDownPos;
        private bool _isDragging;
        private bool _escape;

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
            _isMouseDown = true;
            _mouseDownPos = e.GetPosition(Canvas);
            Canvas.CaptureMouse();

            DrawingAdorner.Update(_mouseDownPos.X, _mouseDownPos.Y, 0, 0);
        }

        public override void HandleMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            Canvas.ReleaseMouseCapture();
            DrawingAdorner.Update(0, 0, 0, 0);

            if (_isDragging)
            {
                var mouseUpPos = e.GetPosition(Canvas);
                var left = Math.Min(_mouseDownPos.X, mouseUpPos.X);
                var top = Math.Min(_mouseDownPos.Y, mouseUpPos.Y);

                var block = new DesignerBlock();
                block.Properties.Width = Math.Abs(_mouseDownPos.X - mouseUpPos.X);
                block.Properties.Height = Math.Abs(_mouseDownPos.Y - mouseUpPos.Y);
                block.Properties.Left = Math.Max(0, left);
                block.Properties.Top = Math.Max(0, top);

                Canvas.AddItem(block);
                Canvas.NotifyItemAdded(block);
            }

            _isDragging = false;
        }

        public override void HandleMouseMove(MouseEventArgs e)
        {
            if (!_isMouseDown)
                return;

            Point mousePos = e.GetPosition(Canvas);
            double left = Math.Min(_mouseDownPos.X, mousePos.X);
            left = Math.Max(0, left);
            double top = Math.Min(_mouseDownPos.Y, mousePos.Y);
            top = Math.Max(0, top);

            DrawingAdorner.Update(left, top,
                Math.Abs(_mouseDownPos.X - mousePos.X), Math.Abs(_mouseDownPos.Y - mousePos.Y));
            _isDragging = true;
        }

        public override void HandleKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape && _isDragging)
            {
                DrawingAdorner.Update(0, 0, 0, 0);
                _isMouseDown = false;
                _isDragging = false;
                Canvas.ReleaseMouseCapture();
            }
        }

        public override void ResetAdorner()
        {
            _drawingAdorner = null;
        }
    }
}
