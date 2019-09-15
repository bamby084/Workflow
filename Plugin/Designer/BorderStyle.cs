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
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Designer
{
    public class BorderStyle : ILayoutNode
    {

        public const double INCHES_PER_MM = 0.03937008d;
        public const double MM_PER_INCH = 25.4d;


        private string _Id = "";

        protected BorderStylePropertiesPanel _LayoutProperties;
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
        public BorderStylePropertiesPanel LayoutProperties { get => _LayoutProperties; }
        private MenuItem _MenuItem;
        public MenuItem MenuItem => _MenuItem;
        public TvItemCategory TreeViewCategory => TvItemCategory.BORDER_STYLES;
        public string TreeViewName { get => "BorderStyle"; }
        public Action OnRemove { get => _OnRemove; set => _OnRemove = value; }

        public Color BorderColor { get; set; }
        public Decimal BorderWidth { get; set; }
        public PenLineCap Cap { get; set; }
        public string LineStyle { get; set; }
        public string Corner { get; set; }
        public Decimal RadiusX { get; set; }
        public Decimal RadiusY { get; set; }
        public PenLineJoin Join { get; set; }
        public Color FillColor { get; set; }
        public Color ShadingColor { get; set; }
        public Decimal ShadingOffsetX { get; set; }
        public Decimal ShadingOffsetY { get; set; }
        public bool LeftBorderFlag { get; set; }
        public bool TopBorderFlag { get; set; }
        public bool RightBorderFlag { get; set; }
        public bool BottomBorderFlag { get; set; }
        public bool LDiagonalFlag { get; set; }
        public bool RDiagonalFlag { get; set; }
        public bool LTCornerFlag { get; set; }
        public bool RTCornerFlag { get; set; }
        public bool RBCornerFlag { get; set; }
        public bool LBCornerFlag { get; set; }
        public Decimal Mitter { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        public BorderStyle() : this(-1) { }

        public BorderStyle(int id)
        {
            _MenuItem = new MenuItem();

            SetId(id);

            BorderColor = Colors.Black;
            BorderWidth = 2;
            Cap = PenLineCap.Flat;
            LineStyle = "Solid";
            Corner = "Round";
            RadiusX = 10;
            RadiusY = 10;
            Join = PenLineJoin.Miter;
            FillColor = Colors.White;
            ShadingColor = Colors.Gray;
            ShadingOffsetX = 10;
            ShadingOffsetY = 10;
            LeftBorderFlag = true;
            TopBorderFlag = true;
            RightBorderFlag = true;
            BottomBorderFlag = true;
            LDiagonalFlag = false;
            RDiagonalFlag = false;
            LTCornerFlag = true;
            RTCornerFlag = true;
            RBCornerFlag = true;
            LBCornerFlag = true;
            Mitter = 0;

            _LayoutProperties = new BorderStylePropertiesPanel(this);

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
            reader.ReadStartElement();
            {
                LeftBorderFlag = bool.Parse(reader.GetAttribute("LeftBorder"));
                TopBorderFlag = bool.Parse(reader.GetAttribute("TopBorder"));
                RightBorderFlag = bool.Parse(reader.GetAttribute("RightBorder"));
                BottomBorderFlag = bool.Parse(reader.GetAttribute("BottomBorder"));
                LDiagonalFlag = bool.Parse(reader.GetAttribute("LDiagonal"));
                RDiagonalFlag = bool.Parse(reader.GetAttribute("RDiagonal"));
                LTCornerFlag = bool.Parse(reader.GetAttribute("LTCorner"));
                RTCornerFlag = bool.Parse(reader.GetAttribute("RTCorner"));
                LBCornerFlag = bool.Parse(reader.GetAttribute("LBCorner"));
                RBCornerFlag = bool.Parse(reader.GetAttribute("RBCorner"));
            }
            reader.Skip();

            {
                BorderColor = (Color)ColorConverter.ConvertFromString(reader.GetAttribute("BorderColor"));
                BorderWidth = int.Parse(reader.GetAttribute("BorderWidth"));
                string cap = reader.GetAttribute("Cap");
                switch (cap)
                {
                    case "Flat":
                        Cap = PenLineCap.Flat; break;
                    case "Round":
                        Cap = PenLineCap.Round; break;
                    case "Square":
                        Cap = PenLineCap.Square; break;
                    case "Triangle":
                        Cap = PenLineCap.Triangle; break;
                }

                LineStyle = (reader.GetAttribute("LineStyle"));
                Corner = (reader.GetAttribute("Corner"));
                RadiusX = Decimal.Parse(reader.GetAttribute("RadiusX"));
                RadiusY = Decimal.Parse(reader.GetAttribute("RadiusY"));
            }
            reader.Skip();

            {
                string join = reader.GetAttribute("Join");
                switch (join)
                {
                    case "Bevel":
                        Join = PenLineJoin.Bevel; break;
                    case "Round":
                        Join = PenLineJoin.Round; break;
                    case "Miter":
                        Join = PenLineJoin.Miter; break;
                }

                FillColor = (Color)ColorConverter.ConvertFromString(reader.GetAttribute("FillColor"));
                ShadingColor = (Color)ColorConverter.ConvertFromString(reader.GetAttribute("ShadingColor"));
                ShadingOffsetX = Decimal.Parse(reader.GetAttribute("ShadingOffsetX"));
                ShadingOffsetY = Decimal.Parse(reader.GetAttribute("ShadingOffsetY"));
                Mitter = Decimal.Parse(reader.GetAttribute("Mitter"));
            }
            reader.Skip();

            _LayoutProperties.RefreshProperty();
        }

        public void SetId(int id)
        {
            Id = "B" + id.ToString();
        }


        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Id", Id);

            writer.WriteStartElement("ShowProperty");
            {
                writer.WriteAttributeString("LeftBorder", LeftBorderFlag.ToString());
                writer.WriteAttributeString("TopBorder", TopBorderFlag.ToString());
                writer.WriteAttributeString("RightBorder", RightBorderFlag.ToString());
                writer.WriteAttributeString("BottomBorder", BottomBorderFlag.ToString());
                writer.WriteAttributeString("LDiagonal", LDiagonalFlag.ToString());
                writer.WriteAttributeString("RDiagonal", RDiagonalFlag.ToString());
                writer.WriteAttributeString("LTCorner", LTCornerFlag.ToString());
                writer.WriteAttributeString("RTCorner", RTCornerFlag.ToString());
                writer.WriteAttributeString("LBCorner", LBCornerFlag.ToString());
                writer.WriteAttributeString("RBCorner", RBCornerFlag.ToString());
            }
            writer.WriteEndElement();

            writer.WriteStartElement("LinesAndCorner");
            {
                writer.WriteAttributeString("BorderColor", BorderColor.ToString());
                writer.WriteAttributeString("BorderWidth", BorderWidth.ToString());
                writer.WriteAttributeString("Cap", Cap.ToString());
                writer.WriteAttributeString("LineStyle", LineStyle.ToString());
                writer.WriteAttributeString("Corner", Corner.ToString());
                writer.WriteAttributeString("RadiusX", RadiusX.ToString());
                writer.WriteAttributeString("RadiusY", RadiusY.ToString());
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Shading");
            {
                writer.WriteAttributeString("Join", Join.ToString());
                writer.WriteAttributeString("FillColor", FillColor.ToString());
                writer.WriteAttributeString("ShadingColor", ShadingColor.ToString());
                writer.WriteAttributeString("ShadingOffsetX", ShadingOffsetX.ToString());
                writer.WriteAttributeString("ShadingOffsetY", ShadingOffsetY.ToString());
                writer.WriteAttributeString("Mitter", Mitter.ToString());
            }
            writer.WriteEndElement();

        }

        public void WriteXmlCanvasControls(XmlWriter writer)
        {

        }

        private void LayoutProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
                            new Action<BorderStylePropertiesPanel>((c) => {
                                _LayoutProperties.drawPreviewRectangle();
                            }),
                                        System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                                        _LayoutProperties
                                );

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BorderStyle"));
        }


    }

    public class BorderStyles : ObservableRangeCollection<BorderStyle>, IXmlSerializable
    {

        private BorderStylePropertiesPanel layoutProperties;
        public BorderStylePropertiesPanel LayoutProperties
        {
            get
            {
                if (layoutProperties == null)
                {
                    MainWindow window = MainWindow.Instance;
                    layoutProperties = new BorderStylePropertiesPanel();
                    layoutProperties.PropertyChanged += LayoutProperties_PropertyChanged;
                }

                return layoutProperties;
            }

            set => layoutProperties = value;
        }

        public BorderStyles() : base()
        {
            CollectionChanged += BorderStyles_CollectionChanged;
        }

        private void UpdateChildPropertyStates(IList<ParagraphStyle> children)
        {
            //var state = LayoutProperties.IsPageOrderVariable;
            // foreach (var page in children)
            // {
            //     page.LayoutProperties.TabNextPage.IsEnabled = state;
            // }
        }

        private void BorderStyles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        public void LayoutProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
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
            writer.WriteStartElement("Borders");
            writer.WriteAttributeString("Count", Count.ToString());
            writer.WriteAttributeString("Counter", DesignerPage.self._border_id.ToString());
            foreach (var border in this)
            {
                writer.WriteStartElement(border.GetType().ToString());
                border.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
