using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Designer.ExtensionMethods;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System;

namespace Designer.DesignerItems
{
    public class DesignerBlock : DesignerItem
    {
        private static readonly object LockObject = new object();
        private static int CurrentIndex;
        private ControlPropertiesViewModel _properties;
        private List<DesignerTableContainer> _tableContainers;

        public BlockDocument Editor { get; set; }

        static DesignerBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerBlock), new FrameworkPropertyMetadata(typeof(DesignerBlock)));
        }

        public DesignerBlock()
        {
            FontSize = 14;
            _properties = new BlockProperties();
            ((BlockProperties)_properties).OnInsertExistingTable += OnInsertExistingTable;
            _properties.Name = $"Block {GetNextIndex()}";
            _tableContainers = new List<DesignerTableContainer>();

            BindProperties();
            this.DataContext = _properties;
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
            contextMenu.FindMenuItemByName("cmiCut").Click += OnCut;
            contextMenu.FindMenuItemByName("cmiCopy").Click += OnCopy;
            contextMenu.FindMenuItemByName("cmiPaste").Click += OnPaste;
        }

        private void OnInsertTable(object sender, RoutedEventArgs routedEventArgs)
        {
            var tableDialog = new InsertTableWindow();
            var result = tableDialog.ShowDialog();
            if (result == null || result.Value == false)
                return;

            var tableProperties = TableProperties.Build(tableDialog.Columns, tableDialog.HeaderRows, tableDialog.BodyRows, tableDialog.FooterRows);
            InsertTable(tableProperties, true);
        }

        private void OnInsertExistingTable(object sender, EventArgs e)
        {
            var tableProperties = (TableProperties)sender;
            InsertTable(tableProperties, false);
        }

        private void InsertTable(TableProperties tableProperties, bool addToTableManager)
        {
            tableProperties.OnDeleted += OnDeleteTable;

            var table = new DesignerTable();
            table.Build(tableProperties);
            var tableContainer = new DesignerTableContainer(table);

            var container = Editor.GetCaretContainer(LogicalDirection.Forward);
            if (container == null)
            {
                var paragraph = new Paragraph();
                paragraph.Inlines.Add(tableContainer);
                Editor.Document.Blocks.Add(paragraph);
            }
            else
            {
                container.Inlines.Add(tableContainer);
                container.Inlines.Add(new LineBreak());
            }

            _tableContainers.Add(tableContainer);
            ((BlockProperties)Properties).Children.Add(tableProperties);

            if (addToTableManager)
                DesignerTableManager.Instance.AddTable(tableProperties);
        }

        private void OnDeleteTable(object sender, System.EventArgs e)
        {
            var tableProperties = (TableProperties)sender;
            var container = _tableContainers.FirstOrDefault(t => ((DesignerTable)t.Blocks.FirstBlock).Properties.Equals(tableProperties));

            if (container != null)
            {
                if (container.Parent != null)
                    (container.Parent as Paragraph).Inlines.Remove(container);

                _tableContainers.Remove(container);
            }

            ((BlockProperties)Properties).Children.Remove(tableProperties);
        }

        private void OnCut(object sender, RoutedEventArgs routedEventArgs)
        {
            Editor.Cut();
        }

        private void OnCopy(object sender, RoutedEventArgs routedEventArgs)
        {
            Editor.Copy();
        }

        private void OnPaste(object sender, RoutedEventArgs routedEventArgs)
        {
            Editor.Paste();
        }

        private void OnContextMenuOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            var contextMenu = (ContextMenu)sender;
            contextMenu.FindMenuItemByName("cmiCut").IsEnabled = !Editor.Selection.IsEmpty;
            contextMenu.FindMenuItemByName("cmiCopy").IsEnabled = !Editor.Selection.IsEmpty;
            contextMenu.FindMenuItemByName("cmiPaste").IsEnabled = Clipboard.ContainsText();
        }

        private int GetNextIndex()
        {
            lock (LockObject)
            {
                CurrentIndex++;
                return CurrentIndex;
            }
        }

        private void BindProperties()
        {
            var isSelectedBinding = new Binding("IsSelected");
            isSelectedBinding.Source = Properties;
            isSelectedBinding.Mode = BindingMode.TwoWay;
            SetBinding(IsSelectedProperty, isSelectedBinding);
        }
    }
}
