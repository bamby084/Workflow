using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Designer.Adorners
{
    public class DesignerItemAdorner : Adorner
    {
        private VisualCollection _children;
        
        public DesignerItemAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _children = new VisualCollection(this);
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

            _children.Add(border);
        }

        private void BuidResizeMarkers()
        {
            _children.Add(new TopLeftResizeMarker(AdornedElement));
            _children.Add(new TopRightResizeMarker(AdornedElement));
            _children.Add(new BottomLeftResizeMarker(AdornedElement));
            _children.Add(new BottomRightResizeMarker(AdornedElement));
            _children.Add(new CenterTopResizeMarker(AdornedElement));
            _children.Add(new CenterLeftMarker(AdornedElement));
            _children.Add(new CenterRightMarker(AdornedElement));
            _children.Add(new CenterBottomResizeMarker(AdornedElement));
            _children.Add(new CenterMarker(AdornedElement));
        }

        protected override int VisualChildrenCount => _children.Count;

        protected override Visual GetVisualChild(int index) => _children[index];

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var control in _children)
            {
                if (control is ResizeMarker marker)
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
