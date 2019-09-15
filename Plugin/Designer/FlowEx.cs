using Designer.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Designer
{
    public class FlowEx : PageBase
    {
        /// <summary> Empty constructor for XML Loading.
        /// Do not use for other purposes </summary>
        public FlowEx() : this(-1) { }

        private FlowControl FlowControl;
        public FlowControl Flow { get => FlowControl; }

        public FlowEx(int id) : base(id)
        {
            HidePropertyControls();

            FlowControl = new FlowControl();
            FlowControl.Width = PageWidth;
            FlowControl.Height = PageHeight;

            Canvas.Children.Add(FlowControl);
            Canvas.SetLeft(FlowControl, 0);
            Canvas.SetTop(FlowControl, 0);

            LayoutTransform.Changed += LayoutTransform_Changed;
        }

        private void LayoutTransform_Changed(object sender, EventArgs e)
        {
            FlowControl.Ruler.Height = 20;
        }

        /// <summary>
        /// Constructor used for quick-cloning temporary pages during processing.
        /// </summary>
        /// <param name="pageProperties">The page properties panel.</param>
        public FlowEx(PagePropertiesPanel pageProperties) : this(-1)
        {
            HidePropertyControls();
            _LayoutProperties = pageProperties;

        }

        public override string TreeViewName => "Flows";

        public override void SetId(int id)
        {
            Id = "Flow " + id.ToString();
        }

        private void HidePropertyControls()
        {
            _LayoutProperties.TabGeneral.Visibility = Visibility.Collapsed;
            _LayoutProperties.TabNextPage.Visibility = Visibility.Collapsed;
            _LayoutProperties.TabSheetNames.Visibility = Visibility.Collapsed;
            _LayoutProperties.TabContainer.Visibility = Visibility.Collapsed;

            _LayoutProperties.TabControl.SelectedIndex = 4;
        }

        private void Ruler_TabChanged(TextRuler.TabEventArgs args)
        {
            try
            {
                //this.TextEditor.SelectionTabs = this.Ruler.TabPositionsInPixels.ToArray();
            }
            catch (Exception)
            {
            }
        }

        private void Ruler_TabRemoved(TextRuler.TabEventArgs args)
        {
            try
            {
                //this.TextEditor.SelectionTabs = this.Ruler.TabPositionsInPixels.ToArray();
            }
            catch (Exception)
            {
            }
        }

        private void Ruler_TabAdded(TextRuler.TabEventArgs args)
        {
            try
            {
                //this.TextEditor.SelectionTabs = this.Ruler.TabPositionsInPixels.ToArray();
            }
            catch (Exception)
            {
            }
        }

        private void Ruler_BothLeftIndentsChanged(int LeftIndent, int HangIndent)
        {
            //this.TextEditor.SelectionIndent = (int)(this.Ruler.LeftIndent * this.Ruler.DotsPerMillimeter);
            //this.TextEditor.SelectionHangingIndent = (int)(this.Ruler.LeftHangingIndent * this.Ruler.DotsPerMillimeter) - (int)(this.Ruler.LeftIndent * this.Ruler.DotsPerMillimeter);
        }

        private void Ruler_RightIndentChanging(int NewValue)
        {
            try
            {
                //this.TextEditor.SelectionRightIndent = (int)(this.Ruler.RightIndent * this.Ruler.DotsPerMillimeter);
            }
            catch (Exception)
            {
            }
        }

        private void Ruler_LeftIndentChanging(int NewValue)
        {
            try
            {
                //this.TextEditor.SelectionIndent = (int)(this.Ruler.LeftIndent * this.Ruler.DotsPerMillimeter);
                //this.TextEditor.SelectionHangingIndent = (int)(this.Ruler.LeftHangingIndent * this.Ruler.DotsPerMillimeter) - (int)(this.Ruler.LeftIndent * this.Ruler.DotsPerMillimeter);
            }
            catch (Exception)
            {
            }

        }

        private void Ruler_LeftHangingIndentChanging(int NewValue)
        {
            try
            {
                //this.TextEditor.SelectionHangingIndent = (int)(this.Ruler.LeftHangingIndent * this.Ruler.DotsPerMillimeter) - (int)(this.Ruler.LeftIndent * this.Ruler.DotsPerMillimeter);
            }
            catch (Exception)
            {
            }
        }
    }



    public class FlowExs : ObservableRangeCollection<FlowEx>, IXmlSerializable
    {

        private PagePropertiesPanel layoutProperties;
        public PagePropertiesPanel LayoutProperties
        {
            get
            {
                if (layoutProperties == null)
                {
                    MainWindow window = MainWindow.Instance;
                    layoutProperties = new PagePropertiesPanel(null);
                    layoutProperties.PropertyChanged += LayoutProperties_PropertyChanged;
                }

                return layoutProperties;
            }

            set => layoutProperties = value;
        }

        public FlowExs() : base()
        {
            CollectionChanged += ParagraphStyles_CollectionChanged;
        }

        private void UpdateChildPropertyStates(IList<ContainerEx> children)
        {
            //var state = LayoutProperties.IsPageOrderVariable;
            // foreach (var page in children)
            // {
            //     page.LayoutProperties.TabNextPage.IsEnabled = state;
            // }
        }

        private void ParagraphStyles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    //UpdateChildPropertyStates(e.NewItems.Cast<ParagraphStyle>().ToList());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
            }

        }

        private void LayoutProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "IsPageOrderVariable")
            //{
            //    UpdateChildPropertyStates(this);
            //}
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {

        }
        public void WriteXml(XmlWriter writer)
        {

        }
    }
}
