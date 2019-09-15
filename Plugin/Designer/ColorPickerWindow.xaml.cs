using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using ColorPicker;
namespace Designer
{
    /// <summary>
    /// Interaction logic for ColorPickerWindow.xaml
    /// </summary>
    public partial class ColorPickerWindow : Window
    {
        public enum ColorSpace
        {
            RGB,
            HSB,
            Lab,
            CMYK
        }

        public static Type ClassType
        {
            get { return typeof(ColorPickerWindow); }
        }

        #region InitialColor

        public static DependencyProperty InitialColorProperty = DependencyProperty.Register("InitialColor", typeof(Color), ClassType,
            new FrameworkPropertyMetadata(Colors.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnInitialColorChanged));
        [Category("ColorPicker")]
        public Color InitialColor
        {
            get
            {
                return (Color)GetValue(InitialColorProperty);
            }
            set
            {
                SetValue(InitialColorProperty, value);
            }
        }


        private static void OnInitialColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cpf = (ColorPickerWindow)d;
            cpf.newCurrent.CurrentColor = (Color)e.NewValue;

        }


        #endregion

        public event EventHandler<EventArgs<Color>> SelectedColorChanged;
        #region SelectedColor

        public static DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), ClassType,
            new FrameworkPropertyMetadata(Colors.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorChanged));
        [Category("ColorPicker")]
        public Color SelectedColor
        {
            get
            {
                return (Color)GetValue(SelectedColorProperty);
            }
            set
            {
                SetValue(SelectedColorProperty, value);
            }
        }


        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cpf = (ColorPickerWindow)d;
            cpf.colorSelector.Color = (Color)e.NewValue;
            if (cpf.SelectedColorChanged != null)
            {
                cpf.SelectedColorChanged(cpf, new EventArgs<Color>((Color)e.NewValue));
            }

        }


        #endregion


        [Category("ColorPicker")]
        public ColorSelector.ESelectionRingMode SelectionRingMode
        {
            get { return colorSelector.SelectionRingMode; }
            set { colorSelector.SelectionRingMode = value; }
        }

        private ColorSpace _ColorSpace;

        public ColorSpace SelectedColorSpace
        {
            get
            {
                return _ColorSpace;
            }

            set
            {
                _ColorSpace = value;
                cmbColorSpace.SelectedIndex = (int)_ColorSpace;
            }
        }

        public string GetColorSpaceName()
        {
            return _ColorSpace.ToString();
        }

        public ColorPickerWindow()
        {
            InitializeComponent();

            SetBinding(SelectedColorProperty, "Color");
            DataContext = colorSelector;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public event EventHandler ApplyChanges;
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            var handler = ApplyChanges;
            if (handler != null) handler(this, e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _ColorSpace = (ColorSpace)cmbColorSpace.SelectedIndex;
        }
    }
}
