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
    public class TableStyle : ILayoutNode
    {

        public const double INCHES_PER_MM = 0.03937008d;
        public const double MM_PER_INCH = 25.4d;


        private string _Id = "";

        protected TablePropertiesPanel _LayoutProperties;
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
        public TablePropertiesPanel LayoutProperties { get => _LayoutProperties; }
        private MenuItem _MenuItem;
        public MenuItem MenuItem => _MenuItem;
        public TvItemCategory TreeViewCategory => TvItemCategory.TABLES;
        public string TreeViewName { get => "Tables"; }
        public Action OnRemove { get => _OnRemove; set => _OnRemove = value; }

        public event PropertyChangedEventHandler PropertyChanged;
        public TableStyle() : this(-1) { }

        public TableStyle(int id)
        {
            _MenuItem = new MenuItem();

            SetId(id);
            _LayoutProperties = new TablePropertiesPanel(this);

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

        }

        public void SetId(int id)
        {
            Id = "Tbl" + id.ToString();
        }


        public void WriteXml(XmlWriter writer)
        {

        }

        public void WriteXmlCanvasControls(XmlWriter writer)
        {

        }

        private void LayoutProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }


    }

    public class TableStyles : ObservableRangeCollection<TableStyle>, IXmlSerializable
    {

        private TablePropertiesPanel layoutProperties;
        public TablePropertiesPanel LayoutProperties
        {
            get
            {
                if (layoutProperties == null)
                {
                    MainWindow window = MainWindow.Instance;
                    layoutProperties = new TablePropertiesPanel();
                    layoutProperties.PropertyChanged += LayoutProperties_PropertyChanged;
                }

                return layoutProperties;
            }

            set => layoutProperties = value;
        }

        public TableStyles() : base()
        {
            CollectionChanged += TableStyles_CollectionChanged;
        }

        private void UpdateChildPropertyStates(IList<TableStyles> children)
        {
            //var state = LayoutProperties.IsPageOrderVariable;
            // foreach (var page in children)
            // {
            //     page.LayoutProperties.TabNextPage.IsEnabled = state;
            // }
        }

        private void TableStyles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
