using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Designer.ExtensionMethods;
using Size = System.Windows.Size;

namespace Designer
{
    public enum TableAlignment
    {
        Left,
        Right
    }

    public class TableEx: Panel
    {
        public FrameworkElement Root { get; private set; }
       
        public int Columns { get; set; }
        public int HeaderRows { get; set; }
        public int FooterRows { get; set; }
        public int BodyRows { get; set; }
        public double WidthPercentage { get; set; } = 1.0;
        public new double MinWidth { get; set; }
        public TableAlignment Alignment { get; set; }

        public void Build()
        {
            var table = new Table();
            table.AddColumns(Columns);

            if (HeaderRows > 0)
            {
                var header = CreateRowGroup(HeaderRows, Columns);
                table.RowGroups.Add(header);
            }

            var body = CreateRowGroup(BodyRows, Columns);
            table.RowGroups.Add(body);

            if (FooterRows > 0)
            {
                var footer = CreateRowGroup(FooterRows, Columns);
                table.RowGroups.Add(footer);
            }

            var container = new FlowDocumentScrollViewer();
            container.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            var flowDocument = new FlowDocument();
            flowDocument.Blocks.Add(table);

            container.Document = flowDocument;
            Root = container;
            Children.Add(Root);
        }

        private TableRowGroup CreateRowGroup(int rows, int columns)
        {
            var rowGroup = new TableRowGroup();
            for (int i = 0; i < rows; i++)
            {
                var row = new TableRow();
                row.AddEmptyCells(columns);

                rowGroup.Rows.Add(row);
            }

            return rowGroup;
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            return Root;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Root.Measure(availableSize);
            return Root.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Root.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return finalSize;
        }

    }
}
