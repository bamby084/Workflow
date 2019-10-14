using System.Windows;
using System.Windows.Media;

namespace Designer.Adorners
{
    public class DrawingBlockAdorner: CanvasSelectionAdorner
    {
        public override double BorderThickness => 2.5;

        public override Brush BorderColor => Brushes.Blue;

        public override DoubleCollection BorderDashArray => null;

        public DrawingBlockAdorner(UIElement adornedElement)
            :base(adornedElement)
        {
        }
    }
}
