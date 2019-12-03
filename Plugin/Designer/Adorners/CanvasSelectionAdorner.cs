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

        public virtual double BorderThickness { get; } = 0.5;

        public virtual Brush BorderColor { get; } = Brushes.Black;

        public virtual Brush BackGround { get; } = Brushes.Transparent;

        public virtual DoubleCollection BorderDashArray { get; } = new DoubleCollection(new List<double>() { 3, 3 });

        public CanvasSelectionAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            SelectionBox = new Rectangle()
            {
                StrokeThickness = BorderThickness,
                Stroke = BorderColor,
                StrokeDashArray = BorderDashArray,
                Fill = BackGround
            };

            Focusable = false;
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

        public void Update(Rect rect)
        {
            Update(rect.Left, rect.Top, rect.Width, rect.Height);
        }
    }
}
