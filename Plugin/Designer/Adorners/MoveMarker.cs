using Designer.ExtensionMethods;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

namespace Designer.Adorners
{
    public class MoveMarker : Marker
    {
        public MoveMarker(UIElement adornedElement)
            : base(adornedElement)
        {
            Cursor = Cursors.SizeAll;
        }

        protected override void OnDrag(object sender, DragDeltaEventArgs e)
        {
            double oldLeft = Canvas.GetLeft(AdornedElement);
            double newLeft = oldLeft + e.HorizontalChange;
            newLeft = newLeft.Clamp(0, ContainerRect.Width - AdornedElement.DesiredSize.Width);
            Canvas.SetLeft(AdornedElement, newLeft);

            double oldTop = Canvas.GetTop(AdornedElement);
            double newTop = oldTop + e.VerticalChange;
            newTop = newTop.Clamp(0, ContainerRect.Height - AdornedElement.DesiredSize.Height);
            Canvas.SetTop(AdornedElement, newTop);
        }

        protected override void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
            if (adornerLayer == null)
                return;

            adornerLayer.Add(new PositionAdorner(AdornedElement));
            base.OnDragStarted(sender, e);
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
