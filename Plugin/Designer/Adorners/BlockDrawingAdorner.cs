using System.Windows;
using System.Windows.Media;

namespace Designer.Adorners
{
    public class BlockDrawingAdorner: CanvasSelectionAdorner
    {
        public override double BorderThickness => 2.5;

        public override Brush BorderColor => Brushes.Blue;

        public override DoubleCollection BorderDashArray => null;

        public BlockDrawingAdorner(UIElement adornedElement)
            :base(adornedElement)
        {
        }
    }
}
