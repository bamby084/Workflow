using Designer.Tools;
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
    /// Interaction logic for BlockStylePropertiesPanel.xaml
    /// </summary>
    public partial class BlockStylePropertiesPanel : UserControl, IXmlSerializable, INotifyPropertyChanged
    {
        private BlockStyle _BlockStyle;
        private string _Name;
        public event PropertyChangedEventHandler PropertyChanged;
        private double _Left;
        private double _Top;
        private double _Width, _Height;

        public double Left
        {
            get => _Left;
            set
            {
                _Left = value;
                UpDownLeft.Value = (Decimal)(value / ImageHelper.DPI * Page.MM_PER_INCH);
            }
        }

        public double Top
        {
            get => _Top;
            set
            {
                _Top = value;
                UpDownTop.Value = (Decimal)(value / ImageHelper.DPI * Page.MM_PER_INCH);
            }
        }

        public double Width
        {
            get => _Width;
            set
            {
                _Width = value;
                UpDownWidth.Value = (Decimal)(value / ImageHelper.DPI * Page.MM_PER_INCH);
            }
        }
        public double Height
        {
            get => _Height;
            set
            {
                _Height = value;
                UpDownHeight.Value = (Decimal)(value / ImageHelper.DPI * Page.MM_PER_INCH);
            }
        }

        public BlockStyle BlockStyle
        {
            get => _BlockStyle;
            set => _BlockStyle = value;
        }

        public string BlockName
        {
            get => _Name;
            set
            {
                _Name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BlockName"));
            }
        }

        public BlockStylePropertiesPanel()
        {
            InitializeComponent();
        }

        public BlockStylePropertiesPanel(BlockStyle blockStyle)
        {
            BlockStyle = blockStyle;
            InitializeComponent();

            BlockName = $"{_BlockStyle.Id} - Block Style";
            Binding binding = new Binding("BlockName");
            binding.Source = this;
            binding.Mode = BindingMode.OneWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            lblParagraphName.SetBinding(Label.ContentProperty, binding);
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

        private void UpDownLeft_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            double val = (double)UpDownLeft.Value.GetValueOrDefault();
            _Left = val * ImageHelper.DPI * Page.INCHES_PER_MM;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Left"));
        }

        private void UpDownTop_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            double val = (double)UpDownTop.Value.GetValueOrDefault();
            _Top = val * ImageHelper.DPI * Page.INCHES_PER_MM;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Top"));
        }

        private void UpDownWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            double val = (double)UpDownWidth.Value.GetValueOrDefault();
            _Width = val * ImageHelper.DPI * Page.INCHES_PER_MM;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Width"));
        }

        private void cmbBorderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BorderStyle bs = cmbBorderList.SelectedItem as BorderStyle;
            _BlockStyle.SetBorderStyle(bs);
        }

        private void UpDownHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            double val = (double)UpDownHeight.Value.GetValueOrDefault();
            _Height = val * ImageHelper.DPI * Page.INCHES_PER_MM;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Height"));
        }
    }
}
