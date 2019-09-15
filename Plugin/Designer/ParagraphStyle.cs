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
    public class ParagraphStyle : ILayoutNode
    {
 
        public const double INCHES_PER_MM = 0.03937008d;
        public const double MM_PER_INCH = 25.4d;


        private string _Id = "";
        private string _prevId = "";

        protected ParagraphStylesPropertiesPanel _LayoutProperties;
        public string Id
        {
            get => _Id;
            set
            {
                _prevId = _Id;
                _Id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Id"));
            }
        }

        private Action _OnRemove;
        public ParagraphStylesPropertiesPanel LayoutProperties { get => _LayoutProperties; }
        private MenuItem _MenuItem;
        public MenuItem MenuItem => _MenuItem;
        public TvItemCategory TreeViewCategory => TvItemCategory.PARAGRAPH_STYLES;
        public string TreeViewName { get => "ParagraphStyle"; }
        public Action OnRemove { get => _OnRemove; set => _OnRemove = value; }

        public event PropertyChangedEventHandler PropertyChanged;
        public ParagraphStyle() : this(-1) { }

        public ParagraphStyle(int id)
        {
            _MenuItem = new MenuItem();

            SetId(id);
            _LayoutProperties = new ParagraphStylesPropertiesPanel(this);

            _LayoutProperties.PropertyChanged += LayoutProperties_PropertyChanged;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow.Instance.DesignerControl.RequestSetActivePage(this);
        }
    
        public void Dispose()
        {
            OnRemove?.Invoke();
        }

        public FrameworkElement GetPropertyLayout()
        {
            return LayoutProperties;
        }

        public FrameworkElement GetRootElement()
        {
            return null;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Id = reader.GetAttribute("Id");
            _LayoutProperties.ReadXml(reader);
        }

        public void SetId(int id)
        {
            Id = "P" + id.ToString();
        }

  
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Id", Id);
            _LayoutProperties.WriteXml(writer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Id")
            {
                foreach (ParagraphStyle info in DesignerPage.self.cmbParagraphStyle.Items)
                {
                    if (info.Id == _prevId)
                    {
                        info.Id = Id;
                    }
                }
            }
        }

        
    }

    public class ParagraphStyles : ObservableRangeCollection<ParagraphStyle>, IXmlSerializable
    {

        private ParagraphStylesPropertiesPanel layoutProperties;
        public ParagraphStylesPropertiesPanel LayoutProperties
        {
            get
            {
                if (layoutProperties == null)
                {
                    MainWindow window = MainWindow.Instance;
                    layoutProperties = new ParagraphStylesPropertiesPanel();
                    layoutProperties.PropertyChanged += LayoutProperties_PropertyChanged;
                }

                return layoutProperties;
            }

            set => layoutProperties = value;
        }

        public ParagraphStyles() : base()
        {
            CollectionChanged += ParagraphStyles_CollectionChanged;
        }

        private void UpdateChildPropertyStates(IList<ParagraphStyle> children)
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
            writer.WriteStartElement("ParagraphStyles");
            writer.WriteAttributeString("Count", Count.ToString());
            writer.WriteAttributeString("Counter", DesignerPage.self._para_id.ToString());
            foreach (var para in this)
            {
                writer.WriteStartElement(para.GetType().ToString());
                para.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
