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
using System.Xml.Serialization;

namespace Designer
{
    /// <summary>
    /// Interaction logic for TablePropertiesPanel.xaml
    /// </summary>
    public partial class TablePropertiesPanel : UserControl, IXmlSerializable, INotifyPropertyChanged
    {
        private TableStyle _TableStyle;
        private string _TableName;

        public TableStyle TableStyle
        {
            get => _TableStyle;
            set => _TableStyle = value;
        }

        public string TableName
        {
            get => _TableName;
            set
            {
                _TableName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TableName"));
            }
        }

        public TablePropertiesPanel()
        {
            InitializeComponent();
        }

        public TablePropertiesPanel(TableStyle style)
        {
            TableStyle = style;
            InitializeComponent();

            TableName = $"{style.Id} - Tables";
            {
                Binding binding = new Binding("TableName");
                binding.Source = this;
                binding.Mode = BindingMode.OneWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                lblTableName.SetBinding(Label.ContentProperty, binding);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void cbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbRowSet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbTableStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbBorderStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbBorderType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbAlignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        }
    }
}
