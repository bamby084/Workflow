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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using iText;

namespace Designer
{
    public class ImageStyle : ILayoutNode
    {
        public const double INCHES_PER_MM = 0.03937008d;
        public const double MM_PER_INCH = 25.4d;

        private string _ImageName = "";
        private string _Id = "";
        

        protected ImagePropertiesPanel _LayoutProperties;
        public string Id
        {
            get => _Id;
            set
            {
                _Id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Id"));
            }
        }

        private Action _OnRemove;
        public ImagePropertiesPanel LayoutProperties { get => _LayoutProperties; }
        private MenuItem _MenuItem;
        public MenuItem MenuItem => _MenuItem;
        public TvItemCategory TreeViewCategory => TvItemCategory.IMAGES;
        public string TreeViewName { get => "Images"; }
        public Action OnRemove { get => _OnRemove; set => _OnRemove = value; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ImageStyle() : this(-1) { }

        public ImageStyle(int id)
        {
            _MenuItem = new MenuItem();

            SetId(id);
            _LayoutProperties = new ImagePropertiesPanel(this);

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
            _LayoutProperties = new ImagePropertiesPanel(this);

            _LayoutProperties.PropertyChanged += LayoutProperties_PropertyChanged;

            Id = reader.GetAttribute("Id");

            _ImageName = reader.GetAttribute("FileName");
            _LayoutProperties.ShowImageProperties(_ImageName);
            _LayoutProperties.ReadXml(reader);
        }

        public void SetId(int id)
        {
            Id = "IMG" + id.ToString();
        }


        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Id", Id);
            writer.WriteAttributeString("FileName", _ImageName);
            _LayoutProperties.WriteXml(writer);
        }

        private void LayoutProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public void SetImage(string imageName)
        {
            _ImageName = imageName;
            _LayoutProperties.ShowImageProperties(imageName);
        }

    }

    public class ImageStyles : ObservableRangeCollection<ImageStyle>, IXmlSerializable
    {

        private ImagePropertiesPanel layoutProperties;
        public ImagePropertiesPanel LayoutProperties
        {
            get
            {
                if (layoutProperties == null)
                {
                    MainWindow window = MainWindow.Instance;
                    layoutProperties = new ImagePropertiesPanel();
                    layoutProperties.PropertyChanged += LayoutProperties_PropertyChanged;
                }

                return layoutProperties;
            }

            set => layoutProperties = value;
        }

        public ImageStyles() : base()
        {
            CollectionChanged += Images_CollectionChanged;
        }

        private void UpdateChildPropertyStates(IList<ColorStyle> children)
        {
            //var state = LayoutProperties.IsPageOrderVariable;
            // foreach (var page in children)
            // {
            //     page.LayoutProperties.TabNextPage.IsEnabled = state;
            // }
        }

        private void Images_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
            writer.WriteStartElement("Images");
            writer.WriteAttributeString("Count", Count.ToString());
            writer.WriteAttributeString("Counter", DesignerPage.self._image_id.ToString());
            foreach (var image in this)
            {
                writer.WriteStartElement(image.GetType().ToString());
                image.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }

}
