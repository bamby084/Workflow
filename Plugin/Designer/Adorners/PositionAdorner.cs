using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core.Converters;

namespace Designer.Adorners
{
    public class PositionAdorner: BaseAdorner
    {
        private TextBlock _positionTextBlock;

        public PositionAdorner(UIElement adornedElement) 
            : base(adornedElement)
        {
            Build();
        }

        private void Build()
        {
            _positionTextBlock = new TextBlock();
            _positionTextBlock.Background = Brushes.White;
            _positionTextBlock.Inlines.Add(new Run() {Text = "("});

            var left = new Run();
            var leftBinding = new Binding("(Canvas.Left)");
            leftBinding.Source = AdornedElement;
            leftBinding.Converter = new RoundedValueConverter();
            left.SetBinding(Run.TextProperty, leftBinding);
            _positionTextBlock.Inlines.Add(left);

            _positionTextBlock.Inlines.Add(new Run() { Text = "," });

            var top = new Run();
            var topBinding = new Binding("(Canvas.Top)");
            topBinding.Source = AdornedElement;
            topBinding.Converter = new RoundedValueConverter();
            top.SetBinding(Run.TextProperty, topBinding);
            _positionTextBlock.Inlines.Add(top);

            _positionTextBlock.Inlines.Add(new Run() {Text = ")"});            
            Children.Add(_positionTextBlock);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var width = _positionTextBlock.DesiredSize.Width;
            var height = _positionTextBlock.DesiredSize.Height;

            _positionTextBlock.Arrange(new Rect((AdornedElement.DesiredSize.Width - width) / 2,
                AdornedElement.DesiredSize.Height / 2 + 10,
                width, height));

            return finalSize;
        }
    }
}
