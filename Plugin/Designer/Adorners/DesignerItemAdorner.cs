using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Designer.Adorners
{
    public class DesignerItemAdorner : BaseAdorner
    {
        public DesignerItemAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            BuidAdorner();
        }

        private void BuidAdorner()
        {
            BuildBorder();
            BuidResizeMarkers();
        }

        private void BuildBorder()
        {
            var border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(SystemColors.HighlightColor)
            };

            Children.Add(border);
        }

        private void BuidResizeMarkers()
        {
            Children.Add(new TopLeftResizeMarker(AdornedElement));
            Children.Add(new TopRightResizeMarker(AdornedElement));
            Children.Add(new BottomLeftResizeMarker(AdornedElement));
            Children.Add(new BottomRightResizeMarker(AdornedElement));
            Children.Add(new CenterTopResizeMarker(AdornedElement));
            Children.Add(new CenterLeftMarker(AdornedElement));
            Children.Add(new CenterRightMarker(AdornedElement));
            Children.Add(new CenterBottomResizeMarker(AdornedElement));
            Children.Add(new MoveMarker(AdornedElement));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var control in Children)
            {
                if (control is Marker marker)
                {
                    marker.Arrange(marker.GetSize());
                }
                else if (control is Border border)
                {
                    border.Arrange(new Rect(0, 0, AdornedElement.DesiredSize.Width, AdornedElement.DesiredSize.Height));
                }
            }

            return finalSize;
        }
    }
}
