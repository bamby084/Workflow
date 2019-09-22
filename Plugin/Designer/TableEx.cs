using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Size = System.Windows.Size;

namespace Designer
{
    public class TableEx: Panel
    {
        public FrameworkElement Root { get; private set; }
       
        public int Columns { get; set; }
        public int Rows { get; set; }
        public double WidthPercentage { get; set; }

        public void Construct()
        {
            var table = new Table();
            table.BorderThickness = new Thickness(1);
            table.BorderBrush = new SolidColorBrush(Colors.HotPink);

            var body = new TableRowGroup();
            for (int i = 0; i < 7; i++)
            {
                table.Columns.Add(new TableColumn());
            }

            for (int i = 0; i < 4; i++)
            {
                var row = new TableRow();
                for (int j = 0; j < 7; j++)
                    row.Cells.Add(new TableCell());

                body.Rows.Add(row);
            }
            table.RowGroups.Add(body);

            var container = new FlowDocumentScrollViewer();
            container.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            var flowDocument = new FlowDocument();
            flowDocument.Blocks.Add(table);

            container.Document = flowDocument;
            Root = container;
            Children.Add(Root);
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {

            return Root;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var parent = (FrameworkElement)VisualTreeHelper.GetParent(this);
            Root.Measure(availableSize);
            return Root.DesiredSize;
            //return new Size(Root.Measure(availableSize));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Root.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return finalSize;
        }

    }
}
