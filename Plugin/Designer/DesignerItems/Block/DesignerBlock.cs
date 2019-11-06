using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Designer.Converters;
using Designer.ExtensionMethods;
using FigureLengthConverter = Designer.Converters.FigureLengthConverter;

namespace Designer.DesignerItems
{
    public class DesignerBlock : DesignerItem
    {
        private static readonly object LockObject = new object();
        private static int CurrentIndex;
        private ControlPropertiesViewModel _properties;
        
        public BlockDocument Editor { get; set; }

        static DesignerBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerBlock), new FrameworkPropertyMetadata(typeof(DesignerBlock)));
        }

        public override ControlPropertiesViewModel Properties => _properties;

        public override void OnApplyTemplate()
        {
            Editor = (BlockDocument)this.GetTemplateChild("PART_Document");
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
            table.Build(tableDialog.Columns, tableDialog.HeaderRows, tableDialog.BodyRows, tableDialog.FooterRows);

            var container = Editor.GetCaretContainer(LogicalDirection.Forward);
            if (container == null)
                Editor.Document.Blocks.Add(table);
            else
            {
                var tableContainer = new DesignerTableContainer(table);
                container.Inlines.Add(tableContainer);
                Editor.IsUndoEnabled = false;
                Editor.IsUndoEnabled = true;
            }
            
            DesignerTableManager.Instance.AddTable(table);
        }

        private void OnContextMenuOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            var contextMenu = (ContextMenu)sender;
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
            lock (LockObject)
            {
                CurrentIndex++;
                return CurrentIndex;
            }
        }
    }
}
