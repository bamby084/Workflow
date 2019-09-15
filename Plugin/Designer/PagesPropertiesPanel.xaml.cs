using JdSuite.Common.Logging;
using JdSuite.Common.Logging.Enums;
using JdSuite.Common.Module;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Designer
{
    /// <summary>
    /// Interaction logic for PagesPropertiesPanel.xaml
    /// </summary>
    public partial class PagesPropertiesPanel : UserControl, IXmlSerializable, INotifyPropertyChanged
    {
        private bool _IsPageOrderVariable = false;
        private Field Schema;
        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsPageOrderVariable
        {
            get => _IsPageOrderVariable;
            set
            {
                _IsPageOrderVariable = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsPageOrderVariable"));
            }
        }
        public PagesPropertiesPanel()
        {
            Schema = MainWindow.Instance.Schema;
            InitializeComponent();
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            var isPoVar = reader.GetAttribute("isPageOrderVariable");
            bool poVarResult = false;
            if (!bool.TryParse(isPoVar, out poVarResult))
            {
                //test for other attributes to determine
                Logger.Log(Severity.ERROR, LogCategory.CONTROL,
                    "PagesPropertiesPanel deserialization: Parsing isPageOrderVariable failed."+
                    "Possible corruption detected.");
            }
            IsPageOrderVariable = poVarResult;
            if (!IsPageOrderVariable)
            {
                CbPageOrder.SelectedIndex = 0;
                return;
            }

            var sts = XmlConvert.DecodeName(reader.GetAttribute("SelectedType"));
            foreach (ComboBoxItem cbi in CbTypeSelector.Items)
            {
                if (cbi.Content as string == sts)
                {
                    CbTypeSelector.SelectedItem = cbi;
                    break;
                }
            }

            reader.ReadStartElement();
            VariableDataGrid.ReadXml(reader);
            reader.ReadEndElement();
            CbPageOrder.SelectedIndex = 1;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("isPageOrderVariable", IsPageOrderVariable.ToString());
            if (!IsPageOrderVariable)
                return;

            var sts = (string)((ComboBoxItem)CbTypeSelector.SelectedValue).Content;
            writer.WriteAttributeString("SelectedType", XmlConvert.EncodeName(sts));
            writer.WriteStartElement("VariableDataGrid");
            VariableDataGrid.WriteXml(writer);
            writer.WriteEndElement();
        }

        private void CbTypeInteger_Selected(object sender, RoutedEventArgs e)
        {
            VariableDataGrid.LoadVariables(Schema, VariableDataGrid.VariableType.INTEGER);
        }

        private void CbTypeText_Selected(object sender, RoutedEventArgs e)
        {
            VariableDataGrid.LoadVariables(Schema, VariableDataGrid.VariableType.STRING);
        }

        private void PageOrderSimple_Selected(object sender, RoutedEventArgs e)
        {
            GridVariable.Visibility = Visibility.Collapsed;
            GridSimple.Visibility = Visibility.Visible;
            IsPageOrderVariable = false;
        }

        private void PageOrderVariable_Selected(object sender, RoutedEventArgs e)
        {
            GridVariable.Visibility = Visibility.Visible;
            GridSimple.Visibility = Visibility.Collapsed;
            IsPageOrderVariable = true;
        }
    }
}
 
