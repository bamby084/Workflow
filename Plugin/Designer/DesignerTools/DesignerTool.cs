using System.Windows.Input;
using System.Windows.Media;

namespace Designer.DesignerTools
{
    public abstract class DesignerTool
    {
        public DesignerCanvas Canvas { get; }

        public virtual Cursor Cursor
        {
            get => Cursors.Arrow;
        }

        public virtual  ImageSource Image { get; }

        public bool IsSelected { get; set; }

        public DesignerTool(DesignerCanvas canvas)
        {
            Canvas = canvas;
            Canvas.Cursor = Cursor;
        }

        public virtual void HandleMouseLeftButtonDown(MouseButtonEventArgs e)
        {
        }

        public virtual void HandleMouseLeftButtonUp(MouseButtonEventArgs e)
        {
        }

        public virtual void HandleMouseMove(MouseEventArgs e)
        {

        }

        public virtual void HandleKeyDown(KeyEventArgs e)
        {

        }
    }
}
