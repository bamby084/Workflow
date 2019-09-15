//using Designer.Common;
using Designer.Controls;
using Designer.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Designer
{
    public class ContainerEx : PageBase
    {
        /// <summary> Empty constructor for XML Loading.
        /// Do not use for other purposes </summary>
        public ContainerEx() : this(-1) { }

        public ContainerEx(int id) : base(id)
        {
            HidePropertyControls();
            PageWidth = 30 * INCHES_PER_MM * ImageHelper.DPI;
            PageHeight = 30 * INCHES_PER_MM * ImageHelper.DPI;
        }

        /// <summary>
        /// Constructor used for quick-cloning temporary pages during processing.
        /// </summary>
        /// <param name="pageProperties">The page properties panel.</param>
        public ContainerEx(PagePropertiesPanel pageProperties) : this(-1)
        {
            _LayoutProperties = pageProperties;
            HidePropertyControls();
            PageWidth = 30 * INCHES_PER_MM * ImageHelper.DPI;
            PageHeight = 30 * INCHES_PER_MM * ImageHelper.DPI;
        }

        public override string TreeViewName => "Containers";

        private void HidePropertyControls()
        {
            _LayoutProperties.TabGeneral.Visibility = Visibility.Collapsed;
            _LayoutProperties.TabNextPage.Visibility = Visibility.Collapsed;
            _LayoutProperties.TabSheetNames.Visibility = Visibility.Collapsed;

            _LayoutProperties.TabControl.SelectedIndex = 3;
        }

        public override void SetId(int id)
        {
            Id = "Con " + id.ToString();
        }
    }

   

    public class Containers : ObservableRangeCollection<ContainerEx>, IXmlSerializable
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

        public Containers() : base()
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
