using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace Designer.Adorners
{
    public abstract class Marker : Thumb
    {
        private const double MarkerSize = 6;

        protected UIElement AdornedElement { get; }

        public Marker(UIElement adornedElement)
        {
            Width = Height = MarkerSize;
            Background = Brushes.White;

            AdornedElement = adornedElement;
            BorderThickness = new Thickness(1);
            BorderBrush = new SolidColorBrush(SystemColors.HighlightColor);

            DragDelta += OnDrag;
            DragStarted += OnDragStarted;
            DragCompleted += OnDragCompleted;
        }

        static Marker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Marker), new FrameworkPropertyMetadata(typeof(Marker)));
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

        public abstract Rect GetSize();
    }
}
