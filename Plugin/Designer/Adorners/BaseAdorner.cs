using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Designer.Adorners
{
    public class BaseAdorner: Adorner
    {
        public VisualCollection Children { get; }

        public BaseAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            Children = new VisualCollection(this);
        }

        protected override Visual GetVisualChild(int index) => Children[index];

        protected override int VisualChildrenCount => Children.Count;
    }
}
