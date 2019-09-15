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
    /// Interaction logic for TextStylePropertiesPanel.xaml
    /// </summary>
    public partial class TextStylePropertiesPanel : UserControl, IXmlSerializable, INotifyPropertyChanged
    {
        private TextStyle _TextStyle;
        private string _TextName;

        public TextStyle TextStyle
        {
            get => _TextStyle;
            set => _TextStyle = value;
        }

        public string TextName
        {
            get => _TextName;
            set
            {
                _TextName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TextName"));
            }
        }

        private int FontName { get; set; }
        private int Subfamily { get; set; }
        private int _FontSize { get; set; }
        private int Bold { get; set; }
        private int Italic { get; set; }
        private int SmallCaps { get; set; }
        private int SuperSubscript { get; set; }
        private double InterSpacing { get; set; }
        private int Kerning { get; set; }
        private int UseFixedWidth { get; set; }
        private double FixedWidth { get; set; }
        private int _Language { get; set; }
        private int HorizontalScale { get; set; }
        private int URLTarget { get; set; }
        private double SuperscirptOffset { get; set; }
        private double SubscriptOffset { get; set; }
        private double SuperSubscriptSize { get; set; }
        private int AffectUnderline { get; set; }
        private int AffectStrikethrough { get; set; }
        private double SmallCapSize { get; set; }
        private int Underline { get; set; }
        private int UnderlineStyle { get; set; }
        private int Strikethrough { get; set; }
        private int StrikethroughLineStyle { get; set; }
        private int CustomUnderlineStrike { get; set; }
        private double UnderlineOffset { get; set; }
        private double UnderlineWidth { get; set; }
        private double StrikethroughOffset { get; set; }
        private double StrikethroughWidth { get; set; }
        private int BorderStyle { get; set; }
        private int ConnectBorders { get; set; }
        private int WithLineGap { get; set; }
        private double LineWidth { get; set; }
        private double Miter { get; set; }
        private int Cap { get; set; }
        private int Join { get; set; }
        private int FillStyle { get; set; }
        private int LineFillStyle { get; set; }
        private int ShadowFillStyle { get; set; }
        private double ShadowOffsetX { get; set; }
        private double ShadowOffsetY { get; set; }




        public TextStylePropertiesPanel()
        {
            InitializeComponent();
        }

        public TextStylePropertiesPanel(TextStyle textStyle)
        {
            TextStyle = textStyle;
            InitializeComponent();

            TextName = $"{textStyle.Id} - Text Styles";
            {
                Binding binding = new Binding("TextName");
                binding.Source = this;
                binding.Mode = BindingMode.OneWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                lblTextName.SetBinding(Label.ContentProperty, binding);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void GetProperty()
        {
            FontName = cbFontName.SelectedIndex;
            Subfamily = cbSubfamily.SelectedIndex;
            _FontSize = cbFontSize.SelectedIndex;
            Bold = cbFontBold.SelectedIndex;
            Italic = cbFontItalic.SelectedIndex;
            SmallCaps = cbSmallCaps.SelectedIndex;
            SuperSubscript = cbSuperSubscript.SelectedIndex;

            InterSpacing = (double)UpDownInterSpacing.Value.GetValueOrDefault();
            Kerning = cbKerning.SelectedIndex;
            UseFixedWidth = cbUseFixedWidth.SelectedIndex;
            FixedWidth = (double)UpDownFixedWidth.Value.GetValueOrDefault();
            _Language = cbLanguage.SelectedIndex;
            HorizontalScale = (int)UpDownHorizontalScale.Value.GetValueOrDefault();

            SuperscirptOffset = (double)UpDownSuperscriptOffset.Value.GetValueOrDefault();
            SubscriptOffset = (double)UpDownSubscriptOffset.Value.GetValueOrDefault();
            SuperSubscriptSize = (double)UpDownSuperSubscriptSize.Value.GetValueOrDefault();
            AffectUnderline = cbSupSubUnderline.SelectedIndex;
            AffectStrikethrough = cbSupSubStrike.SelectedIndex;
            SmallCapSize = (double)UpDownSmallCapsSize.Value.GetValueOrDefault();

            Underline = cbUnderline.SelectedIndex;
            UnderlineStyle = cbUnderlineStyle.SelectedIndex;
            Strikethrough = cbStrikethrough.SelectedIndex;
            StrikethroughLineStyle = cbStrikethroughStyle.SelectedIndex;
            CustomUnderlineStrike = cbCUnderlineStrike.SelectedIndex;
            UnderlineOffset = (double)UpDownUnderlineOffset.Value.GetValueOrDefault();
            UnderlineWidth = (double)UpDownUnderlineWidth.Value.GetValueOrDefault();
            StrikethroughOffset = (double)UpDownStrikeOffset.Value.GetValueOrDefault();
            StrikethroughWidth = (double)UpDownStrikeWidth.Value.GetValueOrDefault();

            BorderStyle = cbBorderStyle.SelectedIndex;
            ConnectBorders = cbConnectBorders.SelectedIndex;
            WithLineGap = cbWidthLineGap.SelectedIndex;

            LineWidth = (double)UpDownLineWidth.Value.GetValueOrDefault();
            Miter = (double)UpDownMiter.Value.GetValueOrDefault();
            Cap = cbCap.SelectedIndex;
            Join = cbJoin.SelectedIndex;
            FillStyle = cbFillStyle.SelectedIndex;
            LineFillStyle = cbLineFillStyle.SelectedIndex;
            ShadowFillStyle = cbShadowFillStyle.SelectedIndex;
            ShadowOffsetX = (double)UpDownShadowOffsetX.Value.GetValueOrDefault();
            ShadowOffsetY = (double)UpDownShadowOffsetY.Value.GetValueOrDefault();
        }

        public void RefreshProperty()
        {
            cbFontName.SelectedIndex = FontName;
            cbSubfamily.SelectedIndex = Subfamily;
            cbFontSize.SelectedIndex = _FontSize;
            cbFontBold.SelectedIndex = Bold;
            cbFontItalic.SelectedIndex = Italic;
            cbSmallCaps.SelectedIndex = SmallCaps;
            cbSuperSubscript.SelectedIndex = SuperSubscript;

            UpDownInterSpacing.Value = (Decimal)InterSpacing;
            cbKerning.SelectedIndex = Kerning;
            cbUseFixedWidth.SelectedIndex = UseFixedWidth;
            UpDownFixedWidth.Value = (Decimal)FixedWidth;
            cbLanguage.SelectedIndex = _Language;
            UpDownHorizontalScale.Value = (Decimal)HorizontalScale;
            cbURLTarget.SelectedIndex = URLTarget;

            UpDownSuperscriptOffset.Value = (Decimal)SuperscirptOffset;
            UpDownSubscriptOffset.Value = (Decimal)SubscriptOffset;
            UpDownSuperSubscriptSize.Value = (Decimal)SuperSubscriptSize;
            cbSupSubUnderline.SelectedIndex = AffectUnderline;
            cbSupSubStrike.SelectedIndex = AffectStrikethrough;
            UpDownSmallCapsSize.Value = (Decimal)SmallCapSize;

            cbUnderline.SelectedIndex = Underline;
            cbUnderlineStyle.SelectedIndex = UnderlineStyle;
            cbStrikethrough.SelectedIndex = Strikethrough;
            cbStrikethroughStyle.SelectedIndex = StrikethroughLineStyle;
            cbCUnderlineStrike.SelectedIndex = CustomUnderlineStrike;
            UpDownUnderlineOffset.Value = (Decimal)UnderlineOffset;
            UpDownUnderlineWidth.Value = (Decimal)UnderlineWidth;
            UpDownStrikeOffset.Value = (Decimal)StrikethroughOffset;
            UpDownStrikeWidth.Value = (Decimal)StrikethroughWidth;

            cbBorderStyle.SelectedIndex = BorderStyle;
            cbConnectBorders.SelectedIndex = ConnectBorders;
            cbWidthLineGap.SelectedIndex = WithLineGap;

            UpDownLineWidth.Value = (Decimal)LineWidth;
            UpDownMiter.Value = (Decimal)Miter;
            cbCap.SelectedIndex = Cap;
            cbJoin.SelectedIndex = Join;
            cbFillStyle.SelectedIndex = FillStyle;
            cbLineFillStyle.SelectedIndex = LineFillStyle;
            cbShadowFillStyle.SelectedIndex = ShadowFillStyle;
            UpDownShadowOffsetX.Value = (Decimal)ShadowOffsetX;
            UpDownShadowOffsetY.Value = (Decimal)ShadowOffsetY;
        }

        private void cbFontName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbSubfamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbFontBold_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbFontItalic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbSmallCaps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbSuperSubscript_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbKerning_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbUseFixedWidth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbURLTarget_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbWrappingRule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbSupSubUnderline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbSupSubStrike_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbUnderline_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbUnderlineStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbStrikethrough_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbStrikethroughStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbCUnderlineStrike_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbBorderStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbConnectBorders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbWidthLineGap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbCap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbJoin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbFillStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbLineFillStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbShadowFillStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            {
                FontName = int.Parse(reader.GetAttribute("FontName"));
                Subfamily = int.Parse(reader.GetAttribute("Subfamily"));
                _FontSize = int.Parse(reader.GetAttribute("FontSize"));
                Bold = int.Parse(reader.GetAttribute("Bold"));
                Italic = int.Parse(reader.GetAttribute("Italic"));
                SmallCaps = int.Parse(reader.GetAttribute("SmallCaps"));
                SuperSubscript = int.Parse(reader.GetAttribute("SuperSubscript"));
            }
            reader.Skip();
            {
                InterSpacing = double.Parse(reader.GetAttribute("InterSpacing"));
                Kerning = int.Parse(reader.GetAttribute("Kerning"));
                UseFixedWidth = int.Parse(reader.GetAttribute("UseFixedWidth"));
                FixedWidth = double.Parse(reader.GetAttribute("FixedWidth"));
                _Language = int.Parse(reader.GetAttribute("Language"));
                HorizontalScale = int.Parse(reader.GetAttribute("HorizontalScale"));
                URLTarget = int.Parse(reader.GetAttribute("URLTarget"));
            }
            reader.Skip();
            {
                SuperscirptOffset = double.Parse(reader.GetAttribute("SupscriptOffset"));
                SubscriptOffset = double.Parse(reader.GetAttribute("SubscriptOffset"));
                SuperSubscriptSize = int.Parse(reader.GetAttribute("SupSubscriptSize"));
                AffectUnderline = int.Parse(reader.GetAttribute("AffectUnderline"));
                AffectStrikethrough = int.Parse(reader.GetAttribute("AffectStrikethrough"));
                SmallCapSize = double.Parse(reader.GetAttribute("SmallCapsSize"));
                
            }
            reader.Skip();
            {
                Underline = int.Parse(reader.GetAttribute("Underline"));
                UnderlineStyle = int.Parse(reader.GetAttribute("UnderlineStyle"));
                Strikethrough = int.Parse(reader.GetAttribute("Strikethrough"));
                StrikethroughLineStyle = int.Parse(reader.GetAttribute("StrikethroughLineStyle"));
                CustomUnderlineStrike = int.Parse(reader.GetAttribute("CustomUnderlineStrike"));
                UnderlineOffset = double.Parse(reader.GetAttribute("UnderlineOffset"));
                UnderlineWidth = double.Parse(reader.GetAttribute("UnderlineWidth"));
                StrikethroughOffset = double.Parse(reader.GetAttribute("StrikethroughOffset"));
                StrikethroughWidth = double.Parse(reader.GetAttribute("StrikethroughWidth"));
            }
            reader.Skip();
            {
                BorderStyle = int.Parse(reader.GetAttribute("BorderStyle"));
                ConnectBorders = int.Parse(reader.GetAttribute("ConnectBorders"));
                WithLineGap = int.Parse(reader.GetAttribute("WithLineGap"));
            }
            reader.Skip();
            {
                LineWidth = double.Parse(reader.GetAttribute("LineWidth"));
                Miter = double.Parse(reader.GetAttribute("Miter"));
                Cap = int.Parse(reader.GetAttribute("Cap"));
                Join = int.Parse(reader.GetAttribute("Join"));
                FillStyle = int.Parse(reader.GetAttribute("FillStyle"));
                LineFillStyle = int.Parse(reader.GetAttribute("LineFillStyle"));
                ShadowFillStyle = int.Parse(reader.GetAttribute("ShadowFillStyle"));
                ShadowOffsetX = double.Parse(reader.GetAttribute("ShadowOffsetX"));
                ShadowOffsetY = double.Parse(reader.GetAttribute("ShadowOffsetY"));
            }
            reader.Skip();

            RefreshProperty();
        }

        public void WriteXml(XmlWriter writer)
        {
            GetProperty();

            writer.WriteStartElement("Font");
            {
                writer.WriteAttributeString("FontName", FontName.ToString());
                writer.WriteAttributeString("Subfamily", Subfamily.ToString());
                writer.WriteAttributeString("FontSize", _FontSize.ToString());
                writer.WriteAttributeString("Bold", Bold.ToString());
                writer.WriteAttributeString("Italic", Italic.ToString());
                writer.WriteAttributeString("SmallCaps", SmallCaps.ToString());
                writer.WriteAttributeString("SuperSubscript", SuperSubscript.ToString());
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Rules");
            {
                writer.WriteAttributeString("InterSpacing", InterSpacing.ToString());
                writer.WriteAttributeString("Kerning", Kerning.ToString());
                writer.WriteAttributeString("UseFixedWidth", UseFixedWidth.ToString());
                writer.WriteAttributeString("FixedWidth", FixedWidth.ToString());
                writer.WriteAttributeString("Language", _Language.ToString());
                writer.WriteAttributeString("HorizontalScale", HorizontalScale.ToString());
                writer.WriteAttributeString("URLTarget", URLTarget.ToString());
            }
            writer.WriteEndElement();

            writer.WriteStartElement("SupSubscript");
            {
                writer.WriteAttributeString("SupscriptOffset", SuperscirptOffset.ToString());
                writer.WriteAttributeString("SubscriptOffset", SubscriptOffset.ToString());
                writer.WriteAttributeString("SupSubscriptSize", SuperSubscriptSize.ToString());
                writer.WriteAttributeString("AffectUnderline", AffectUnderline.ToString());
                writer.WriteAttributeString("AffectStrikethrough", AffectStrikethrough.ToString());
                writer.WriteAttributeString("SmallCapsSize", SmallCapSize.ToString());
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Lines");
            {
                writer.WriteAttributeString("Underline", Underline.ToString());
                writer.WriteAttributeString("UnderlineStyle", UnderlineStyle.ToString());
                writer.WriteAttributeString("Strikethrough", Strikethrough.ToString());
                writer.WriteAttributeString("StrikethroughLineStyle", StrikethroughLineStyle.ToString());
                writer.WriteAttributeString("CustomUnderlineStrike", CustomUnderlineStrike.ToString());
                writer.WriteAttributeString("UnderlineOffset", UnderlineOffset.ToString());
                writer.WriteAttributeString("UnderlineWidth", UnderlineWidth.ToString());
                writer.WriteAttributeString("StrikethroughOffset", StrikethroughOffset.ToString());
                writer.WriteAttributeString("StrikethroughWidth", StrikethroughWidth.ToString());
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Border");
            {
                writer.WriteAttributeString("BorderStyle", BorderStyle.ToString());
                writer.WriteAttributeString("ConnectBorders", ConnectBorders.ToString());
                writer.WriteAttributeString("WithLineGap", WithLineGap.ToString());
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Outline");
            {
                writer.WriteAttributeString("LineWidth", LineWidth.ToString());
                writer.WriteAttributeString("Miter", Miter.ToString());
                writer.WriteAttributeString("Cap", Cap.ToString());
                writer.WriteAttributeString("Join", Join.ToString());
                writer.WriteAttributeString("FillStyle", FillStyle.ToString());
                writer.WriteAttributeString("LineFillStyle", LineFillStyle.ToString());
                writer.WriteAttributeString("ShadowFillStyle", ShadowFillStyle.ToString());
                writer.WriteAttributeString("ShadowOffsetX", ShadowOffsetX.ToString());
                writer.WriteAttributeString("ShadowOffsetY", ShadowOffsetY.ToString());
            }
            writer.WriteEndElement();

        }
    }
}
