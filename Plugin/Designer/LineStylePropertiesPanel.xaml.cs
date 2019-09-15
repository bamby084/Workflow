using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Designer
{
    /// <summary>
    /// Interaction logic for LineStylePropertiesPanel.xaml
    /// </summary>
    public partial class LineStylePropertiesPanel : UserControl, IXmlSerializable, INotifyPropertyChanged
    {
        private LineStyle _LineStyle;
        private string _LineName;

        public LineStyle LineStyle
        {
            get => _LineStyle;
            set => _LineStyle = value;
        }

        public string LineName
        {
            get => _LineName;
            set
            {
                _LineName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LineName"));
            }
        }

        public LineStylePropertiesPanel()
        {
            InitializeComponent();
        }

        public LineStylePropertiesPanel(LineStyle style)
        {
            LineStyle = style;
            InitializeComponent();

            LineName = $"{style.Id} - Line Styles";
            {
                Binding binding = new Binding("LineName");
                binding.Source = this;
                binding.Mode = BindingMode.OneWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                lblBorderName.SetBinding(Label.ContentProperty, binding);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;


        private void drawPreviewLine()
        {
            canvas.Children.Clear();

            double canvasHeight = canvas.ActualHeight;
            double canvasWidth = canvas.ActualWidth;

            Line linePreview = new Line();

            linePreview.X1 = 30;
            linePreview.Y1 = canvasHeight / 2;
            linePreview.X2 = canvasWidth - 30;
            linePreview.Y2 = canvasHeight / 2;
            linePreview.StrokeThickness = 2;
            linePreview.Stroke = new SolidColorBrush(Colors.Black);

            switch (_LineStyle.lineStyle.ToLower())
            {
                case "solid":
                    linePreview.StrokeDashArray = new DoubleCollection();
                    break;
                case "dot":
                    linePreview.StrokeDashArray = new DoubleCollection() { 4, 2 };
                    break;
                case "dashdot":
                    linePreview.StrokeDashArray = new DoubleCollection() { 4, 2, 1, 2 };
                    break;
                case "dashdotdot":
                    linePreview.StrokeDashArray = new DoubleCollection() { 4, 2, 1, 2, 1, 2 };
                    break;
                default:
                    linePreview.StrokeDashArray = new DoubleCollection();
                    break;
            }

            canvas.Children.Add(linePreview);
            Canvas.SetLeft(linePreview, (double)_LineStyle.Offset);
        }

        #region eventhandlers

        private void cmbLineStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int nSelectedIndex = cmbDashStyle.SelectedIndex;

            switch (nSelectedIndex)
            {
                case 0:
                    _LineStyle.lineStyle = "Solid";
                    break;
                case 1:
                    _LineStyle.lineStyle = "Dot";
                    break;
                case 2:
                    _LineStyle.lineStyle = "DashDot";
                    break;
                case 3:
                    _LineStyle.lineStyle = "DashDotDot";
                    break;
            }
            // Pass event to Main window
            OnCmbLineStyleSelectionChanged(e);
        }

        private void numerLineOffset_ValueChanged(object sender, EventArgs e)
        {
            _LineStyle.Offset = numerOffset.Value;
            // Pass event to Main window
            OnNumerLineOffsetValueChanged(e);
        }

        protected virtual void OnCmbLineStyleSelectionChanged(SelectionChangedEventArgs e)
        {
            var handler = CmbLineStyleSelectionChanged;
            if (handler != null)
                handler(this, e);

            drawPreviewLine();
        }

        protected virtual void OnNumerLineOffsetValueChanged(EventArgs e)
        {
            var handler = NumerLineOffsetValueChanged;
            if (handler != null)
                handler(this, e);

            drawPreviewLine();
        }

        public void RefreshProperty()
        {
            numerOffset.Value = _LineStyle.Offset;

            string dashStyle = _LineStyle.lineStyle;
            int selectedIndex = 0;
            switch (dashStyle)
            {
                case "Solid":
                    selectedIndex = 0;
                    break;
                case "Dot":
                    selectedIndex = 1;
                    break;
                case "DashDot":
                    selectedIndex = 2;
                    break;
                case "DashDotDot":
                   selectedIndex = 3;
                    break;
            }

            cmbDashStyle.SelectedIndex = selectedIndex;
        }

        #endregion

        // Event delegates
        public event EventHandler<SelectionChangedEventArgs> CmbLineStyleSelectionChanged;
        public event EventHandler<EventArgs> NumerLineOffsetValueChanged;

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

        private void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            drawPreviewLine();
        }
    }
}
