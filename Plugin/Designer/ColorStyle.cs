using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Designer
{
    public class ColorStyle : ILayoutNode
    {

        public const double INCHES_PER_MM = 0.03937008d;
        public const double MM_PER_INCH = 25.4d;

        private Color _prevColor;
        private Color _Color;
        public Color Color
        {
            get => _Color;
            set
            {
                _prevColor = _Color;
                _Color = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Color"));
            }
        }

        private string _Id = "";
        private string _prevId;

        protected ColorsPropertiesPanel _LayoutProperties;
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
        public ColorsPropertiesPanel LayoutProperties { get => _LayoutProperties; }
        private MenuItem _MenuItem;
        public MenuItem MenuItem => _MenuItem;
        public TvItemCategory TreeViewCategory => TvItemCategory.COLORS;
        public string TreeViewName { get => "Colors"; }
        public Action OnRemove { get => _OnRemove; set => _OnRemove = value; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ColorStyle() : this(-1) { }

        public ColorStyle(int id)
        {
            _MenuItem = new MenuItem();

            SetId(id);
            _LayoutProperties = new ColorsPropertiesPanel(this);

            PropertyChanged += LayoutProperties_PropertyChanged;
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
            Id = "C" + id.ToString();
        }


        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Id", Id);
            _LayoutProperties.WriteXml(writer);
        }

        private void LayoutProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Id")
            {
                foreach (DesignerPage.ColorInfo info in DesignerPage.self.cmbColorName.Items)
                {
                    if (info.Name == _prevId && info.IsUser)
                    {
                        info.Name = Id;
                    }
                }
            }

            else if (e.PropertyName == "Color")
            {
                foreach (DesignerPage.ColorInfo info in DesignerPage.self.cmbColorName.Items)
                {
                    if (info.Name == Id && info.IsUser)
                    {
                        info.Color = Color;
                    }
                }
            }

            DesignerPage.self.cmbColorName.Items.Refresh();
        }

    }

    public class ColorStyles : ObservableRangeCollection<ColorStyle>, IXmlSerializable
    {

        private ColorsPropertiesPanel layoutProperties;
        public ColorsPropertiesPanel LayoutProperties
        {
            get
            {
                if (layoutProperties == null)
                {
                    MainWindow window = MainWindow.Instance;
                    layoutProperties = new ColorsPropertiesPanel();
                    layoutProperties.PropertyChanged += LayoutProperties_PropertyChanged;
                }

                return layoutProperties;
            }

            set => layoutProperties = value;
        }

        public ColorStyles() : base()
        {
            CollectionChanged += Colors_CollectionChanged;
        }

        private void UpdateChildPropertyStates(IList<ColorStyle> children)
        {
            //var state = LayoutProperties.IsPageOrderVariable;
            // foreach (var page in children)
            // {
            //     page.LayoutProperties.TabNextPage.IsEnabled = state;
            // }
        }

        private void Colors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
            writer.WriteStartElement("Colors");
            writer.WriteAttributeString("Count", Count.ToString());
            writer.WriteAttributeString("Counter", DesignerPage.self._color_id.ToString());
            foreach (var color in this)
            {
                writer.WriteStartElement(color.GetType().ToString());
                color.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }

}
