using ColorPicker.ColorModels.HSB;
using ColorPicker.ColorModels.Lab;
using ColorPicker.ColorModels.CMYK;
using ColorPicker.ColorModels.RGB;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Designer
{
    /// <summary>
    /// Interaction logic for ColorsPropertiesPanel.xaml
    /// </summary>
    public partial class ColorsPropertiesPanel : UserControl, IXmlSerializable, INotifyPropertyChanged
    {
        private ColorStyle _ColorStyle;
        private string _ColorStyleName;
        private ColorPickerWindow.ColorSpace _ColorSpace = ColorPickerWindow.ColorSpace.RGB;
        private Color _Color = Colors.Black;

        public ColorStyle ColorStyle
        {
            get => _ColorStyle;
            set => _ColorStyle = value;
        }

        public string ColorStyleName
        {
            get
            {
                return _ColorStyleName;
            }
            set
            {
                _ColorStyleName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ColorName"));
            }
        }

        public ColorsPropertiesPanel()
        {
            InitializeComponent();
        }

        public ColorsPropertiesPanel(ColorStyle color)
        {
            ColorStyle = color;

            InitializeComponent();

            ColorStyleName = $"{color.Id} - Colors";
            {
                Binding binding = new Binding("ColorName");
                binding.Source = this;
                binding.Mode = BindingMode.OneWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                lblColorName.SetBinding(Label.ContentProperty, binding);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            byte r = byte.Parse(reader.GetAttribute("ColorR"));
            byte g = byte.Parse(reader.GetAttribute("ColorG"));
            byte b = byte.Parse(reader.GetAttribute("ColorB"));

            _Color = Color.FromRgb(r, g, b);
            _ColorSpace = (ColorPickerWindow.ColorSpace)int.Parse(reader.GetAttribute("ColorSpace"));

            ColorStyle.Color = _Color;

            RefreshProperties();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("ColorR", _Color.R.ToString());
            writer.WriteAttributeString("ColorG", _Color.G.ToString());
            writer.WriteAttributeString("ColorB", _Color.B.ToString());
            writer.WriteAttributeString("ColorSpace", ((int)_ColorSpace).ToString());
        }

        private void cbSpotColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void RefreshProperties()
        {
            lblColor.Background = new SolidColorBrush(_Color);
            lblColorSpace.Content = "ColorSpace: " + _ColorSpace.ToString();

            lblRGB.Content = "RGB: " + _Color.R.ToString() + ", " + _Color.G.ToString() + ", " + _Color.B.ToString();

            HSBModel hsbModel = new HSBModel();
            lblHSB.Content = "HSB: " + Convert.ToInt32(hsbModel.HComponent(_Color)).ToString() + ", " + Convert.ToInt32(hsbModel.SComponent(_Color) * 100).ToString() + ", " + Convert.ToInt32(hsbModel.BComponent(_Color) * 100).ToString();

            LabModel labModel = new LabModel();
            lblLab.Content = "Lab: " + Convert.ToInt32(labModel.LComponent(_Color)).ToString() + ", " + Convert.ToInt32(labModel.AComponent(_Color)).ToString() + ", " + Convert.ToInt32(labModel.BComponent(_Color)).ToString();

            CMYKModel cmykModel = new CMYKModel();
            lblCMYK.Content = "CMYK: " + ((int)(cmykModel.CComponent(_Color) / 255.0 * 100 + 0.5)).ToString() + ", " + ((int)(cmykModel.MComponent(_Color) / 255.0 * 100 + 0.5)).ToString() + ", " + ((int)(cmykModel.YComponent(_Color) / 255.0 * 100 + 0.5)).ToString() + ", " + ((int)(cmykModel.KComponent(_Color) / 255.0 * 100 + 0.5)).ToString();
        }

        void dlg_ApplyChanges(object sender, EventArgs e)
        {
            var dlg = (ColorPickerWindow)sender;
            _Color = dlg.SelectedColor;
            _ColorSpace = dlg.SelectedColorSpace;

            ColorStyle.Color = _Color;

            RefreshProperties();

            dlg.InitialColor = ((SolidColorBrush)lblColor.Background).Color;
        }

        private void btnChange_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ColorPickerWindow dlg = new ColorPickerWindow();
            dlg.InitialColor = _Color;
            dlg.SelectedColor = _Color;
            dlg.SelectedColorSpace = _ColorSpace;
            dlg.ApplyChanges += new EventHandler(dlg_ApplyChanges);

            if (dlg.ShowDialog() == true)
            {
                dlg_ApplyChanges(dlg, null);
            }
        }
    }
}
