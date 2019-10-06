using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.Core.Converters;

namespace Designer.Adorners
{
    public abstract class SizeAdorner : Adorner
    {
        public double LineMargin => 10.0;
        public double TextPadding => 5.0;

        public VisualCollection Children { get; }

        public Brush LineBrush { get; }

        protected SizeAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            Children = new VisualCollection(this);
            LineBrush = new SolidColorBrush(SystemColors.HighlightColor);
        }

        protected override int VisualChildrenCount => Children.Count;

        protected override Visual GetVisualChild(int index) => Children[index];
    }

    public abstract class HorizontalSizeAdorner : SizeAdorner
    {
        private Line _adornerLine;
        private TextBlock _adornerTextBlock;

        protected HorizontalSizeAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            AddLine();
            AddText();
        }

        protected abstract double Top { get; }

        private void AddLine()
        {
            _adornerLine = new Line()
            {
                StrokeThickness = 1,
                SnapsToDevicePixels = true,
                Stroke = LineBrush
            };

            var lineWidthBinding = new Binding("Width");
            lineWidthBinding.Source = AdornedElement;

            _adornerLine.SetBinding(Line.X2Property, lineWidthBinding);
            Children.Add(_adornerLine);
        }

        private void AddText()
        {
            _adornerTextBlock = new TextBlock()
            {
                Background = Brushes.White,
                Padding = new Thickness(TextPadding, 0, TextPadding, 0)
            };

            var textBinding = new Binding("Width");
            textBinding.Source = AdornedElement;
            textBinding.Converter = new RoundedValueConverter() { Precision = 0 };

            _adornerTextBlock.SetBinding(TextBlock.TextProperty, textBinding);
            Children.Add(_adornerTextBlock);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var width = AdornedElement.DesiredSize.Width;
            var textWidth = _adornerTextBlock.DesiredSize.Width;
            var textHeight = _adornerTextBlock.DesiredSize.Height;

            _adornerLine.Arrange(new Rect(0, Top, width, this.DesiredSize.Height));
            _adornerTextBlock.Arrange(new Rect((width - textWidth) / 2, Top - textHeight / 2,
                textWidth, textHeight));

            return finalSize;
        }
    }

    public abstract class VerticalSizeAdorner : SizeAdorner
    {
        private Line _adornerLine;
        private TextBlock _adornerTextBlock;

        protected VerticalSizeAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            AddLine();
            AddText();
        }

        protected abstract double Left { get; }

        private void AddLine()
        {
            _adornerLine = new Line()
            {
                StrokeThickness = 1,
                SnapsToDevicePixels = true,
                Stroke = LineBrush
            };

            var lineHeightBinding = new Binding("Height");
            lineHeightBinding.Source = AdornedElement;

            _adornerLine.SetBinding(Line.Y2Property, lineHeightBinding);
            Children.Add(_adornerLine);
        }

        private void AddText()
        {
            _adornerTextBlock = new TextBlock()
            {
                Background = Brushes.White,
                Padding = new Thickness(TextPadding, 0, TextPadding, 0)
            };

            _adornerTextBlock.LayoutTransform = new RotateTransform(90);

            var textBinding = new Binding("Height");
            textBinding.Source = AdornedElement;
            textBinding.Converter = new RoundedValueConverter() { Precision = 0 };

            _adornerTextBlock.SetBinding(TextBlock.TextProperty, textBinding);
            Children.Add(_adornerTextBlock);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var height = AdornedElement.DesiredSize.Height;
            var textWidth = _adornerTextBlock.DesiredSize.Width;
            var textHeight = _adornerTextBlock.DesiredSize.Height;

            _adornerLine.Arrange(new Rect(Left, 0, this.DesiredSize.Width, height));
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
