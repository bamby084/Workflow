using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JdSuite.Common.TreeListView
{
    /// <summary>
    /// Interaction logic for TreeNodeX.xaml
    /// </summary>
    public partial class TreeNodeX : UserControl, INotifyPropertyChanged, INotifyPropertyChanging
    {
        static NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(TreeNodeX));
        readonly static GridLength CollapsedRowHeight = new GridLength(0, GridUnitType.Pixel);
        readonly static int ChildNodeLeftMargin = 10;
        readonly static int PropLineLength = 10;
        readonly static int NodeLineLength = 12;
        readonly static string NodeRowTag = "node";
        readonly static string PropRowTag = "prop";
        internal static TreeNodeX RootNode = null;

        static HeightToPositionConverter htoPosConverter = new HeightToPositionConverter();
        static NodeHeaderTextConverter nodeHeaderTextConverter = new NodeHeaderTextConverter();

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        private readonly static Rectangle SelectedRectangle = new Rectangle()
        {
            Fill = Brushes.LightBlue
        };

        private static TreeNodeX SelectedNode = null;

        public int TotalRecordCount
        {
            get
            {
                if (DataItem == null)
                    return 0;

                return DataItem.ChildCount;

            }
        }

        public int CurrentRecordNo
        {
            get
            {
                int rec = 0;
                if (textRecordNo == null)
                    return rec;

                int.TryParse(textRecordNo.Text, out rec);
                return rec;
            }
            set
            {
                if (textRecordNo != null)
                {
                    textRecordNo.Text = value.ToString();
                }
            }
        }



        public int SelectedRow { get; private set; }

        private DataItem _dataItem;
        public DataItem DataItem
        {
            get { return _dataItem; }
            set { SetPropertry(ref _dataItem, value); }
        }

        public Line NodeLine
        {
            get; set;
        }
        public TreeNodeX()
        {
            InitializeComponent();

        }


        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void OnPropertyChanging([CallerMemberName]string propertyName = null)
        {

            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
        protected bool SetPropertry<T>(ref T Storage, T Value, [CallerMemberName] string Propertyname = null)
        {
            if (EqualityComparer<T>.Default.Equals(Storage, Value)) return false;
            Storage = Value;
            OnPropertyChanged(Propertyname);
            return true;
        }


        public int ParentGridRowNo { get; set; }
        public TextBlock TypeTextBlock
        {
            get { return this.textBlockType; }
        }

        public string NodeText
        {
            get { return textNodeHeader.Text; }
        }

        public int Id { get; set; }

        private bool _iscollapsed;

        /// <summary>
        /// When True shows + on False it shows -
        /// </summary>
        public bool IsCollapsed
        {
            get { return _iscollapsed; }
            set
            {

                if (SetPropertry(ref _iscollapsed, value))
                {
                    if (_iscollapsed)
                    {
                        btnToggle.Content = "+";
                    }
                    else
                    {
                        btnToggle.Content = "-";
                    }
                }
            }
        }

        public void Expand(TreeNodeX treeNode)
        {
            treeNode.Visibility = Visibility.Visible;
            treeNode.nodeBorder.Visibility = Visibility.Visible;
            treeNode.grid.Visibility = Visibility.Visible;


            if (treeNode.NodeLine != null)
                treeNode.NodeLine.Visibility = Visibility.Visible;

            bool firstChildNodeVisible = false;

            for (int i = 1; i < treeNode.grid.RowDefinitions.Count; i++)
            {
                treeNode.grid.RowDefinitions[i].Height = CollapsedRowHeight;

                if ((string)treeNode.grid.RowDefinitions[i].Tag == PropRowTag)
                {
                    treeNode.grid.RowDefinitions[i].Height = GridLength.Auto;
                }
                else if (!firstChildNodeVisible)
                {
                    if ((string)treeNode.grid.RowDefinitions[i].Tag == NodeRowTag)
                    {
                        treeNode.grid.RowDefinitions[i].Height = GridLength.Auto;
                        firstChildNodeVisible = true;
                    }
                }
            }

        }

        public static void Collapse(TreeNodeX node, bool collapseChildren = true)
        {
            for (int i = 1; i < node.grid.RowDefinitions.Count; i++)
            {
                node.grid.RowDefinitions[i].Height = CollapsedRowHeight;
            }

            foreach (var line in node.grid.FindVisualChildren<Line>(2))
            {
                if (line.Name == "propLine")
                {
                    line.Visibility = Visibility.Collapsed;
                }
            }

            if (collapseChildren)
            {
                foreach (var child in node.grid.Children.OfType<TreeNodeX>())
                {
                    child.NodeLine.Visibility = Visibility.Collapsed;
                    child.Visibility = Visibility.Collapsed;
                    Collapse(child, collapseChildren);
                    child.IsCollapsed = true;
                }
            }


            node.IsCollapsed = true;
        }

        /// <summary>
        /// Shows node header alongwith its properties, set GridRow.Height=Auto in which this node lies, which is Parent grid out of this node
        /// </summary>
        /// <param name="node"></param>
        public void ShowNode(TreeNodeX node)
        {
            node.Visibility = Visibility.Visible;
            node.nodeBorder.Visibility = Visibility.Visible;
            node.grid.Visibility = Visibility.Visible;


            if (node.NodeLine != null)
                node.NodeLine.Visibility = Visibility.Visible;

            bool propertyExpanded = false;

            foreach (var line in node.grid.FindVisualChildren<Line>(2))
            {
                if (line.Name == "propLine")
                {
                    line.Visibility = Visibility.Visible;
                }
            }


            for (int i = 1; i < node.grid.RowDefinitions.Count; i++)
            {
                if ((string)node.grid.RowDefinitions[i].Tag == PropRowTag)
                {
                    node.grid.RowDefinitions[i].Height = GridLength.Auto;
                    propertyExpanded = true;
                }
            }

            //node.CurrentRecordNo = 0;

            if (propertyExpanded)
                node.IsCollapsed = false;
        }

        /// <summary>
        /// Shows node header alongwith its properties, set GridRow.Height=Auto in which this node lies, which is Parent grid out of this node
        /// </summary>
        /// <param name="node"></param>
        public static void HideNode(TreeNodeX node)
        {
            node.Visibility = Visibility.Collapsed;
            node.nodeBorder.Visibility = Visibility.Collapsed;
            node.grid.Visibility = Visibility.Collapsed;


            if (node.NodeLine != null)
                node.NodeLine.Visibility = Visibility.Collapsed;
            /*
            for (int i = 1; i < treeNode.grid.RowDefinitions.Count; i++)
            {
                if ((string)treeNode.grid.RowDefinitions[i].Tag == PropRowTag)
                {
                    treeNode.grid.RowDefinitions[i].Height = CollapsedRowHeight;
                }
            }*/
           // node.CurrentRecordNo = 0;
            node.IsCollapsed = true;
        }


        public static void AdjustColumnWidth(TreeNodeX rootNode, bool IgnoreCollapsed)
        {
            //this.rootNode.grid.ColumnDefinitions[0].

            var rect = LayoutInformation.GetLayoutSlot(rootNode.textBlockType);
            // Debug.WriteLine($"Rect={rect.ToString()}");
            foreach (var child in rootNode.grid.Children.OfType<TreeNodeX>())
            {
                if (child.IsCollapsed || child.Visibility != Visibility.Visible) continue;

                //logger.Info($"Adjusting_1 Parent={rootNode.NodeText}-->{child.NodeText}={rect.X}");
                child.SetColumnWidth(0, rect.X, IgnoreCollapsed);
            }
        }




        public void SetColumnWidth(int ColumnId, double xWidth, bool IgnoreCollapsed = true)
        {
            if (this.Visibility != Visibility.Visible)
                return;

            var width = xWidth - ((NodeLine.X2 - NodeLine.X1) - 1);

            if (width < 100)
            {
                logger.Warn($"Invalid column width value {width} in TreeNodeX");
                return;
            }

            if (this.DataItem.Level >= 3)
                width = width - 1;

            if (this.grid.ColumnDefinitions[ColumnId].Width.Value != width)
            {
                this.grid.ColumnDefinitions[ColumnId].Width = new GridLength(width);

                //logger.Info($"Setting_node_column_width Node:{this.NodeText}[{DataItem.Level}], Width={width}, ParentWidth={xWidth}");
            }

            foreach (var child in grid.Children.OfType<TreeNodeX>())
            {
                if (child.Visibility != Visibility.Visible)
                    continue;

                //logger.Info($"Setting_child_node_column_width Parent:{this.NodeText}[{DataItem.Level}]-->{child.NodeText}[{child.DataItem.Level}]  ParentWidth={width} ChildWidth would be <{width}");
                child.SetColumnWidth(ColumnId, width);
            }
        }


        private void BtnPage_Click(object sender, RoutedEventArgs e)
        {
            bool adjustWidth = false;
            Button source = sender as Button;
            if (e.OriginalSource == btnRight)
            {
                int recNo = CurrentRecordNo;
                if (recNo < TotalRecordCount)
                {
                    recNo++;
                    CurrentRecordNo = recNo;
                    ShowRecordNo(recNo);
                    adjustWidth = true;
                }


            }
            else if (e.OriginalSource == btnLeft)
            {
                int recNo = CurrentRecordNo;
                if (recNo > 0)
                {
                    recNo--;
                    CurrentRecordNo = recNo;
                    ShowRecordNo(recNo);
                    adjustWidth = true;
                }

            }

            if (adjustWidth)
                AdjustColumnWidth(RootNode, true);
        }


        private void BtnToggle_Click(object sender, RoutedEventArgs e)
        {

            if (IsCollapsed)
            {
                IsCollapsed = false;
                ShowNode(this);

                if (this.TotalRecordCount > 0)
                {
                    ShowRecordNo(CurrentRecordNo, true);
                }
            }
            else
            {
                IsCollapsed = true;
                Collapse(this);
            }

            e.Handled = true;
            AdjustColumnWidth(RootNode, true);
        }

        private void TextBoxInteger_TextChanged(object sender, TextChangedEventArgs e)
        {
            int result = 0;
            TextBox txt = sender as TextBox;
            if (txt != null && txt.Text.Length > 0)
            {
                if (!int.TryParse(txt.Text, out result))
                {
                    if (result >= 0)
                    {
                        txt.Text = txt.Text.Substring(0, txt.Text.Length - 1);
                        txt.CaretIndex = txt.Text.Length;
                    }
                }
            }

        }


        private void TextRecordNo_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                if (int.TryParse(textRecordNo.Text, out var recno))
                {
                    if (recno < 1)
                        return;

                    if (recno > TotalRecordCount)
                        return;

                    ShowRecordNo(recno);
                }
            }

        }

        public void ShowRecordNo(int RecordNo, bool ShowChild = false)
        {

            string text = "[" + RecordNo + "]";

            foreach (var node in grid.Children.OfType<TreeNodeX>())
            {
                int rowIndex = Grid.GetRow(node);

                if (node.NodeText.Contains(text))
                {
                    IsCollapsed = false;
                    ShowNode(node);
                    grid.RowDefinitions[rowIndex].Height = GridLength.Auto;
                    if (ShowChild)
                    {
                        node.ShowRecordNo(node.CurrentRecordNo, ShowChild);
                    }
                }
                else
                {

                    HideNode(node);
                    grid.RowDefinitions[rowIndex].Height = CollapsedRowHeight;
                    //if (!node.IsCollapsed)
                    //Collapse(node, false);
                }
            }

            //this.UpdateLayout();
        }




        #region NodeCreation

        /// <summary>
        /// Entry function to create new node
        /// </summary>
        /// <param name="dataNode"></param>
        /// <param name="treeNode"></param>
        public static void LoadData(DataItem dataNode, TreeNodeX treeNode)
        {
            treeNode.DataItem = dataNode;
            treeNode.DataContext = treeNode.DataItem;

            AddProps(dataNode, treeNode);

            treeNode.CurrentRecordNo = 0;
             
            if (dataNode.ChildCount > 0)
            {
                int childId = 0;
                foreach (var dataChild in dataNode.Children)
                {
                    childId++;
                    var childNode = AddChildNode(dataChild, treeNode);
                    childNode.textNodeHeader.Text = dataChild.Name + "[" + childId + "]";
                    childNode.CurrentRecordNo = 0;
                    HideNode(childNode);
                }
            }
            

            HideNode(treeNode);
        }

        /// <summary>
        /// Adds child nodes to parentTreeNode
        /// </summary>
        /// <param name="dataNode"></param>
        /// <param name="parentTreeNode"></param>
        /// <returns></returns>
        private static TreeNodeX AddChildNode(DataItem dataNode, TreeNodeX parentTreeNode)
        {

            var gridRow = new RowDefinition() { Height = GridLength.Auto };
            parentTreeNode.grid.RowDefinitions.Add(gridRow);
            int rowId = parentTreeNode.grid.RowDefinitions.Count - 1;

            gridRow.Tag = NodeRowTag;

            Line line = new Line();
            line.Name = "nodeLineHor";
            line.StrokeThickness = 1;
            line.Stroke = Brushes.Blue;
            line.VerticalAlignment = VerticalAlignment.Top;
            line.X1 = 0;
            line.X2 = NodeLineLength;
            line.Y1 = 10;
            line.Y2 = 10;

            Grid.SetColumn(line, 0);
            Grid.SetRow(line, rowId);

            //Add line to parent
            parentTreeNode.grid.Children.Add(line);

            //Binding binding = new Binding("ActualWidth");
            // binding.Source = nodeStackPanel;
            //binding.Converter = htoPosConverter;
            // binding.Mode = BindingMode.OneWay;

            // l.SetBinding(Line.Y1Property, binding);
            // l.SetBinding(Line.Y2Property, binding);
            //tn.SetBinding(TreeNodeX.WidthProperty, binding);




            TreeNodeX childTreeNode = new TreeNodeX();
            childTreeNode.HorizontalAlignment = HorizontalAlignment.Stretch;
            childTreeNode.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            childTreeNode.Margin = new Thickness(ChildNodeLeftMargin, 0, 0, 0);
            Grid.SetColumn(childTreeNode, 0);
            Grid.SetRow(childTreeNode, rowId);
            Grid.SetColumnSpan(childTreeNode, 3);

            childTreeNode.ParentGridRowNo = rowId;

            childTreeNode.NodeLine = line;
            childTreeNode.textNodeHeader.Text = dataNode.Name;

            //Add child node to Parent
            parentTreeNode.grid.Children.Add(childTreeNode);


            /*Removing line as requested by John
            Line hline = new Line()
            {
                X1 = 0,
                Y1 = 0,
                X2 = 1,
                Y2 = 0
            };

            Grid.SetRow(hline, 0);
            Grid.SetColumn(hline, 0);
            Grid.SetColumnSpan(hline, 3);
            hline.VerticalAlignment = VerticalAlignment.Bottom;
            hline.Stretch = Stretch.Uniform;
            hline.Stroke = Brushes.Red;
            hline.StrokeThickness = 0.5;
            childTreeNode.grid.Children.Add(hline);

            */

            //Load children and properties for childTreeNode
            LoadData(dataNode, childTreeNode);
            // if (dataNode.ChildCount > 1)
            {
                // childTreeNode.btnShowAll.Visibility = Visibility.Visible;
                childTreeNode.btnLeft.Visibility = Visibility.Visible;
                childTreeNode.btnRight.Visibility = Visibility.Visible;
                childTreeNode.textRecordNo.Visibility = Visibility.Visible;
                childTreeNode.textTotalChildCount.Visibility = Visibility.Visible;

            }
            // else
            {
                // childTreeNode.btnShowAll.Visibility = Visibility.Hidden;
                /*
                childTreeNode.btnLeft.Visibility = Visibility.Hidden;
                childTreeNode.btnRight.Visibility = Visibility.Hidden;
                childTreeNode.textRecordNo.Visibility = Visibility.Hidden;
                childTreeNode.textTotalChildCount.Visibility = Visibility.Hidden;*/
            }


            return childTreeNode;
        }

        /// <summary>
        /// Adds properties to treeNode, each property is added to new treeNode.grid row, such grid rows have Tag=PropRow, line.Name=propLine
        /// </summary>
        /// <param name="item"></param>
        /// <param name="treeNode"></param>
        private static void AddProps(DataItem item, TreeNodeX treeNode)
        {
            if (item.PropCount > 0)
            {
                // bool firstProp = true;
                foreach (var prop in item.Props)
                {

                    var rowdef = new RowDefinition() { Height = GridLength.Auto };
                    treeNode.grid.RowDefinitions.Add(rowdef);
                    int rowId = treeNode.grid.RowDefinitions.Count - 1;

                    rowdef.Tag = PropRowTag;

                    /*Removing line as requested by John
                 Line hline = new Line()
                 {
                     X1 = 0,
                     Y1 = 0,
                     X2 = 1,
                     Y2 = 0
                 };

                 Grid.SetRow(hline, rowId);
                 Grid.SetColumn(hline, 0);
                 Grid.SetColumnSpan(hline, 3);
                 hline.VerticalAlignment = VerticalAlignment.Bottom;
                 hline.Stretch = Stretch.Uniform;
                 hline.Stroke = Brushes.Black;
                 hline.StrokeThickness = 0.5;
                 treeNode.grid.Children.Add(hline);

                 if (firstProp)
                 {
                     hline = new Line()
                     {
                         X1 = 0,
                         Y1 = 0,
                         X2 = 1,
                         Y2 = 0
                     };

                     Grid.SetRow(hline, rowId);
                     Grid.SetColumn(hline, 0);
                     Grid.SetColumnSpan(hline, 3);
                     hline.VerticalAlignment = VerticalAlignment.Top;
                     hline.Stretch = Stretch.Uniform;
                     hline.Stroke = Brushes.Black;
                     hline.StrokeThickness = 0.5;
                     treeNode.grid.Children.Add(hline);
                     firstProp = false;
                 }
                 */

                    //1. StackPanel holds line and name only
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Name = "stackPanelProp";
                    stackPanel.Orientation = Orientation.Horizontal;
                    stackPanel.VerticalAlignment = VerticalAlignment.Top;
                    Grid.SetColumn(stackPanel, 0);
                    Grid.SetRow(stackPanel, rowId);

                    //2. Small Horizontal line for property
                    Line line = new Line();
                    line.Name = "propLine";
                    line.StrokeThickness = 1;
                    line.Stroke = Brushes.Orange;
                    line.VerticalAlignment = VerticalAlignment.Center;
                    line.X1 = 0;
                    line.X2 = PropLineLength;

                    Binding binding = new Binding("ActualHeight");
                    binding.Source = stackPanel;
                    binding.Converter = htoPosConverter;
                    binding.Mode = BindingMode.OneWay;

                    line.SetBinding(Line.Y1Property, binding);
                    line.SetBinding(Line.Y2Property, binding);


                    stackPanel.Children.Add(line);

                    //3. Textblock for property name
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = prop.Name;
                    stackPanel.Children.Add(textBlock);


                    treeNode.grid.Children.Add(stackPanel);

                    //4. TextBlock for property type
                    textBlock = new TextBlock();
                    textBlock.Text = prop.Type;
                    Grid.SetColumn(textBlock, 1);
                    Grid.SetRow(textBlock, rowId);

                    treeNode.grid.Children.Add(textBlock);


                    //5. TextBlock for property value
                    textBlock = new TextBlock();
                    textBlock.Text = prop.Value;
                    Grid.SetColumn(textBlock, 2);
                    Grid.SetRow(textBlock, rowId);

                    treeNode.grid.Children.Add(textBlock);
                }
            }
        }




        #endregion NodeCreation



        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            var point = Mouse.GetPosition(grid);

            int row = 0;
            int col = 0;
            double accumulatedHeight = 0.0;
            double accumulatedWidth = 0.0;

            // calc row mouse was over
            foreach (var rowDefinition in grid.RowDefinitions)
            {
                accumulatedHeight += rowDefinition.ActualHeight;
                if (accumulatedHeight >= point.Y)
                    break;
                row++;
            }

            // calc col mouse was over
            foreach (var columnDefinition in grid.ColumnDefinitions)
            {
                accumulatedWidth += columnDefinition.ActualWidth;
                if (accumulatedWidth >= point.X)
                    break;
                col++;
            }

            SelectedRow = row;
            SelectRow(row);

            e.Handled = true;
        }

        private void SelectRow(int row)
        {
            if (SelectedNode != null)
                SelectedNode.grid.Children.Remove(SelectedRectangle);


            Grid.SetColumn(SelectedRectangle, 0);
            Grid.SetRow(SelectedRectangle, row);
            Grid.SetColumnSpan(SelectedRectangle, 3);

            if (row < grid.RowDefinitions.Count)
            {
                var rowDef = grid.RowDefinitions[row];
                SelectedRectangle.Height = rowDef.ActualHeight - 2;
                grid.Children.Add(SelectedRectangle);
                Canvas.SetZIndex(SelectedRectangle, -1);
                SelectedNode = this;
                // MessageBox.Show($"Grid[{row},{col}]");
                //e.Handled = true;
            }
        }

        public override string ToString()
        {
            return this.NodeText;
        }
    }







    #region Converter_classes

    public class HeightToPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((((double)value) / 2) - 5);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NodeHeaderTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion Converter_classes
}
