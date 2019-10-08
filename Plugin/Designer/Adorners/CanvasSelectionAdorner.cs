using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;


namespace Designer.Adorners
{
    public class CanvasSelectionAdorner : BaseAdorner
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public Size Size { get; set; }

        private Rectangle SelectionBox { get; }

        public CanvasSelectionAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            SelectionBox = new Rectangle()
            {
                StrokeThickness = 0.5,
                Stroke = Brushes.Black,
                StrokeDashArray = new DoubleCollection(new List<double>() { 3, 3 })
            };

            Children.Add(SelectionBox);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            SelectionBox.Arrange(new Rect(new Point(Left, Top), Size));
            return finalSize;
        }

        public void Update(double left, double top, double width, double height)
        {
            Left = left;
            Top = top;
            Size = new Size(width, height);

            this.InvalidateArrange();
        }
    }
}
