using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Designer.Adorners
{
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
            DragStarted += OnDragStarted;
            DragCompleted += OnDragCompleted;
        }

        protected virtual void OnDragStarted(object sender, DragStartedEventArgs e)
        {
        }

        protected virtual void OnDrag(object sender, DragDeltaEventArgs e)
        {
        }

        protected virtual void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
            if (adornerLayer == null)
                return;

            var adorners = adornerLayer.GetAdorners(AdornedElement);
            foreach (var adorner in adorners)
            {
                if (adorner is SizeAdorner)
                    adornerLayer.Remove(adorner);
            }
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

        protected override void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
            if (adornerLayer == null)
                return;

            adornerLayer.Add(new TopSizeAdorner(AdornedElement));
            adornerLayer.Add(new LeftSizeAdorner(AdornedElement));
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

        protected override void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
            if (adornerLayer == null)
                return;

            adornerLayer.Add(new TopSizeAdorner(AdornedElement));
            adornerLayer.Add(new RightSizeAdorner(AdornedElement));
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

        protected override void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
            if (adornerLayer == null)
                return;

            adornerLayer.Add(new LeftSizeAdorner(AdornedElement));
            adornerLayer.Add(new BottomSizeAdorner(AdornedElement));
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
            : base(adornedElement)
        {
            Cursor = Cursors.SizeNWSE;
        }

        protected override void OnDrag(object sender, DragDeltaEventArgs e)
        {
            ExpandToRight(e.HorizontalChange);
            ExpandToBottom(e.VerticalChange);
        }

        protected override void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
            if (adornerLayer == null)
                return;

            adornerLayer.Add(new RightSizeAdorner(AdornedElement));
            adornerLayer.Add(new BottomSizeAdorner(AdornedElement));
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

        protected override void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
            if (adornerLayer == null)
                return;

            adornerLayer.Add(new BottomSizeAdorner(AdornedElement));
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

        protected override void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
            if (adornerLayer == null)
                return;

            adornerLayer.Add(new BottomSizeAdorner(AdornedElement));
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

        protected override void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
            if (adornerLayer == null)
                return;

            adornerLayer.Add(new RightSizeAdorner(AdornedElement));
        }
    }

    public class CenterBottomResizeMarker : ResizeMarker
    {
        public CenterBottomResizeMarker(UIElement adornedElement)
            : base(adornedElement)
        {
            Cursor = Cursors.SizeNS;
        }

        protected override void OnDrag(object sender, DragDeltaEventArgs e)
        {
            ExpandToBottom(e.VerticalChange);
        }

        protected override void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
            if (adornerLayer == null)
                return;

            adornerLayer.Add(new RightSizeAdorner(AdornedElement));
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
            : base(adornedElement)
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

        protected override void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
            if (adornerLayer == null)
                return;

            adornerLayer.Add(new PositionAdorner(AdornedElement));
        }

        protected override void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
            if (adornerLayer == null)
                return;

            var adorners = adornerLayer.GetAdorners(AdornedElement);
            var positionAdorner = adorners.FirstOrDefault(adorner => adorner.GetType() == typeof(PositionAdorner));

            if (positionAdorner != null)
                adornerLayer.Remove(positionAdorner);
        }

        public override Rect GetSize()
        {
            var width = this.DesiredSize.Width;
            var height = this.DesiredSize.Height;

            return new Rect((AdornedElement.DesiredSize.Width - width) / 2, (AdornedElement.DesiredSize.Height - height) / 2, width, height);
        }
    }

}
