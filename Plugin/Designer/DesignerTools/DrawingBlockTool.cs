using Designer.Adorners;
using Designer.DesignerItems;
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Designer.DesignerTools
{
    public class DrawingBlockTool : DesignerTool
    {
        private bool _isMouseDown;
        private Point _mouseDownPos;
        private bool _isDragging;

        private BlockDrawingAdorner _drawingAdorner;
        private BlockDrawingAdorner DrawingAdorner {
            get
            {
                if(_drawingAdorner == null)
                {
                    _drawingAdorner = new BlockDrawingAdorner(Canvas);
                    var adornerLayer = AdornerLayer.GetAdornerLayer(Canvas);
                    adornerLayer.Add(_drawingAdorner);
                }

                return _drawingAdorner;
            }
        }

        public DrawingBlockTool(DesignerCanvas canvas)
            :base(canvas)
        {

        }

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
                block.Width = Math.Abs(_mouseDownPos.X - mouseUpPos.X);
                block.Height = Math.Abs(_mouseDownPos.Y - mouseUpPos.Y);
               
                block.SetValue(System.Windows.Controls.Canvas.LeftProperty, left);
                block.SetValue(System.Windows.Controls.Canvas.TopProperty, top);
                block.SetValue(DesignerCanvas.IsSelectableProperty, true);

                Canvas.Children.Add(block);
            }

            _isDragging = false;
        }

        public override void HandleMouseMove(MouseEventArgs e)
        {
            if (!_isMouseDown)
                return;

            Point mousePos = e.GetPosition(Canvas);
            DrawingAdorner.Update(Math.Min(_mouseDownPos.X, mousePos.X), Math.Min(_mouseDownPos.Y, mousePos.Y),
                Math.Abs(_mouseDownPos.X - mousePos.X), Math.Abs(_mouseDownPos.Y - mousePos.Y));
            _isDragging = true;
        }
    }
}
