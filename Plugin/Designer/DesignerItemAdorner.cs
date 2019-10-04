using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Designer
{
    public class DesignerItemAdorner : Adorner
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

    public abstract class ResizeMarker : Thumb
    {
        private new const double MinWidth = 30.0;
        private new const double MinHeight = 30.0;

        protected UIElement AdornedElement { get; }

        protected ResizeMarker(UIElement adornedElement)
        {
            Width = Height = 6;
            Background = Brushes.Transparent;
            AdornedElement = adornedElement;
            BorderThickness = new Thickness(1);
            BorderBrush = new SolidColorBrush(SystemColors.HighlightColor);
            DragDelta += OnDrag;
        }

        protected virtual void OnDrag(object sender, DragDeltaEventArgs e)
        {
        }

        protected virtual void ExpandToLeft(double delta)
        {
            double newWidth = Math.Max(AdornedElement.DesiredSize.Width - delta, MinWidth);
            AdornedElement.SetValue(WidthProperty, newWidth);

            var oldLeft = Canvas.GetLeft(AdornedElement);
            Canvas.SetLeft(AdornedElement, oldLeft - (newWidth - AdornedElement.DesiredSize.Width));
        }

        protected virtual void ExpandToTop(double delta)
        {
            var newHeight = Math.Max(AdornedElement.DesiredSize.Height - delta, MinHeight);
            AdornedElement.SetValue(HeightProperty, newHeight);

            var oldTop = Canvas.GetTop(AdornedElement);
            Canvas.SetTop(AdornedElement, oldTop - (newHeight - AdornedElement.DesiredSize.Height));
        }

        protected virtual void ExpandToRight(double delta)
        {
            double newWidth = Math.Max(AdornedElement.DesiredSize.Width + delta, MinWidth);
            AdornedElement.SetValue(WidthProperty, newWidth);
        }

        protected virtual void ExpandToBottom(double delta)
        {
            var newHeight = Math.Max(AdornedElement.DesiredSize.Height + delta, MinHeight);
            AdornedElement.SetValue(HeightProperty, newHeight);
        }

        public abstract Rect GetSize();
    }

    public class TopLeftResizeMarker : ResizeMarker
    {
        public TopLeftResizeMarker(UIElement adornedElement)
            : base(adornedElement)
        {
            Cursor = Cursors.SizeNWSE;
        }

        protected override void OnDrag(object sender, DragDeltaEventArgs e)
        {
            ExpandToLeft(e.HorizontalChange);
            ExpandToTop(e.VerticalChange);
        }

        public override Rect GetSize()
        {
            var width = this.DesiredSize.Width;
            var height = this.DesiredSize.Height;

            return new Rect(-width / 2, -height / 2, width, height);
        }
    }

    public class TopRightResizeMarker : ResizeMarker
    {
        public TopRightResizeMarker(UIElement adornedElement)
            : base(adornedElement)
        {
            Cursor = Cursors.SizeNESW;
        }

        protected override void OnDrag(object sender, DragDeltaEventArgs e)
        {
            ExpandToRight(e.HorizontalChange);
            ExpandToTop(e.VerticalChange);
        }

        public override Rect GetSize()
        {
            var width = this.DesiredSize.Width;
            var height = this.DesiredSize.Height;

            return new Rect(AdornedElement.DesiredSize.Width - width / 2, -height / 2, width, height);
        }
    }

    public class BottomLeftResizeMarker : ResizeMarker
    {
        public BottomLeftResizeMarker(UIElement adornedElement)
            : base(adornedElement)
        {
            Cursor = Cursors.SizeNESW;
        }

        protected override void OnDrag(object sender, DragDeltaEventArgs e)
        {
            ExpandToLeft(e.HorizontalChange);
            ExpandToBottom(e.VerticalChange);
        }

        public override Rect GetSize()
        {
            var width = this.DesiredSize.Width;
            var height = this.DesiredSize.Height;

            return new Rect(-width / 2, AdornedElement.DesiredSize.Height - height / 2, width, height);
        }
    }

    public class BottomRightResizeMarker : ResizeMarker
    {
        public BottomRightResizeMarker(UIElement adornedElement)
            :base(adornedElement)
        {
            Cursor = Cursors.SizeNWSE;
        }

        protected override void OnDrag(object sender, DragDeltaEventArgs e)
        {
            ExpandToRight(e.HorizontalChange);
            ExpandToBottom(e.VerticalChange);
        }

        public override Rect GetSize()
        {
            var width = this.DesiredSize.Width;
            var height = this.DesiredSize.Height;

            return new Rect(AdornedElement.DesiredSize.Width - width / 2, AdornedElement.DesiredSize.Height - height / 2, width, height);
        }
    }

    public class CenterLeftMarker : ResizeMarker
    {
        public CenterLeftMarker(UIElement adornedElement)
            : base(adornedElement)
        {
            Cursor = Cursors.SizeWE;
        }

        protected override void OnDrag(object sender, DragDeltaEventArgs e)
        {
            ExpandToLeft(e.HorizontalChange);
        }

        public override Rect GetSize()
        {
            var width = this.DesiredSize.Width;
            var height = this.DesiredSize.Height;

            return new Rect(-width / 2, (AdornedElement.DesiredSize.Height - height) / 2, width, height);
        }
    }

    public class CenterRightMarker : ResizeMarker
    {
        public CenterRightMarker(UIElement adornedElement)
            : base(adornedElement)
        {
            Cursor = Cursors.SizeWE;
        }

        protected override void OnDrag(object sender, DragDeltaEventArgs e)
        {
            ExpandToRight(e.HorizontalChange);
        }

        public override Rect GetSize()
        {
            var width = this.DesiredSize.Width;
            var height = this.DesiredSize.Height;

            return new Rect(AdornedElement.DesiredSize.Width - width / 2, (AdornedElement.DesiredSize.Height - height) / 2, width, height);
        }
    }

    public class CenterTopResizeMarker : ResizeMarker
    {
        public CenterTopResizeMarker(UIElement adornedElement)
            : base(adornedElement)
        {
            Cursor = Cursors.SizeNS;
        }

        public override Rect GetSize()
        {
            var width = this.DesiredSize.Width;
            var height = this.DesiredSize.Height;

            return new Rect((AdornedElement.DesiredSize.Width - width) / 2, -height / 2, width, height);
        }

        protected override void OnDrag(object sender, DragDeltaEventArgs e)
        {
            ExpandToTop(e.VerticalChange);
        }
    }

    public class CenterBottomResizeMarker : ResizeMarker
    {
        public CenterBottomResizeMarker(UIElement adornedElement)
            :base(adornedElement)
        {
            Cursor = Cursors.SizeNS;
        }

        protected override void OnDrag(object sender, DragDeltaEventArgs e)
        {
            ExpandToBottom(e.VerticalChange);
        }

        public override Rect GetSize()
        {
            var width = this.DesiredSize.Width;
            var height = this.DesiredSize.Height;

            return new Rect((AdornedElement.DesiredSize.Width - width) / 2, AdornedElement.DesiredSize.Height - height / 2, width, height);
        }
    }

    public class CenterMarker : ResizeMarker
    {
        public CenterMarker(UIElement adornedElement)
            :base(adornedElement)
        {
            Cursor = Cursors.SizeAll;
        }

        protected override void OnDrag(object sender, DragDeltaEventArgs e)
        {
            var oldLeft = Canvas.GetLeft(AdornedElement);
            Canvas.SetLeft(AdornedElement, oldLeft + e.HorizontalChange);

            var oldTop = Canvas.GetTop(AdornedElement);
            Canvas.SetTop(AdornedElement, oldTop + e.VerticalChange);
        }

        public override Rect GetSize()
        {
            var width = this.DesiredSize.Width;
            var height = this.DesiredSize.Height;

            return new Rect((AdornedElement.DesiredSize.Width - width) / 2, (AdornedElement.DesiredSize.Height - height) / 2, width, height);
        }
    }

    public class SizeAdorner : Adorner
    {
        private VisualCollection _children;

        public SizeAdorner(UIElement adornedElement)
            :base(adornedElement)
        {
            _children = new VisualCollection(this);
            BuildAdorner();
        }

        private TextBlock _text;
        private void BuildAdorner()
        {
            var line = new Line();
            line.StrokeThickness = 1;
            line.SnapsToDevicePixels = true;
            line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            line.Stroke = new SolidColorBrush(SystemColors.HighlightColor);
            
            var binding = new Binding("Width");
            binding.Source = AdornedElement;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            line.SetBinding(Line.X2Property, binding);

            _text = new TextBlock();
            _text.SetBinding(TextBlock.TextProperty, binding);

            _children.Add(line);
            _children.Add(_text);
        }

        protected override int VisualChildrenCount => _children.Count;
        protected override Visual GetVisualChild(int index) => _children[index];
        protected override Size ArrangeOverride(Size finalSize)
        {
            (_children[0] as Line).Arrange(new Rect(0, AdornedElement.DesiredSize.Height + 10, AdornedElement.DesiredSize.Width, this.DesiredSize.Height));
            _text.Arrange(new Rect((AdornedElement.DesiredSize.Width - _text.DesiredSize.Width) / 2, AdornedElement.DesiredSize.Height + 10, _text.DesiredSize.Width, _text.DesiredSize.Height));
            return finalSize;
        }
    }
}
