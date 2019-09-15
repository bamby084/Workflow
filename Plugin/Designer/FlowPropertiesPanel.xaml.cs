using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Designer
{
    /// <summary>
    /// Interaction logic for FlowPropertiesPanel.xaml
    /// </summary>
    public partial class FlowPropertiesPanel : UserControl, IXmlSerializable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private FlowStyle _FlowStyle;
        private string _FlowhName;

        public double FlowViewWidth { get; set; }
        public bool SectionFlow { get; set; }
        public int LockForDocx { get; set; }
        

        public FlowStyle FlowStyle
        {
            get => _FlowStyle;
            set => _FlowStyle = value;
        }

        public string FlowName
        {
            get => _FlowhName;
            set
            {
                _FlowhName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FlowName"));
            }
        }

        public FlowPropertiesPanel()
        {
            InitializeComponent();
        }

        public FlowPropertiesPanel(FlowStyle flowStyle)
        {
            FlowStyle = flowStyle;
            InitializeComponent();

            FlowName = $"{FlowStyle.Id} - Flows";
            {
                Binding binding = new Binding("FlowName");
                binding.Source = this;
                binding.Mode = BindingMode.OneWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                lblFlowName.SetBinding(Label.ContentProperty, binding);
            }

            GetProperty();
        }

        public void GetProperty()
        {
            FlowViewWidth = (double)UpDownFlowViewWidth.Value.GetValueOrDefault();
            SectionFlow = (bool)chkSectionFlow.IsChecked;
            LockForDocx = cbLockForDocx.SelectedIndex;
        }

        public void RefreshProperty()
        {
            UpDownFlowViewWidth.Value = (Decimal)FlowViewWidth;
            chkSectionFlow.IsChecked = SectionFlow;
            cbLockForDocx.SelectedIndex = LockForDocx;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            {
                FlowViewWidth = double.Parse(reader.GetAttribute("FlowViewWidth"));
                SectionFlow = bool.Parse(reader.GetAttribute("SectionFlow"));
                LockForDocx = int.Parse(reader.GetAttribute("LockForDocx"));
            }

            RefreshProperty();
        }

        public void WriteXml(XmlWriter writer)
        {
            GetProperty();
                        
            writer.WriteAttributeString("FlowViewWidth", FlowViewWidth.ToString());
            writer.WriteAttributeString("SectionFlow", SectionFlow.ToString());
            writer.WriteAttributeString("LockForDocx", LockForDocx.ToString());
        }
    }
}
