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
    public partial class FlowStyle : ILayoutNode
    {

        public const double INCHES_PER_MM = 0.03937008d;
        public const double MM_PER_INCH = 25.4d;

        private FlowControl _Flow = new FlowControl();
        public FlowControl Flow
        {
            get => _Flow;
            set
            {
                _Flow = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FlowStyle"));
            }
        }

        private string _Id = "";

        protected FlowPropertiesPanel _LayoutProperties;
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
        public FlowPropertiesPanel LayoutProperties { get => _LayoutProperties; }
        private MenuItem _MenuItem;
        private BlockStyle BlockStyle;

        public MenuItem MenuItem => _MenuItem;
        public TvItemCategory TreeViewCategory => TvItemCategory.COLORS;
        public string TreeViewName { get => "Colors"; }
        public Action OnRemove { get => _OnRemove; set => _OnRemove = value; }

        public event PropertyChangedEventHandler PropertyChanged;

        public FlowStyle() { }

        public FlowStyle(int id, BlockStyle bs)
        {
            _MenuItem = new MenuItem();

            SetId(id);
            _LayoutProperties = new FlowPropertiesPanel(this);

            PropertyChanged += LayoutProperties_PropertyChanged;

            BlockStyle = bs;

            _Flow.lblTitle.Content = Id;

            bs.Canvas.Children.Add(_Flow);

            double borderWidth = 10;

            Canvas.SetLeft(_Flow, bs.Left + borderWidth);
            Canvas.SetTop(_Flow, bs.Top + borderWidth);

            _Flow.Width = bs.Width - borderWidth * 2;
            _Flow.Height = bs.Height - borderWidth * 2;

            bs.FlowStyles.Add(this);
        }

        public void RefreshFlowControl()
        {
            double borderWidth = 10;

            Flow.Width = BlockStyle.Width - borderWidth * 2;
            Flow.Height = BlockStyle.Height - borderWidth * 2;

            Canvas.SetLeft(Flow, BlockStyle.Left + borderWidth);
            Canvas.SetTop(Flow, BlockStyle.Top + borderWidth);
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
            Id = "Flow" + id.ToString();
        }


        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Id", Id);
            _LayoutProperties.WriteXml(writer);
        }

        private void LayoutProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
           
        }

    }

    public class FlowStyles : ObservableRangeCollection<FlowStyle>, IXmlSerializable
    {

        private FlowPropertiesPanel layoutProperties;
        public FlowPropertiesPanel LayoutProperties
        {
            get
            {
                if (layoutProperties == null)
                {
                    MainWindow window = MainWindow.Instance;
                    layoutProperties = new FlowPropertiesPanel();
                    layoutProperties.PropertyChanged += LayoutProperties_PropertyChanged;
                }

                return layoutProperties;
            }

            set => layoutProperties = value;
        }

        public FlowStyles() : base()
        {
            CollectionChanged += Flow_CollectionChanged;
        }

        private void UpdateChildPropertyStates(IList<ColorStyle> children)
        {
            //var state = LayoutProperties.IsPageOrderVariable;
            // foreach (var page in children)
            // {
            //     page.LayoutProperties.TabNextPage.IsEnabled = state;
            // }
        }

        private void Flow_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
            writer.WriteStartElement("FlowStyles");
            writer.WriteAttributeString("Count", Count.ToString());
            writer.WriteAttributeString("Counter", DesignerPage.self._flow_id.ToString());
            foreach (var flow in this)
            {
                writer.WriteStartElement(flow.GetType().ToString());
                flow.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
