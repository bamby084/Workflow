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
        private ContextMenu _contextMenu;

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
                var figure = new Figure(table)
                {
                    Margin = new Thickness(0),
                    Padding = new Thickness(0),
                };

                var alignmentBinding = new Binding("Properties.Alignment");
                alignmentBinding.Source = table;
                alignmentBinding.Converter = new FigureHorizontalAnchorConverter();
                alignmentBinding.Mode = BindingMode.TwoWay;
                figure.SetBinding(Figure.HorizontalAnchorProperty, alignmentBinding);

                var widthBinding = new Binding("Properties.WidthPercentage");
                widthBinding.Converter = new FigureLengthConverter();
                widthBinding.ConverterParameter = FigureUnitType.Page;
                widthBinding.Source = table;
                widthBinding.Mode = BindingMode.TwoWay;
                figure.SetBinding(Figure.WidthProperty, widthBinding);
                
                //foreach(var column in table.Columns)
                //{
                //    var binding = new Binding("");
                //    binding.Source = table;
                //}


                container.Inlines.Add(figure);
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
