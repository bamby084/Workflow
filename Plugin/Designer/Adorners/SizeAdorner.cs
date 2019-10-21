using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Designer.Converters;
using Xceed.Wpf.Toolkit.Core.Converters;

namespace Designer.Adorners
{
    public abstract class SizeAdorner : BaseAdorner
    {
        public double LineMargin => 10.0;
        public double TextPadding => 5.0;
        public double LineThickness => 1;

        public Brush LineBrush { get; } = new SolidColorBrush(SystemColors.HighlightColor);

        protected SizeAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }
    }

    public abstract class HorizontalSizeAdorner : SizeAdorner
    {
        private Border _adornerLine;
        private Border _adornerLine2;
        private TextBlock _adornerTextBlock;
        private const double LineHeight = 1.0;
        private const double Line2Height = 10;

        protected HorizontalSizeAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            AddLines();
            AddText();
        }

        protected abstract double Top { get; }

        private void AddLines()
        {
            _adornerLine = new Border()
            {
                BorderBrush = LineBrush,
                BorderThickness = new Thickness(1),
                Height = LineHeight
            };

            var lineWidthBinding = new Binding("Width");
            lineWidthBinding.Source = AdornedElement;
            _adornerLine.SetBinding(Rectangle.WidthProperty, lineWidthBinding);
            Children.Add(_adornerLine);

            _adornerLine2 = new Border()
            {
                BorderBrush = LineBrush,
                BorderThickness = new Thickness(1, 0, 1, 0),
                Height = Line2Height
            };

            _adornerLine2.SetBinding(WidthProperty, lineWidthBinding);
            Children.Add(_adornerLine2);
        }

        private void AddText()
        {
            _adornerTextBlock = new TextBlock()
            {
                Background = Brushes.White,
                Padding = new Thickness(TextPadding, 0, TextPadding, 0)
            };

            var textBinding = new MultiBinding();
            textBinding.Converter = new UnitToStringConverter();
            textBinding.Bindings.Add(new Binding("Width") {Source = AdornedElement});
            textBinding.Bindings.Add(new Binding("UnitType") {Source = UnitOfMeasure.Current});

            _adornerTextBlock.SetBinding(TextBlock.TextProperty, textBinding);
            Children.Add(_adornerTextBlock);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var width = AdornedElement.DesiredSize.Width;
            var textWidth = _adornerTextBlock.DesiredSize.Width;
            var textHeight = _adornerTextBlock.DesiredSize.Height;

            _adornerLine.Arrange(new Rect(0, Top, width, LineHeight));
            _adornerLine2.Arrange(new Rect(0, Top - Line2Height / 2, width, Line2Height));
            _adornerTextBlock.Arrange(new Rect((width - textWidth) / 2, Top - textHeight / 2,
                textWidth, textHeight));

            return finalSize;
        }
    }

    public abstract class VerticalSizeAdorner : SizeAdorner
    {
        private Border _adornerLine;
        private Border _adornerLine2;
        private TextBlock _adornerTextBlock;
        private const double LineWidth = 1.0;
        private const double Line2Width = 10.0;

        protected VerticalSizeAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            AddLines();
            AddText();
        }

        protected abstract double Left { get; }

        private void AddLines()
        {
            _adornerLine = new Border()
            {
                BorderThickness = new Thickness(1),
                BorderBrush = LineBrush,
                Width = LineWidth
            };

            var lineHeightBinding = new Binding("Height");
            lineHeightBinding.Source = AdornedElement;

            _adornerLine.SetBinding(HeightProperty, lineHeightBinding);
            Children.Add(_adornerLine);

            _adornerLine2 = new Border()
            {
                BorderBrush = LineBrush,
                BorderThickness = new Thickness(0, 1, 0, 1),
                Width = Line2Width
            };

            _adornerLine2.SetBinding(HeightProperty, lineHeightBinding);
            Children.Add(_adornerLine2);
            
        }

        private void AddText()
        {
            _adornerTextBlock = new TextBlock()
            {
                Background = Brushes.White,
                Padding = new Thickness(TextPadding, 0, TextPadding, 0)
            };

            _adornerTextBlock.LayoutTransform = new RotateTransform(90);

            var textBinding = new MultiBinding();
            textBinding.Converter = new UnitToStringConverter();
            textBinding.Bindings.Add(new Binding("Height") { Source = AdornedElement });
            textBinding.Bindings.Add(new Binding("UnitType") { Source = UnitOfMeasure.Current });

            _adornerTextBlock.SetBinding(TextBlock.TextProperty, textBinding);
            Children.Add(_adornerTextBlock);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var height = AdornedElement.DesiredSize.Height;
            var textWidth = _adornerTextBlock.DesiredSize.Width;
            var textHeight = _adornerTextBlock.DesiredSize.Height;

            _adornerLine.Arrange(new Rect(Left, 0, LineWidth, height));
            _adornerLine2.Arrange(new Rect(Left - Line2Width / 2, 0, Line2Width, height));
            _adornerTextBlock.Arrange(new Rect(Left - textWidth / 2, (height - textHeight) / 2, textWidth, textHeight));

            return finalSize;
        }
    }

    public class BottomSizeAdorner : HorizontalSizeAdorner
    {
        public BottomSizeAdorner(UIElement adornedElement)
            : base(adornedElement)
        {

        }

        protected override double Top => AdornedElement.DesiredSize.Height + LineMargin;
    }

    public class TopSizeAdorner : HorizontalSizeAdorner
    {
        public TopSizeAdorner(UIElement adornedElement)
            : base(adornedElement)
        {

        }

        protected override double Top => -LineMargin;
    }

    public class LeftSizeAdorner : VerticalSizeAdorner
    {
        public LeftSizeAdorner(UIElement adornedElement)
            : base(adornedElement)
        {

        }

        protected override double Left => -LineMargin;
    }

    public class RightSizeAdorner : VerticalSizeAdorner
    {
        public RightSizeAdorner(UIElement adornedElement)
            : base(adornedElement)
        {

        }

        protected override double Left => AdornedElement.DesiredSize.Width + LineMargin;
    }
}
