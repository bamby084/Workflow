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

namespace Designer
{
    public partial class FontStyle : ILayoutNode
    {

        public const double INCHES_PER_MM = 0.03937008d;
        public const double MM_PER_INCH = 25.4d;


        private string _Id = "";
        private string _FontName = "";
        private string _FileName = "";        
        public string FileName
        {
            get => _FileName;
            set => _FileName = value;
        }

        protected FontPropertiesPanel _LayoutProperties;

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
        public FontPropertiesPanel LayoutProperties { get => _LayoutProperties; }
        private MenuItem _MenuItem;
        public MenuItem MenuItem => _MenuItem;
        public TvItemCategory TreeViewCategory => TvItemCategory.FONTS;
        public string TreeViewName { get => "Fonts"; }
        public Action OnRemove { get => _OnRemove; set => _OnRemove = value; }

        public event PropertyChangedEventHandler PropertyChanged;
        public FontStyle() : this(-1) { }

        public FontStyle(int id)
        {
            _MenuItem = new MenuItem();

            SetId(id);
            _LayoutProperties = new FontPropertiesPanel(this);

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
            _LayoutProperties = new FontPropertiesPanel(this);

            _LayoutProperties.PropertyChanged += LayoutProperties_PropertyChanged;

            Id = reader.GetAttribute("Id");

            _FontName = reader.GetAttribute("FontName");
            _FileName = reader.GetAttribute("FileName");
            _LayoutProperties.ShowFontProperties(_FileName);
        }

        public void SetId(int id)
        {
            Id = "F" + id.ToString();
        }


        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Id", Id);
            writer.WriteAttributeString("FileName", _FileName);
            _LayoutProperties.WriteXml(writer);
        }

        private void LayoutProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public void SetFont(string fileName)
        {
            _FileName = fileName;

            _LayoutProperties.ShowFontProperties(fileName);
        }

        public void Refresh()
        {
            _LayoutProperties.ShowFontProperties(_FileName);
        }
    }

    public class FontStyles : ObservableRangeCollection<FontStyle>, IXmlSerializable
    {

        private FontPropertiesPanel layoutProperties;
        public FontPropertiesPanel LayoutProperties
        {
            get
            {
                if (layoutProperties == null)
                {
                    MainWindow window = MainWindow.Instance;
                    layoutProperties = new FontPropertiesPanel();
                    layoutProperties.PropertyChanged += LayoutProperties_PropertyChanged;
                }

                return layoutProperties;
            }

            set => layoutProperties = value;
        }

        public FontStyles() : base()
        {
            CollectionChanged += FontStyles_CollectionChanged;
        }

        private void UpdateChildPropertyStates(IList<ParagraphStyle> children)
        {
            //var state = LayoutProperties.IsPageOrderVariable;
            // foreach (var page in children)
            // {
            //     page.LayoutProperties.TabNextPage.IsEnabled = state;
            // }
        }

        private void FontStyles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
            writer.WriteStartElement("Fonts");
            writer.WriteAttributeString("Count", Count.ToString());
            writer.WriteAttributeString("Counter", DesignerPage.self._font_id.ToString());
            foreach (var font in this)
            {
                writer.WriteStartElement(font.GetType().ToString());
                font.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public void RefreshAll()
        {
            foreach (var font in this)
            {
                font.Refresh();
            }
        }
    }
}
