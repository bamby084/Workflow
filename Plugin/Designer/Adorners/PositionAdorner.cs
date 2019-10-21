using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using Designer.Converters;
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
            var leftBinding = new MultiBinding();
            leftBinding.Converter = new UnitToStringConverter();
            leftBinding.Bindings.Add(new Binding("(Canvas.Left)") {Source = AdornedElement});
            leftBinding.Bindings.Add(new Binding("UnitType") { Source = UnitOfMeasure.Current });
            left.SetBinding(Run.TextProperty, leftBinding);
            _positionTextBlock.Inlines.Add(left);

            _positionTextBlock.Inlines.Add(new Run() { Text = "," });

            var top = new Run();
            var topBinding = new MultiBinding();
            topBinding.Converter = new UnitToStringConverter();
            topBinding.Bindings.Add(new Binding("(Canvas.Top)") {Source = AdornedElement});
            topBinding.Bindings.Add(new Binding("UnitType") {Source = UnitOfMeasure.Current});
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
