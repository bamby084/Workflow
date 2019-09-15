using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using System.Text;
//using Sid.Windows.Controls;
using System.Windows.Media.Animation;

namespace Designer
{
    /// <summary>
    /// Interaction logic for LibraryTreeItem.xaml
    /// </summary>
    public partial class LibraryTreeItem : TreeViewItem
    {
        public bool bControlFocus = false;
        public bool EditMode = false;

        //Point startItemDragPoint = new Point(0, 0);

        public LibraryTreeItem()
        {
            InitializeComponent();
        }

        private TvItemCategory _ItemCategory;
        private Type _ElementType;

        public Type ElementType => _ElementType;
        public TvItemCategory ItemCategory => _ItemCategory;

        protected LibraryTreeItem(Type type, TvItemCategory category)
        {
            InitializeComponent();
            _ElementType = type;
            _ItemCategory = category;
            
        }

        private void btnTreeMouseUp_Delete(object sender, MouseButtonEventArgs e)
        {
            bool delete = true;
            if (imgLibrary.Visibility == Visibility.Visible)
            {
                if (MessageBox.Show("Do you want to remove this item?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    delete = true;
                }
                else
                {
                    delete = false;
                }
            }

            if (delete)
            {
                if (ItemCategory == TvItemCategory.PARAGRAPH_STYLES)
                    DesignerPage.self.DeleteParagraphStyle(this);
                else if (ItemCategory == TvItemCategory.TEXT_STYLES)
                    DesignerPage.self.DeleteTextStyle(this);
                else if (ItemCategory == TvItemCategory.FONTS)
                    DesignerPage.self.DeleteFont(this);
                else if (ItemCategory == TvItemCategory.IMAGES)
                    DesignerPage.self.DeleteImage(this);
                else if (ItemCategory == TvItemCategory.TABLES)
                    DesignerPage.self.DeleteTable(this);
                else if (ItemCategory == TvItemCategory.LINE_STYLES)
                    DesignerPage.self.DeleteLineStyle(this);
                else if (ItemCategory == TvItemCategory.BORDER_STYLES)
                    DesignerPage.self.DeleteBorderStyle(this);
                else if (ItemCategory == TvItemCategory.COLORS)
                    DesignerPage.self.DeleteColor(this);
                else if (ItemCategory == TvItemCategory.BLOCK_STYLES)
                    DesignerPage.self.DeleteBlock(this);
                else if (ItemCategory == TvItemCategory.FLOW_STYLES)
                    DesignerPage.self.DeleteFlow(this);
            }
        }

        public void changedID(string newName)
        {

            if (ItemCategory == TvItemCategory.PARAGRAPH_STYLES)
            {
                ParagraphStyle ps = this.Resources["ParagraphStyleRef"] as ParagraphStyle;
                ps.Id = newName;
            }
            else if (ItemCategory == TvItemCategory.TEXT_STYLES)
            {
                TextStyle ts = this.Resources["TextStyleRef"] as TextStyle;
                ts.Id = newName;
            }
            else if (ItemCategory == TvItemCategory.FONTS)
            {
                FontStyle fs = this.Resources["FontStyleRef"] as FontStyle;
                fs.Id = newName;
            }
            else if (ItemCategory == TvItemCategory.IMAGES)
            {
                ImageStyle iss = this.Resources["ImageStyleRef"] as ImageStyle;
                iss.Id = newName;
            }
            else if (ItemCategory == TvItemCategory.TABLES)
            {
                TableStyle ts = this.Resources["TableStyleRef"] as TableStyle;
                ts.Id = newName;
            }
            else if (ItemCategory == TvItemCategory.LINE_STYLES)
            {
                LineStyle ls = this.Resources["LineStyleRef"] as LineStyle;
                ls.Id = newName;
            }
            else if (ItemCategory == TvItemCategory.BORDER_STYLES)
            {
                BorderStyle bs = this.Resources["BorderStyleRef"] as BorderStyle;
                bs.Id = newName;
            }
            else if (ItemCategory == TvItemCategory.COLORS)
            {
                ColorStyle cs = this.Resources["ColorStyleRef"] as ColorStyle;
                cs.Id = newName;
            }
            else if (ItemCategory == TvItemCategory.BLOCK_STYLES)
            {
                BlockStyle cs = this.Resources["BlockStyleRef"] as BlockStyle;
                cs.Id = newName;
            }


        }

        public string createNewName(string oldName)
        {
            string newName = oldName;
            if (oldName.Contains("-"))
            {
                string next = oldName.Substring(oldName.LastIndexOf("-")).Replace("-", "");
                if (oldName.LastIndexOf(next) == oldName.Length - 1)
                {
                    if (char.IsDigit(next, 0))
                    {
                        oldName = oldName.Replace("-" + next, "");
                        newName = oldName + "-" + (Convert.ToInt32(next) + 1).ToString();
                    }
                    else
                        newName = oldName + "-1";
                }
                else
                    newName = oldName + "-1";
            }
            else
            {
                newName = oldName + "-1";
            }
            return newName;

        }

        private void TreeViewItem_GotFocus(object sender, RoutedEventArgs e)
        {
            //this.IsSelected = true;
        }

        private void TreeViewItem_LostFocus(object sender, RoutedEventArgs e)
        {
            //this.IsSelected = false;
            Bd.BorderBrush = Brushes.Transparent;

           
        }

        private void TreeViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
        }

        private void TreeViewItem_MouseLeave(object sender, MouseEventArgs e)
        {
        }

        private void TreeViewItem_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void TreeViewItem_FocusableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }


        private void label_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
        }

        private void label_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
        }

        public void label_DoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        public string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings. 
            try
            {
                //@"^[?!\d]"   @"^\d+(?!$)"

                string temp = Regex.Replace(strIn, @"^\d+", "", RegexOptions.None);

                temp = Regex.Replace(temp, @"^\.+", "", RegexOptions.None);

                return Regex.Replace(temp, @"[^\w\.-]", "",
                                     RegexOptions.None);
            }
            // we should return Empty
            catch (Exception ex)
            {
                return String.Empty;
            }
        }

        private void txtbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)  //Cancel it
            {
                txtbox.Visibility = Visibility.Hidden;
                label.Visibility = Visibility.Visible;
                return;
            }

            if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Return)
            {
                if (txtbox.Text != "")
                {
                    string oldName = Convert.ToString(label.Content);

                    char[] separate = { '_' };
                    string[] token = oldName.Split(separate);
                    if (token.Length > 0)
                    {
                        string newName = token[0] + "__" + txtbox.Text.Trim();
                        label.Content = newName;
                        label.Visibility = Visibility.Visible;
                        txtbox.Visibility = Visibility.Hidden;
                        bControlFocus = false;

                        changedID(newName);
                    }

                    e.Handled = true;

                }
            }
        }
        private void txtbox_LostFocus(object sender, RoutedEventArgs e)
        {
            label.Visibility = Visibility.Visible;
            txtbox.Visibility = Visibility.Hidden;
        }

        private void stackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            label.Foreground = Brushes.IndianRed;
            imgTreeDelete.Visibility = Visibility.Visible;

            if (ItemCategory == TvItemCategory.PAGES ||
                ItemCategory == TvItemCategory.BLOCK_STYLES ||
                ItemCategory == TvItemCategory.FLOW_STYLES)
            {
                imgTreeAdd.Visibility = Visibility.Visible;
            }
            

            imgMore.Visibility = Visibility.Collapsed;
        }


        private void stackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.IsSelected == false)
                label.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));  //Light Blue Text
            imgTreeDelete.Visibility = Visibility.Hidden;

            if (ItemCategory == TvItemCategory.PAGES ||
                ItemCategory == TvItemCategory.BLOCK_STYLES ||
                ItemCategory == TvItemCategory.FLOW_STYLES)
            {
                imgTreeAdd.Visibility = Visibility.Hidden;
            }

            imgMore.Visibility = Visibility.Collapsed;
        }



        private void stackPanel_GotFocus(object sender, RoutedEventArgs e)
        {
            //Bd.BorderBrush = new SolidColorBrush(System.Drawing.Color.FromArgb(255, 195, 163, 76));
            label.Foreground = Brushes.IndianRed;
        }

        private void stackPanel_LostFocus(object sender, RoutedEventArgs e)
        {
            label.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            label.Foreground = Brushes.IndianRed;
            if (txtbox.Visibility == Visibility.Visible)
            {
                Keyboard.Focus(txtbox);
            }

            if (DesignerPage.self.chkBlockArrow.IsChecked == true)
            {
                if (ItemCategory == TvItemCategory.BLOCK_STYLES)
                {
                    BlockStyle bs = Resources["BlockStyleRef"] as BlockStyle;
                    bs.ShowControlBorder(true);
                }
            }
        }

        private void TreeViewItem_UnSelected(object sender, RoutedEventArgs e)
        {
            if (imgLibrary.Visibility == Visibility.Visible)
            {
                //Bd.BorderBrush = Brushes.Transparent;
                label.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            }

            if (ItemCategory == TvItemCategory.BLOCK_STYLES)
            {
                BlockStyle bs = Resources["BlockStyleRef"] as BlockStyle;
                bs.ShowControlBorder(false);
            }
        }



        private void stackPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void stackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void TreeViewItem_PreviewDoubleClick(object sender, MouseButtonEventArgs e) //for #1768
        {
            TextBlock textBlock = e.OriginalSource as TextBlock;
            if (textBlock == null)
            {
                e.Handled = true;
            }
        }

        private void TreeViewItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DesignerPage.self.treeItem_DoubleClick(sender, e);
        }

        private void TreeViewItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (imgLibrary.Visibility == Visibility.Visible)
            {
                //this.IsSelected = true;
            }
        }

        private void TreeViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            imgTreeDelete.Visibility = Visibility.Hidden;
            if (ItemCategory == TvItemCategory.PAGES ||
                ItemCategory == TvItemCategory.BLOCK_STYLES ||
                ItemCategory == TvItemCategory.FLOW_STYLES)
            {
                imgTreeAdd.Visibility = Visibility.Hidden;
            }

            imgMore.Visibility = Visibility.Collapsed;
            txtbox.Visibility = Visibility.Collapsed;

        }

        private void imgMore_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e != null)
                e.Handled = true;  //to stop Double click from opening tree

            bControlFocus = true;
            label.Visibility = Visibility.Hidden;
            txtbox.Visibility = Visibility.Visible;
            txtbox.Text = label.Content.ToString();

            Keyboard.Focus(txtbox);
            txtbox.Focus();
            txtbox.SelectAll();
        }

        private void TreeViewItem_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F2)
            {
                bControlFocus = true;
                label.Visibility = Visibility.Hidden;
                txtbox.Visibility = Visibility.Visible;
                txtbox.Text = label.Content.ToString();

                Keyboard.Focus(txtbox);
                txtbox.Focus();
                txtbox.SelectAll();
            }
        }

        public string getNewName(string name, TreeViewItem Parentnode, LibraryTreeItem item)
        {
            
            string nodeName = name;
            string newname = "";
            //while (nodeName != newname)
            {
                newname = nodeName;
                /*
                foreach (LibraryTreeItem chitem in Parentnode.Items)
                {
                    if (item != null)
                    {
                        if (chitem.label.Content.ToString().ToLower().Equals(item.label.Content.ToString().ToLower()))
                            continue;
                    }
                    if (chitem.label.Content.ToString().ToLower().Equals(nodeName.ToLower()))
                    {
                        nodeName = createNewName(nodeName);
                        break;
                    }
                }
                */
            }
            return nodeName;
        }

        private void TreeViewItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //this.IsSelected = true;
        }

        private void imgLibrary_MouseUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void imgTreeAdd_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (imgLibrary.Visibility == Visibility.Visible)
            {
                if (ItemCategory == TvItemCategory.PAGES)
                {
                    Expander.Visibility = Visibility.Visible;
                    DesignerPage.self.AddNewBlock(this);
                }
                else if (ItemCategory == TvItemCategory.BLOCK_STYLES)
                {
                    Expander.Visibility = Visibility.Visible;
                    DesignerPage.self.AddNewFlow(this);
                }

            }
        }

        private void Expander_Checked(object sender, RoutedEventArgs e)
        {
            IsExpanded = true;
        }

        private void Expander_Unchecked(object sender, RoutedEventArgs e)
        {
            IsExpanded = false;
        }
    }

    public class ItemsToRemove
    {
        public LibraryTreeItem parent { get; set; }
        public LibraryTreeItem child { get; set; }
    }

}
