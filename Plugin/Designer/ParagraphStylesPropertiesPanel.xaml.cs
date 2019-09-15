using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ParagraphStylesPropertiesPanel.xaml
    /// </summary>
    public partial class ParagraphStylesPropertiesPanel : UserControl, IXmlSerializable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ParagraphStyle _ParagraphStyle;
        private string _ParagraphName;

        public class TabsInfo
        {
            public double Position { get; set; }
            public int Type { get; set; }
            public int Point { get; set; }
            public string Leader { get; set; }

            public TabsInfo(double position, int type, int point, string leader)
            {
                Position = position;
                Type = type;
                Point = point;
                Leader = leader;
            }
        }

        public int Alignment { get; set; }
        public double LeftIndent { get; set; }
        public double RightIndent { get; set; }
        public double FirstLineLeftIndent { get; set; }
        public int SpaceBeforeOnFirst { get; set; }
        public double SpaceBefore { get; set; }
        public double SpaceAfter { get; set; }
        public int LineSpacing { get; set; }
        public double LineSpacingValue { get; set; }       
        public int IgnoreEmptyLines { get; set; }
        public int ReadingOrder { get; set; }
        public int BorderStyle { get; set; }
        public double TabsDefault { get; set; }
        public ObservableCollection<TabsInfo> TabsInfos { get; set; }
        public int UseOutsideTabs { get; set; }


        public ParagraphStyle ParagraphStyle
        {
            get => _ParagraphStyle;
            set => _ParagraphStyle = value;
        }

        public string ParagraphName
        {
            get => _ParagraphName;
            set
            {
                _ParagraphName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ParagraphName"));
            }
        }

        public ParagraphStylesPropertiesPanel()
        {
            InitializeComponent();
        }

        public ParagraphStylesPropertiesPanel(ParagraphStyle paragraphStyle)
        {
            ParagraphStyle = paragraphStyle;
            InitializeComponent();

            ParagraphName = $"{paragraphStyle.Id} - Paragraph Style";
            {
                Binding binding = new Binding("ParagraphName");
                binding.Source = this;
                binding.Mode = BindingMode.OneWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                lblParagraphName.SetBinding(Label.ContentProperty, binding);
            }

            GetProperty();
        }

        public void GetProperty()
        {
            Alignment = cbAlignment.SelectedIndex;
            LeftIndent = (double)UpDownLeftIndent.Value.GetValueOrDefault();
            RightIndent = (double)UpDownRightIndent.Value.GetValueOrDefault();
            FirstLineLeftIndent = (double)UpDownFirstlineLeftIndent.Value.GetValueOrDefault();
            SpaceBeforeOnFirst = (int)cbSpaceBeforeOnFirst.SelectedIndex;
            SpaceBefore = (double)UpDownSpaceBefore.Value.GetValueOrDefault();
            SpaceAfter = (double)UpDownSpaceAfter.Value.GetValueOrDefault();
            LineSpacing = cbLineSpacing.SelectedIndex;
            LineSpacingValue = (double)UpDownLineSpacing.Value.GetValueOrDefault();
            IgnoreEmptyLines = cbIgnoreEmptyLines.SelectedIndex;
            ReadingOrder = cbReadingOrder.SelectedIndex;
            TabsDefault = (double)UpDownTabsDefault.Value.GetValueOrDefault();
        }

        public void RefreshProperty()
        {
            cbAlignment.SelectedIndex = Alignment;
            UpDownLeftIndent.Value = (Decimal)LeftIndent;
            UpDownRightIndent.Value = (Decimal)RightIndent;
            UpDownFirstlineLeftIndent.Value = (Decimal)FirstLineLeftIndent;
            cbSpaceBeforeOnFirst.SelectedIndex = SpaceBeforeOnFirst;
            UpDownSpaceBefore.Value = (Decimal)SpaceBefore;
            UpDownSpaceAfter.Value = (Decimal)SpaceAfter;
            cbLineSpacing.SelectedIndex = LineSpacing;
            UpDownLineSpacing.Value = (Decimal)LineSpacing;
            cbIgnoreEmptyLines.SelectedIndex = IgnoreEmptyLines;
            cbReadingOrder.SelectedIndex = ReadingOrder;
            UpDownTabsDefault.Value = (Decimal)TabsDefault;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            {
                Alignment = int.Parse(reader.GetAttribute("Alignment"));
                LeftIndent = double.Parse(reader.GetAttribute("LeftIndent"));
                RightIndent = double.Parse(reader.GetAttribute("RightIndent"));
                FirstLineLeftIndent = double.Parse(reader.GetAttribute("FirstLineLeftIndent"));
                SpaceBeforeOnFirst = int.Parse(reader.GetAttribute("SpaceBeforeOnFirst"));
                SpaceBefore = double.Parse(reader.GetAttribute("SpaceBefore"));
                SpaceAfter = double.Parse(reader.GetAttribute("SpaceAfter"));
                LineSpacing = int.Parse(reader.GetAttribute("LineSpacing"));
                LineSpacingValue = double.Parse(reader.GetAttribute("LineSpacingValue"));
            }
            reader.Skip();
            {
                IgnoreEmptyLines = int.Parse(reader.GetAttribute("IgnoreEmptyLines"));
                ReadingOrder = int.Parse(reader.GetAttribute("ReadingOrder"));
            }
            reader.Skip();
            {
                BorderStyle = int.Parse(reader.GetAttribute("BorderStyle"));
            }
            reader.Skip();
            {
                TabsDefault = double.Parse(reader.GetAttribute("Default"));
            }
            reader.Skip();

            RefreshProperty();
        }

        public void WriteXml(XmlWriter writer)
        {
            GetProperty();

            writer.WriteStartElement("General");
            writer.WriteAttributeString("Alignment", Alignment.ToString());
            writer.WriteAttributeString("LeftIndent", LeftIndent.ToString());
            writer.WriteAttributeString("RightIndent", RightIndent.ToString());
            writer.WriteAttributeString("FirstLineLeftIndent", FirstLineLeftIndent.ToString());
            writer.WriteAttributeString("SpaceBeforeOnFirst", SpaceBeforeOnFirst.ToString());
            writer.WriteAttributeString("SpaceBefore", SpaceBefore.ToString());
            writer.WriteAttributeString("SpaceAfter", SpaceAfter.ToString());
            writer.WriteAttributeString("LineSpacing", LineSpacing.ToString());
            writer.WriteAttributeString("LineSpacingValue", LineSpacingValue.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Advanced");
            writer.WriteAttributeString("IgnoreEmptyLines", IgnoreEmptyLines.ToString());
            writer.WriteAttributeString("ReadingOrder", ReadingOrder.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Borders");
            writer.WriteAttributeString("BorderStyle", BorderStyle.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Tabs");
            writer.WriteAttributeString("Default", TabsDefault.ToString());
            //writer.WriteAttributeString("UseOutsideTabs", UseOutsideTabs.ToString());
            writer.WriteEndElement();
        }

        private void cbAlignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbSpaceBeforeOnFirst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbLineSpacing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbIgnoreEmptyLines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbReadingOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbBorderStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbFlowBreakBefore_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbFlowBreakAfter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbKeepLinesTogether_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbWrappingType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbTruncateToOneLine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbUseOutsideTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
