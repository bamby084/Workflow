using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WPFCanvasTable;

namespace Designer
{
    /// <summary>
    /// Interaction logic for FlowControl.xaml
    /// </summary>
    public partial class FlowControl : System.Windows.Controls.UserControl
    {

        private System.Drawing.Rectangle contentRectagle;

        public FlowControl()
        {
            InitializeComponent();

            ExtendedRichTextBox.CharStyle charStyle1 = new ExtendedRichTextBox.CharStyle();
            ExtendedRichTextBox.ParaLineSpacing paraLineSpacing1 = new ExtendedRichTextBox.ParaLineSpacing();
            ExtendedRichTextBox.ParaListStyle paraListStyle1 = new ExtendedRichTextBox.ParaListStyle();

            this.TextEditor.BorderStyle = System.Windows.Forms.BorderStyle.None;

            this.TextEditor.AcceptsTab = true;
            this.TextEditor.EnableAutoDragDrop = true;
            this.TextEditor.HideSelection = false;
            this.TextEditor.Name = "TextEditor";
            charStyle1.Bold = false;
            charStyle1.Italic = false;
            charStyle1.Link = false;
            charStyle1.Strikeout = false;
            charStyle1.Underline = false;
            this.TextEditor.SelectionCharStyle = charStyle1;
            this.TextEditor.SelectionFont2 = new System.Drawing.Font("Microsoft Sans Serif", 1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Inch);
            paraLineSpacing1.ExactSpacing = 0;
            paraLineSpacing1.SpacingStyle = ExtendedRichTextBox.ParaLineSpacing.LineSpacingStyle.Unknown;
            this.TextEditor.SelectionLineSpacing = paraLineSpacing1;
            paraListStyle1.BulletCharCode = ((short)(0));
            paraListStyle1.NumberingStart = ((short)(0));
            paraListStyle1.Style = ExtendedRichTextBox.ParaListStyle.ListStyle.NumberAndParenthesis;
            paraListStyle1.Type = ExtendedRichTextBox.ParaListStyle.ListType.None;
            this.TextEditor.SelectionListType = paraListStyle1;
            this.TextEditor.SelectionOffsetType = ExtendedRichTextBox.OffsetType.None;
            this.TextEditor.SelectionSpaceAfter = 0;
            this.TextEditor.SelectionSpaceBefore = 0;

            this.TextEditor.TabIndex = 5;
            this.TextEditor.Text = "";
            //this.TextEditor.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.TextEditor_LinkClicked);
            this.TextEditor.SelectionChanged += new System.EventHandler(this.TextEditor_SelectionChanged);
            this.TextEditor.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TextEditor_MouseMove);
            this.TextEditor.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TextEditor_MouseUp);
            this.TextEditor.ContentsResized += new ContentsResizedEventHandler(this.TextEditor_ContentsResized);

            this.Ruler.BackColor = System.Drawing.Color.Transparent;
            this.Ruler.Font = new System.Drawing.Font("Arial", 7.25F);
            this.Ruler.LeftHangingIndent = 0;
            this.Ruler.LeftIndent = 0;
            this.Ruler.LeftMargin = 1;
            this.Ruler.Name = "Ruler";
            this.Ruler.NoMargins = true;
            this.Ruler.RightIndent = 0;
            this.Ruler.RightMargin = 1;
            this.Ruler.TabIndex = 8;
            this.Ruler.TabsEnabled = true;
            this.Ruler.LeftHangingIndentChanging += new TextRuler.IndentChangedEventHandler(this.Ruler_LeftHangingIndentChanging);
            this.Ruler.LeftIndentChanging += new TextRuler.IndentChangedEventHandler(this.Ruler_LeftIndentChanging);
            this.Ruler.RightIndentChanging += new TextRuler.IndentChangedEventHandler(this.Ruler_RightIndentChanging);
            this.Ruler.BothLeftIndentsChanged += new TextRuler.MultiIndentChangedEventHandler(this.Ruler_BothLeftIndentsChanged);
            this.Ruler.TabAdded += new TextRuler.TabChangedEventHandler(this.Ruler_TabAdded);
            this.Ruler.TabRemoved += new TextRuler.TabChangedEventHandler(this.Ruler_TabRemoved);
            this.Ruler.TabChanged += new TextRuler.TabChangedEventHandler(this.Ruler_TabChanged);

            this.TextEditor.Select(0, 0);
            this.Ruler.LeftIndent = 0;
            this.Ruler.LeftHangingIndent = 0;
            this.Ruler.RightIndent = 0;
            this.TextEditor.SelectionIndent = 0;
            this.TextEditor.SelectionRightIndent = 0;
            this.TextEditor.SelectionHangingIndent = 0;
            this.TextEditor.ScrollBars = RichTextBoxScrollBars.None;            
        }

        private void TextEditor_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            contentRectagle = e.NewRectangle;
            //TextEditor.Height = (TextEditor.GetLineFromCharIndex(TextEditor.Text.Length) + 1) * TextEditor.Font.Height + 1 + TextEditor.Margin.Vertical + 16;
        }

        private void Ruler_TabChanged(TextRuler.TabEventArgs args)
        {
            try
            {
                this.TextEditor.SelectionTabs = this.Ruler.TabPositionsInPixels.ToArray();
            }
            catch (Exception)
            {
            }
        }

        private void Ruler_TabRemoved(TextRuler.TabEventArgs args)
        {
            try
            {
                this.TextEditor.SelectionTabs = this.Ruler.TabPositionsInPixels.ToArray();
            }
            catch (Exception)
            {
            }
        }

        private void Ruler_TabAdded(TextRuler.TabEventArgs args)
        {
            try
            {
                this.TextEditor.SelectionTabs = this.Ruler.TabPositionsInPixels.ToArray();
            }
            catch (Exception)
            {
            }
        }

        private void Ruler_BothLeftIndentsChanged(int LeftIndent, int HangIndent)
        {
            this.TextEditor.SelectionIndent = (int)(this.Ruler.LeftIndent * this.Ruler.DotsPerMillimeter);
            this.TextEditor.SelectionHangingIndent = (int)(this.Ruler.LeftHangingIndent * this.Ruler.DotsPerMillimeter) - (int)(this.Ruler.LeftIndent * this.Ruler.DotsPerMillimeter);
        }

        private void Ruler_RightIndentChanging(int NewValue)
        {
            try
            {
                this.TextEditor.SelectionRightIndent = (int)(this.Ruler.RightIndent * this.Ruler.DotsPerMillimeter);
            }
            catch (Exception)
            {
            }
        }

        private void Ruler_LeftIndentChanging(int NewValue)
        {
            try
            {
                this.TextEditor.SelectionIndent = (int)(this.Ruler.LeftIndent * this.Ruler.DotsPerMillimeter);
                this.TextEditor.SelectionHangingIndent = (int)(this.Ruler.LeftHangingIndent * this.Ruler.DotsPerMillimeter) - (int)(this.Ruler.LeftIndent * this.Ruler.DotsPerMillimeter);
            }
            catch (Exception)
            {
            }

        }

        private void Ruler_LeftHangingIndentChanging(int NewValue)
        {
            try
            {
                this.TextEditor.SelectionHangingIndent = (int)(this.Ruler.LeftHangingIndent * this.Ruler.DotsPerMillimeter) - (int)(this.Ruler.LeftIndent * this.Ruler.DotsPerMillimeter);
            }
            catch (Exception)
            {
            }
        }

        private void TextEditor_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (this.TextEditor.SelectionType == RichTextBoxSelectionTypes.Object ||
                    this.TextEditor.SelectionType == RichTextBoxSelectionTypes.MultiObject)
                {
                    System.Windows.MessageBox.Show(Convert.ToString(this.TextEditor.SelectedObject().sizel.Width));
                }
            }

        }

        private void TextEditor_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
        }

        private void TextEditor_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                #region Alignment
                if (TextEditor.SelectionAlignment == ExtendedRichTextBox.RichTextAlign.Left)
                {
                    DesignerPage.self.btnAlignLeft.IsChecked = true;
                    DesignerPage.self.btnAlignCenter.IsChecked = false;
                    DesignerPage.self.btnAlignRight.IsChecked = false;
                    DesignerPage.self.btnJustify.IsChecked = false;
                }
                else if (TextEditor.SelectionAlignment == ExtendedRichTextBox.RichTextAlign.Center)
                {
                    DesignerPage.self.btnAlignLeft.IsChecked = false;
                    DesignerPage.self.btnAlignCenter.IsChecked = true;
                    DesignerPage.self.btnAlignRight.IsChecked = false;
                    DesignerPage.self.btnJustify.IsChecked = false;
                }
                else if (TextEditor.SelectionAlignment == ExtendedRichTextBox.RichTextAlign.Right)
                {
                    DesignerPage.self.btnAlignLeft.IsChecked = false;
                    DesignerPage.self.btnAlignCenter.IsChecked = false;
                    DesignerPage.self.btnAlignRight.IsChecked = true;
                    DesignerPage.self.btnJustify.IsChecked = false;
                }
                else if (TextEditor.SelectionAlignment == ExtendedRichTextBox.RichTextAlign.Justify)
                {
                    DesignerPage.self.btnAlignLeft.IsChecked = false;
                    DesignerPage.self.btnAlignRight.IsChecked = false;
                    DesignerPage.self.btnAlignCenter.IsChecked = false;
                    DesignerPage.self.btnJustify.IsChecked = true;
                }
                else
                {
                    DesignerPage.self.btnAlignLeft.IsChecked = true;
                    DesignerPage.self.btnAlignCenter.IsChecked = false;
                    DesignerPage.self.btnAlignRight.IsChecked = false;
                }

                #endregion

                #region Tab positions
                this.Ruler.SetTabPositionsInPixels(this.TextEditor.SelectionTabs);
                #endregion

                #region Color
                System.Drawing.Color curColor = this.TextEditor.SelectionColor2;
                System.Windows.Media.Color selColor = System.Windows.Media.Color.FromArgb(255, curColor.R, curColor.G, curColor.B);

                foreach (DesignerPage.ColorInfo cf in DesignerPage.self.cmbColorName.Items)
                {
                    if (cf.Color == selColor)
                    {
                        DesignerPage.self.cmbColorName.SelectedItem = cf;
                        break;
                    }
                }
                #endregion

                #region Font
                try
                {
                    DesignerPage.self.cmbFontSize.Text = Convert.ToInt32(this.TextEditor.SelectionFont2.Size).ToString();
                }
                catch
                {
                    DesignerPage.self.cmbFontSize.Text = "";
                }

                try
                {
                    DesignerPage.self.cmbFontName.Text = this.TextEditor.SelectionFont2.Name;
                }
                catch
                {
                    DesignerPage.self.cmbFontName.Text = "";
                }

                if (DesignerPage.self.cmbFontName.Text != "")
                {
                    System.Drawing.FontFamily ff = new System.Drawing.FontFamily(DesignerPage.self.cmbFontName.Text);
                    if (ff.IsStyleAvailable(System.Drawing.FontStyle.Bold) == true)
                    {
                        DesignerPage.self.btnBold.IsEnabled = true;
                        DesignerPage.self.btnBold.IsChecked = this.TextEditor.SelectionCharStyle.Bold;
                    }
                    else
                    {
                        DesignerPage.self.btnBold.IsEnabled = false;
                        DesignerPage.self.btnBold.IsChecked = false;
                    }

                    if (ff.IsStyleAvailable(System.Drawing.FontStyle.Italic) == true)
                    {
                        DesignerPage.self.btnItalic.IsEnabled = true;
                        DesignerPage.self.btnItalic.IsChecked = this.TextEditor.SelectionCharStyle.Italic;
                    }
                    else
                    {
                        DesignerPage.self.btnItalic.IsEnabled = false;
                        DesignerPage.self.btnItalic.IsChecked = false;
                    }

                    if (ff.IsStyleAvailable(System.Drawing.FontStyle.Underline) == true)
                    {
                        DesignerPage.self.btnUnderline.IsEnabled = true;
                        DesignerPage.self.btnUnderline.IsChecked = this.TextEditor.SelectionCharStyle.Underline;
                    }
                    else
                    {
                        DesignerPage.self.btnUnderline.IsEnabled = false;
                        DesignerPage.self.btnUnderline.IsChecked = false;
                    }

                    if (ff.IsStyleAvailable(System.Drawing.FontStyle.Strikeout) == true)
                    {
                        DesignerPage.self.btnStrikeThrough.IsEnabled = true;
                        DesignerPage.self.btnStrikeThrough.IsChecked = this.TextEditor.SelectionCharStyle.Strikeout;
                    }
                    else
                    {
                        DesignerPage.self.btnStrikeThrough.IsEnabled = false;
                        DesignerPage.self.btnStrikeThrough.IsChecked = false;
                    }

                    ff.Dispose();
                }
                else
                {
                    DesignerPage.self.btnBold.IsChecked = false;
                    DesignerPage.self.btnItalic.IsChecked = false;
                    DesignerPage.self.btnUnderline.IsChecked = false;
                    DesignerPage.self.btnStrikeThrough.IsChecked = false;
                }
                #endregion

                if (this.TextEditor.SelectionLength < this.TextEditor.TextLength - 1)
                {
                    this.Ruler.LeftIndent = (int)(this.TextEditor.SelectionIndent / this.Ruler.DotsPerMillimeter); //convert pixels to millimeter

                    this.Ruler.LeftHangingIndent = (int)((float)this.TextEditor.SelectionHangingIndent / this.Ruler.DotsPerMillimeter) + this.Ruler.LeftIndent; //convert pixels to millimeters

                    this.Ruler.RightIndent = (int)(this.TextEditor.SelectionRightIndent / this.Ruler.DotsPerMillimeter); //convert pixels to millimeters                
                }

                switch (this.TextEditor.SelectionListType.Type)
                {
                    case ExtendedRichTextBox.ParaListStyle.ListType.None:
                        DesignerPage.self.btnNumberedList.IsChecked = false;
                        DesignerPage.self.btnBulletedList.IsChecked = false;
                        break;
                    case ExtendedRichTextBox.ParaListStyle.ListType.SmallLetters:
                        DesignerPage.self.btnNumberedList.IsChecked = false;
                        DesignerPage.self.btnBulletedList.IsChecked = false;
                        break;
                    case ExtendedRichTextBox.ParaListStyle.ListType.CapitalLetters:
                        DesignerPage.self.btnNumberedList.IsChecked = false;
                        DesignerPage.self.btnBulletedList.IsChecked = false;
                        break;
                    case ExtendedRichTextBox.ParaListStyle.ListType.SmallRoman:
                        DesignerPage.self.btnNumberedList.IsChecked = false;
                        DesignerPage.self.btnBulletedList.IsChecked = false;
                        break;
                    case ExtendedRichTextBox.ParaListStyle.ListType.CapitalRoman:
                        DesignerPage.self.btnNumberedList.IsChecked = false;
                        DesignerPage.self.btnBulletedList.IsChecked = false;
                        break;
                    case ExtendedRichTextBox.ParaListStyle.ListType.Bullet:
                        DesignerPage.self.btnNumberedList.IsChecked = false;
                        DesignerPage.self.btnBulletedList.IsChecked = true;
                        break;
                    case ExtendedRichTextBox.ParaListStyle.ListType.Numbers:
                        DesignerPage.self.btnNumberedList.IsChecked = true;
                        DesignerPage.self.btnBulletedList.IsChecked = false;
                        break;
                    case ExtendedRichTextBox.ParaListStyle.ListType.CharBullet:
                        DesignerPage.self.btnNumberedList.IsChecked = true;
                        DesignerPage.self.btnBulletedList.IsChecked = false;
                        break;
                    default:
                        break;
                }


                this.TextEditor.UpdateObjects();
            }
            catch (Exception)
            {
            }
        }

        private void TextEditor_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                Process.Start(e.LinkText);
            }
            catch (Exception)
            {
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.TextEditor.Select(0, 0);
            this.TextEditor.AppendText("some text");
            this.TextEditor.Select(0, 0);
            this.TextEditor.Clear();
            this.TextEditor.SetLayoutType(ExtendedRichTextBox.LayoutModes.WYSIWYG);
        }
    }
}
