using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using osk2.TypographicFonts;

namespace Designer
{
    /// <summary>
    /// Interaction logic for FontPropertiesPanel.xaml
    /// </summary>
    /// 
    public class CustomRowItem
    {
        public string FontName { get; set; }
        public string FontStyle { get; set; }
        public string Weight { get; set; }
        public string SubFamily { get; set; }
        public string Style { get; set; }
        public string Preview { get; set; }
        public string FileName { get; set; }
    }

    public partial class FontPropertiesPanel : UserControl, IXmlSerializable, INotifyPropertyChanged
    {
        private FontStyle _FontStyle;
        private string _FontName;
        private string _FileName;

        public FontStyle FontStyle
        {
            get => _FontStyle;
            set => _FontStyle = value;
        }

        public string FontName
        {
            get
            {
                return _FontName + " - " + Path.GetFileName(_FileName) + " - Font";
            }
            set
            {
                _FontName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FontName"));
            }
        }

        public FontPropertiesPanel()
        {
            InitializeComponent();
        }

        public FontPropertiesPanel(FontStyle style)
        {
            FontStyle = style;
            InitializeComponent();

            FontName = $"{style.Id} - Font";
            {
                Binding binding = new Binding("FontName");
                binding.Source = this;
                binding.Mode = BindingMode.OneWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                lblFontName.SetBinding(Label.ContentProperty, binding);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("FontName", _FontName);
        }

        public void ShowFontProperties(string filename)
        {
            SubFamilyGrid.Items.Clear();

            TypographicFont[] fonts = TypographicFont.FromFile(filename);
            foreach (var subFamily in fonts)
            {
                var styleName = "Nomal";
                var style = "";
                if (subFamily.Bold)
                {
                    styleName = "Bold";
                    style += "B ";
                }

                if (subFamily.Italic)
                {
                    styleName = "Italic";
                    style += "I ";
                }

                if (subFamily.Oblique)
                {
                    styleName = "Oblique";
                    style += "Ob ";
                }

                if (subFamily.Underlined)
                {
                    styleName = "Underlined";
                    style += "U ";
                }

                if (subFamily.Negative)
                {
                    styleName = "Negative";
                    style += "N ";
                }

                if (subFamily.Outlined)
                {
                    styleName = "Outlined";
                    style += "O ";
                }


                if (subFamily.Strikeout)
                {
                    styleName = "Strikeout";
                    style += "S";
                }


                string weightName = "";
                switch (subFamily.Weight)
                {
                    case TypographicFontWeight.Black:
                        weightName = "Black"; break;

                    case TypographicFontWeight.Bold:
                        weightName = "Bold"; break;

                    case TypographicFontWeight.ExtraBold:
                        weightName = "ExtraBold"; break;

                    case TypographicFontWeight.ExtraLight:
                        weightName = "ExtraLight"; break;

                    case TypographicFontWeight.Light:
                        weightName = "Light"; break;

                    case TypographicFontWeight.Medium:
                        weightName = "Medium"; break;

                    case TypographicFontWeight.Normal:
                        weightName = "Normal"; break;

                    case TypographicFontWeight.Semibold:
                        weightName = "Semibold"; break;

                    case TypographicFontWeight.Thin:
                        weightName = "Thin"; break;
                }

                    
                char[] split = new char[] { '\\' };
                string[] items = subFamily.FileName.Split(split);

                var item = new CustomRowItem
                {
                    FontName = subFamily.Family,
                    Weight = weightName,
                    FontStyle = styleName,
                    SubFamily = subFamily.SubFamily,
                    Style = style,
                    Preview = "ABCDEabcde12345!@#$%",
                    FileName = items[items.Length - 1]
                };
                SubFamilyGrid.Items.Add(item);
            }

            SubFamilyGrid.SelectedIndex = -1;
            _FileName = filename;
            if (fonts.Length > 0)
                FontName = fonts[0].Family;
        }

    }
}
