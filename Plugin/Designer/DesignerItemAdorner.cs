using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Designer
{
    public class DesignerItemAdorner: Adorner
    {
        private VisualCollection _children;
        private Border _border;

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
            _border = new Border
            {
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(SystemColors.HighlightColor)
            };

            _children.Add(_border);
        }

        private void BuidResizeMarkers()
        {
            _children.Add(new TopLeftResizeMarker(AdornedElement));
        }

        protected override int VisualChildrenCount => _children.Count;

        protected override Visual GetVisualChild(int index) => _children[index];

        protected override Size ArrangeOverride(Size finalSize)
        {
            _border.Arrange(new Rect(0, 0, AdornedElement.DesiredSize.Width, AdornedElement.DesiredSize.Height));

            foreach (var control in _children)
            {
                if (control is ResizeMarker marker)
                {
                    marker.Arrange(marker.GetSize());
                }
            }

            return finalSize;
        }
    }

    public abstract class ResizeMarker: Thumb
    {
        protected UIElement AdornedElement { get;}

        protected ResizeMarker(UIElement adornedElement)
        {
            Width = Height = 6;
            Background = Brushes.Transparent;
            AdornedElement = adornedElement;
            BorderThickness = new Thickness(1);
            BorderBrush = new SolidColorBrush(SystemColors.HighlightColor);
            DragDelta += OnDrag;
        }

        public virtual void OnDrag(object sender, DragDeltaEventArgs e)
        {
        }

        public abstract Rect GetSize();
    }

    public class TopLeftResizeMarker : ResizeMarker
    {
        public TopLeftResizeMarker(UIElement adornedElement)
            :base(adornedElement)
        {
            Cursor = Cursors.SizeNWSE;
        }

        public override void OnDrag(object sender, DragDeltaEventArgs e)
        {
            var newWidth = Math.Max(AdornedElement.DesiredSize.Width - e.HorizontalChange,
                (AdornedElement as FrameworkElement).MinWidth);
            var oldLeft = Canvas.GetLeft(AdornedElement);
            var newLeft = oldLeft - (newWidth - AdornedElement.DesiredSize.Width);
            (AdornedElement as FrameworkElement).Width = newWidth;
            Canvas.SetLeft(AdornedElement, newLeft);
        }

        public override Rect GetSize()
        {
            var width = this.DesiredSize.Width;
            var height = this.DesiredSize.Height;

            return new Rect(-width / 2, -height / 2, width, height);
        }
    }
}
