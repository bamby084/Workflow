using Designer.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using Designer.Tools;

namespace Designer
{
    /// <summary>
    /// Interaction logic for ElementPropertiesPanel.xaml
    /// </summary>
    public partial class ElementPropertiesPanel : UserControl, IXmlSerializable, INotifyPropertyChanged
    {
        private CanvasElement _Element;
        public CanvasElement Element { get => _Element; set => _Element = value; }

        public ElementPropertiesPanel()
        {
            InitializeComponent();
        }

        public ElementPropertiesPanel(CanvasElement element)
        {
            Element = element;
            Element.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Id")
                {
                    //PageName = $"{Page.Id} - Page";
                }
                else if (e.PropertyName == "ElementWidth")
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CustomWidth"));
                }
                else if (e.PropertyName == "ElementHeight")
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CustomHeight"));
                }
            };

            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public decimal CustomWidth
        {
            get
            {
                return (decimal)(Element.Width / ImageHelper.DPI);
            }

            set
            {
                Element.Width = decimal.ToDouble(value) * ImageHelper.DPI;
                PropertyChanged(this, new PropertyChangedEventArgs("CustomWidth"));
            }
        }

        public decimal CustomHeight
        {
            get
            {
                return (decimal)(Element.Height / ImageHelper.DPI);
            }

            set
            {
                Element.Height = decimal.ToDouble(value) * ImageHelper.DPI;
                PropertyChanged(this, new PropertyChangedEventArgs("CustomHeight"));
            }
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
