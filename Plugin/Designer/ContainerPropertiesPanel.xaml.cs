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
using System.Xml;
using System.Xml.Schema;

namespace Designer
{
    /// <summary>
    /// Interaction logic for ContainerPropertiesPanel.xaml
    /// </summary>
    public partial class ContainerPropertiesPanel : UserControl
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private PageBase _Container;
        private string _ContainerName;

        public PageBase Container
        {
            get => _Container;
            set => _Container = value;
        }

        public string ContainerName
        {
            get
            {
                return _ContainerName;
            }
            set
            {
                _ContainerName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ContainerName"));
            }
        }

        public ContainerPropertiesPanel()
        {
            InitializeComponent();
        }

        public ContainerPropertiesPanel(PageBase paragraphStyle)
        {
            Container = paragraphStyle;

            InitializeComponent();

            ContainerName = $"{paragraphStyle.Id} - Container";
            {
                Binding binding = new Binding("ContainerName");
                binding.Source = this;
                binding.Mode = BindingMode.OneWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                lblParagraph.SetBinding(Label.ContentProperty, binding);
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
