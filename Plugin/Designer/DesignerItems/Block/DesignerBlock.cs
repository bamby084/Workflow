using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Designer.ExtensionMethods;

namespace Designer.DesignerItems
{
    public class DesignerBlock: DesignerItem
    {
        private static readonly object LockObject = new object();
        private static int CurrentIndex;
        private ControlPropertiesViewModel _properties;
        private ContextMenu _contextMenu;

        public BlockDocument Editor { get; set; }

        static DesignerBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerBlock), new FrameworkPropertyMetadata(typeof(DesignerBlock)));
        }

        public override ControlPropertiesViewModel Properties => _properties;

        public override void OnApplyTemplate()
        {
            Editor = (BlockDocument) this.GetTemplateChild("PART_Document");
            RegisterContextMenuEvents();
        }

        private void RegisterContextMenuEvents()
        {
            if (Editor?.ContextMenu == null)
                return;

            var contextMenu = Editor.ContextMenu;
            contextMenu.Opened += OnContextMenuOpened;
            contextMenu.FindMenuItemByName("cmiInsertTable").Click += OnInsertTable;
        }

        private void OnInsertTable(object sender, RoutedEventArgs routedEventArgs)
        {
            var tableDialog = new InsertTableWindow();
            var result = tableDialog.ShowDialog();
            if (result == null || result.Value == false)
                return;

            var table = new DesignerTable();
            table.Properties.HeaderRows = tableDialog.HeaderRows;
            table.Properties.BodyRows = tableDialog.BodyRows;
            table.Properties.FooterRows = tableDialog.FooterRows;
            table.Properties.Columns = tableDialog.Columns;
            table.Build();

            Paragraph container = Editor.GetNearestParagraphFromCurrentCaret(LogicalDirection.Forward);
            if (container == null)
                Editor.Document.Blocks.Add(table);
            else
            {
                try
                {
                    Editor.Document.Blocks.InsertAfter(container, table);
                }
                catch
                {
                    var figure = new Figure(table)
                    {
                        Margin = new Thickness(0),
                        Padding = new Thickness(0)
                    };
                    container.Inlines.Add(figure);
                }
            }

            DesignerTableManager.Instance.AddTable(table);
        }

        private void OnContextMenuOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            var contextMenu = (ContextMenu) sender;
            contextMenu.FindMenuItemByName("cmiCut").IsEnabled = !Editor.Selection.IsEmpty;
            contextMenu.FindMenuItemByName("cmiCopy").IsEnabled = !Editor.Selection.IsEmpty;
            contextMenu.FindMenuItemByName("cmiPaste").IsEnabled = Clipboard.ContainsText();
        }

        public DesignerBlock()
        {
            FontSize = 14;
            _properties = new BlockProperties();
            _properties.Name = $"Block {GetNextIndex()}";
            this.DataContext = _properties;
        }

        private int GetNextIndex()
        {
            lock(LockObject)
            {
                CurrentIndex ++;
                return CurrentIndex;
            }
        }
    }
}
