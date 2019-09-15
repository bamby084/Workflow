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
    /// Interaction logic for BorderStylePropertiesPanel.xaml
    /// </summary>
    public partial class BorderStylePropertiesPanel : UserControl, IXmlSerializable, INotifyPropertyChanged
    {
        private BorderStyle _BorderStyle;
        private string _BorderName;

        public BorderStyle BorderStyle
        {
            get => _BorderStyle;
            set => _BorderStyle = value;
        }

        public string BorderName
        {
            get => _BorderName;
            set
            {
                _BorderName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BorderName"));
            }
        }

        public BorderStylePropertiesPanel()
        {
            InitializeComponent();
        }

        public BorderStylePropertiesPanel(BorderStyle style)
        {
            BorderStyle = style;
            InitializeComponent();

            BorderName = $"{style.Id} - Border Styles";
            {
                Binding binding = new Binding("BorderName");
                binding.Source = this;
                binding.Mode = BindingMode.OneWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                lblBorderName.SetBinding(Label.ContentProperty, binding);
            }

            //drawPreviewRectangle();
        }

        public void RefreshProperty()
        {
            pickerBorderColor.SelectedColor = new SolidColorBrush(_BorderStyle.BorderColor);

            numerWidth.Value = _BorderStyle.BorderWidth;

            cmbCap.SelectedIndex = (int)_BorderStyle.Cap;

            int selectedIndex = 0;
            switch(_BorderStyle.LineStyle)
            {
                case "Solid":
                    selectedIndex = 0; break;
                case "Dot":
                    selectedIndex = 1; break;
                case "DashDot":
                    selectedIndex = 2; break;
                case "DashDotDot":
                    selectedIndex = 3; break;
            }
            cmbBorderStyle.SelectedIndex = selectedIndex;

            selectedIndex = 0;
            switch (_BorderStyle.Corner)
            {
                case "Standard":
                    selectedIndex = 0; break;
                case "Round":
                    selectedIndex = 1; break;
                case "Round Out":
                    selectedIndex = 2; break;
                case "Cut Out":
                    selectedIndex = 3; break;
            }
            cmbCorner.SelectedIndex = selectedIndex;
            
            
            numerRadiusX.Value = _BorderStyle.RadiusX;
            numerRadiusY.Value = _BorderStyle.RadiusY;
            cmbJoin.SelectedIndex = (int)_BorderStyle.Join;

            pickerFill.SelectedColor = new SolidColorBrush( _BorderStyle.FillColor);
            pickerShadingFill.SelectedColor = new SolidColorBrush(_BorderStyle.ShadingColor);

            numerMiter.Value = _BorderStyle.Mitter;
            numerOffsetX.Value = _BorderStyle.ShadingOffsetX;
            numerOffsetY.Value = _BorderStyle.ShadingOffsetY;

            btnLeftBorder.IsChecked = _BorderStyle.LeftBorderFlag;
            btnTopBorder.IsChecked = _BorderStyle.TopBorderFlag;
            btnRightBorder.IsChecked = _BorderStyle.RightBorderFlag;
            btnBottomBorder.IsChecked = _BorderStyle.BottomBorderFlag;
            btnLDigonal.IsChecked = _BorderStyle.LDiagonalFlag;
            btnRDiagonal.IsChecked = _BorderStyle.RDiagonalFlag;
            btnLTCorner.IsChecked = _BorderStyle.LTCornerFlag;
            btnRTCorner.IsChecked = _BorderStyle.RTCornerFlag;
            btnRBCorner.IsChecked = _BorderStyle.RBCornerFlag;
            btnLBCorner.IsChecked = _BorderStyle.LBCornerFlag;
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
        }

        #region event handler

        private void txtBorderColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            Color colorBorderColor = (Color)ColorConverter.ConvertFromString(txtBorderColor.Text);
            _BorderStyle.BorderColor = colorBorderColor;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BorderColor"));
        }

        private void numerBorderWidth_ValueChanged(object sender, EventArgs e)
        {
            // Pass event to Main Window
            _BorderStyle.BorderWidth = numerWidth.Value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BorderWidth"));
        }

        private void cmbCap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int nSelectedIndex = cmbCap.SelectedIndex;

            switch (nSelectedIndex)
            {
                case 0:
                    _BorderStyle.Cap = PenLineCap.Flat;
                    break;
                case 1:
                    _BorderStyle.Cap = PenLineCap.Round;
                    break;
                case 2:
                    _BorderStyle.Cap = PenLineCap.Square;
                    break;
            }
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Cap"));
        }

        private void cmbLineStyle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int nSelectedIndex = cmbBorderStyle.SelectedIndex;

            switch (nSelectedIndex)
            {
                case 0:
                    _BorderStyle.LineStyle = "Solid";
                    break;
                case 1:
                    _BorderStyle.LineStyle = "Dot";
                    break;
                case 2:
                    _BorderStyle.LineStyle = "DashDot";
                    break;
                case 3:
                    _BorderStyle.LineStyle = "DashDotDot";
                    break;
            }
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LineStyle"));
        }

        private void cmbCorner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int nSelectedIndex = cmbCorner.SelectedIndex;

            switch (nSelectedIndex)
            {
                case 0:
                    _BorderStyle.Corner = "Standard";
                    break;
                case 1:
                    _BorderStyle.Corner = "Round";
                    break;
                case 2:
                    _BorderStyle.Corner = "Round Out";
                    break;
                case 3:
                    _BorderStyle.Corner = "Cut Out";
                    break;
            }
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Corner"));
        }

        private void numerRadiusX_ValueChanged(object sender, EventArgs e)
        {
            _BorderStyle.RadiusX = numerRadiusX.Value;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RadiusX"));
        }

        private void numerRadiusYTextBox_ValueChanged(object sender, EventArgs e)
        {
            _BorderStyle.RadiusY = numerRadiusY.Value;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RadiusY"));
        }

        private void cmbJoin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int nSelectedIndex = cmbJoin.SelectedIndex;

            switch (nSelectedIndex)
            {
                case 0:
                    _BorderStyle.Join = PenLineJoin.Miter;
                    break;
                case 1:
                    _BorderStyle.Join = PenLineJoin.Round;
                    break;
                case 2:
                    _BorderStyle.Join = PenLineJoin.Bevel;
                    break;
            }
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Join"));
        }

        private void numerMiter_ValueChanged(object sender, EventArgs e)
        {

            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Miter"));
        }


        private void txtFill_TextChanged(object sender, TextChangedEventArgs e)
        {
            Color colorFillColor = (Color)ColorConverter.ConvertFromString(txtFill.Text);
            _BorderStyle.FillColor = colorFillColor;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FillColor"));
        }

        private void txtShadingFill_TextChanged(object sender, TextChangedEventArgs e)
        {
            Color colorShadingColor = (Color)ColorConverter.ConvertFromString(txtShadingFill.Text);
            _BorderStyle.ShadingColor = colorShadingColor;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShadingColor"));
        }

        private void numerOffsetX_ValueChanged(object sender, EventArgs e)
        {
            _BorderStyle.ShadingOffsetX = numerOffsetX.Value;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShadingOffsetX"));
        }

        private void numerOffsetY_ValueChanged(object sender, EventArgs e)
        {
            _BorderStyle.ShadingOffsetY = numerOffsetY.Value;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ShadingOffsetY"));
        }

        private void btnLeftBorder_Checked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.LeftBorderFlag = true;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LeftBorder"));
        }

        private void btnLeftBorder_Unchecked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.LeftBorderFlag = false;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LeftBorder"));
        }

        private void btnTopBorder_Checked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.TopBorderFlag = true;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TopBorder"));
        }

        private void btnTopBorder_Unchecked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.TopBorderFlag = false;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TopBorder"));
        }

        private void btnRightBorder_Checked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.RightBorderFlag = true;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RightBorder"));
        }

        private void btnRightBorder_Unchecked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.RightBorderFlag = false;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RighBorder"));
        }

        private void btnBottomBorder_Checked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.BottomBorderFlag = true;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BottomBorder"));
        }

        private void btnBottomBorder_Unchecked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.BottomBorderFlag = false;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BottomBorder"));
        }

        private void btnLDiagonal_Checked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.LDiagonalFlag = true;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LDiagonal"));
        }

        private void btnLDiagonal_Unchecked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.LDiagonalFlag = false;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LDiagonal"));
        }

        private void btnRDiagonal_Checked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.RDiagonalFlag = true;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RDiagonal"));
        }

        private void btnRDiagonal_Unchecked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.RDiagonalFlag = false;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RDiagonal"));
        }

        private void btnLTCorner_Checked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.LTCornerFlag = true;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LTCorner"));
        }

        private void btnLTCorner_Unchecked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.LTCornerFlag = false;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LTCorner"));
        }

        private void btnRTCorner_Checked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.RTCornerFlag = true;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RTCorner"));
        }

        private void btnRTCorner_Unchecked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.RTCornerFlag = false;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RTCorner"));
        }

        private void btnRBCorner_Checked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.RBCornerFlag = true;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RBCorner"));
        }

        private void btnRBCorner_Unchecked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.RBCornerFlag = false;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RBCorner"));
        }

        private void btnLBCorner_Checked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.LBCornerFlag = true;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LBCorner"));
        }

        private void btnLBCorner_Unchecked(object sender, RoutedEventArgs e)
        {
            _BorderStyle.LBCornerFlag = false;
            // Pass event to Main Window
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LBCorner"));
        }

        #endregion

        private void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            drawPreviewRectangle();
        }


        public void drawPreviewRectangle()
        {
            canvas.Children.Clear();

            double canvasHeight = canvas.ActualHeight;
            double canvasWidth = canvas.ActualWidth;

            double rectHeight = canvasHeight * 0.7;
            double rectWidth = canvasWidth - 20;

            Color borderColor = _BorderStyle.BorderColor;
            Decimal borderWidth = _BorderStyle.BorderWidth;
            PenLineCap cap = _BorderStyle.Cap;
            string lineStyle = _BorderStyle.LineStyle;
            string corner = _BorderStyle.Corner;
            double radiusX = (double)_BorderStyle.RadiusX;
            double radiusY = (double)_BorderStyle.RadiusY;
            PenLineJoin join = _BorderStyle.Join;
            Color fillColor = _BorderStyle.FillColor;
            Color shadingFillColor = _BorderStyle.ShadingColor;
            double shadingOffsetX = (double)_BorderStyle.ShadingOffsetX;
            double shadingOffsetY = (double)_BorderStyle.ShadingOffsetY;


            #region shading

            PathFigure pathShadingFigure = new PathFigure();
            pathShadingFigure.StartPoint = new System.Windows.Point(0, 0 + radiusY);
            pathShadingFigure.IsClosed = true;

            PathSegment arcShadingLTSegment;
            switch (corner.ToLower())
            {
                case "standard":
                    arcShadingLTSegment = new PolyLineSegment(new PointCollection() { new Point(0, 0), new Point(radiusX, 0), new Point(0, 0) }, true);
                    break;
                case "round":
                    arcShadingLTSegment = new ArcSegment(new Point(radiusX, 0), new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true);
                    break;
                case "round out":
                    arcShadingLTSegment = new ArcSegment(new Point(radiusX, 0), new Size(radiusX, radiusY), 0, false, SweepDirection.Counterclockwise, true);
                    break;
                case "cut out":
                    arcShadingLTSegment = new LineSegment(new Point(radiusX, 0), true);
                    break;
                default:
                    arcShadingLTSegment = new PolyLineSegment(new PointCollection() { new Point(0, 0), new Point(radiusX, 0), new Point(0, 0) }, true);
                    break;
            }

            LineSegment lineShadingTopSegment = new LineSegment(new Point(rectWidth - radiusX, 0), true);

            PathSegment arcShadingRTSegment;
            switch (corner.ToLower())
            {
                case "standard":
                    arcShadingRTSegment = new PolyLineSegment(new PointCollection() { new Point(rectWidth, 0), new Point(rectWidth, radiusY), new Point(rectWidth, 0) }, true);
                    break;
                case "round":
                    arcShadingRTSegment = new ArcSegment(new Point(rectWidth, radiusY), new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true);
                    break;
                case "round out":
                    arcShadingRTSegment = new ArcSegment(new Point(rectWidth, radiusY), new Size(radiusX, radiusY), 0, false, SweepDirection.Counterclockwise, true);
                    break;
                case "cut out":
                    arcShadingRTSegment = new LineSegment(new Point(rectWidth, radiusY), true);
                    break;
                default:
                    arcShadingRTSegment = new PolyLineSegment(new PointCollection() { new Point(rectWidth, 0), new Point(rectWidth, radiusY), new Point(rectWidth, 0) }, true);
                    break;
            }

            LineSegment lineShadingRightSegment = new LineSegment(new Point(rectWidth, rectHeight - radiusY), true);

            PathSegment arcShadingRBSegment;
            switch (corner.ToLower())
            {
                case "standard":
                    arcShadingRBSegment = new PolyLineSegment(new PointCollection() { new Point(rectWidth, rectHeight), new Point(rectWidth - radiusX, rectHeight), new Point(rectWidth, rectHeight) }, true);
                    break;
                case "round":
                    arcShadingRBSegment = new ArcSegment(new Point(rectWidth - radiusX, rectHeight), new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true);
                    break;
                case "round out":
                    arcShadingRBSegment = new ArcSegment(new Point(rectWidth - radiusX, rectHeight), new Size(radiusX, radiusY), 0, false, SweepDirection.Counterclockwise, true);
                    break;
                case "cut out":
                    arcShadingRBSegment = new LineSegment(new Point(rectWidth - radiusX, rectHeight), true);
                    break;
                default:
                    arcShadingRBSegment = new PolyLineSegment(new PointCollection() { new Point(rectWidth, rectHeight), new Point(rectWidth - radiusX, rectHeight), new Point(rectWidth, rectHeight) }, true);
                    break;
            }

            LineSegment lineShadingBottomSegment = new LineSegment(new Point(radiusX, rectHeight), true);

            PathSegment arcShadingLBSegment;
            switch (corner.ToLower())
            {
                case "standard":
                    arcShadingLBSegment = new PolyLineSegment(new PointCollection() { new Point(0, rectHeight), new Point(0, rectHeight - radiusY), new Point(0, rectHeight) }, true);
                    break;
                case "round":
                    arcShadingLBSegment = new ArcSegment(new Point(0, rectHeight - radiusY), new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true);
                    break;
                case "round out":
                    arcShadingLBSegment = new ArcSegment(new Point(0, rectHeight - radiusY), new Size(radiusX, radiusY), 0, false, SweepDirection.Counterclockwise, true);
                    break;
                case "cut out":
                    arcShadingLBSegment = new LineSegment(new Point(0, rectHeight - radiusY), true);
                    break;
                default:
                    arcShadingLBSegment = new PolyLineSegment(new PointCollection() { new Point(0, rectHeight), new Point(0, rectHeight - radiusY), new Point(0, rectHeight) }, true);
                    break;
            }

            LineSegment lineShadingLeftSegment = new LineSegment(new Point(0, radiusY), true);

            pathShadingFigure.Segments.Add(arcShadingLTSegment);
            pathShadingFigure.Segments.Add(lineShadingTopSegment);
            pathShadingFigure.Segments.Add(arcShadingRTSegment);
            pathShadingFigure.Segments.Add(lineShadingRightSegment);
            pathShadingFigure.Segments.Add(arcShadingRBSegment);
            pathShadingFigure.Segments.Add(lineShadingBottomSegment);
            pathShadingFigure.Segments.Add(arcShadingLBSegment);
            pathShadingFigure.Segments.Add(lineShadingLeftSegment);

            PathGeometry pathShadingGeometry = new PathGeometry();
            pathShadingGeometry.Figures.Add(pathShadingFigure);
            pathShadingGeometry.FillRule = FillRule.Nonzero;

            System.Windows.Shapes.Path pathShading = new System.Windows.Shapes.Path();
            pathShading.Stretch = System.Windows.Media.Stretch.Fill;
            pathShading.StrokeLineJoin = System.Windows.Media.PenLineJoin.Round;
            pathShading.Stroke = new SolidColorBrush(shadingFillColor);
            pathShading.Fill = new SolidColorBrush(shadingFillColor);
            pathShading.StrokeThickness = (double)borderWidth;
            pathShading.Data = pathShadingGeometry;

            canvas.Children.Add(pathShading);
            Canvas.SetLeft(pathShading, shadingOffsetX + (double)borderWidth / 2);
            Canvas.SetTop(pathShading, shadingOffsetY + (double)borderWidth / 2);

            #endregion

            #region rectangle

            PathFigure pathRectFigure = new PathFigure();
            pathRectFigure.StartPoint = new System.Windows.Point(0, radiusY);
            pathRectFigure.IsClosed = true;

            PathSegment arcRectLTSegment;
            switch (corner.ToLower())
            {
                case "standard":
                    arcRectLTSegment = new PolyLineSegment(new PointCollection() { new Point(0, 0), new Point(radiusX, 0), new Point(0, 0) }, true);
                    break;
                case "round":
                    arcRectLTSegment = new ArcSegment(new Point(radiusX, 0), new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true);
                    break;
                case "round out":
                    arcRectLTSegment = new ArcSegment(new Point(radiusX, 0), new Size(radiusX, radiusY), 0, false, SweepDirection.Counterclockwise, true);
                    break;
                case "cut out":
                    arcRectLTSegment = new LineSegment(new Point(radiusX, 0), true);
                    break;
                default:
                    arcRectLTSegment = new PolyLineSegment(new PointCollection() { new Point(0, 0), new Point(radiusX, 0), new Point(0, 0) }, true);
                    break;
            }

            LineSegment lineRectTopSegment = new LineSegment(new Point(rectWidth - radiusX, 0), true);

            PathSegment arcRectRTSegment;
            switch (corner.ToLower())
            {
                case "standard":
                    arcRectRTSegment = new PolyLineSegment(new PointCollection() { new Point(rectWidth, 0), new Point(rectWidth, radiusY), new Point(rectWidth, 0) }, true);
                    break;
                case "round":
                    arcRectRTSegment = new ArcSegment(new Point(rectWidth, radiusY), new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true);
                    break;
                case "round out":
                    arcRectRTSegment = new ArcSegment(new Point(rectWidth, radiusY), new Size(radiusX, radiusY), 0, false, SweepDirection.Counterclockwise, true);
                    break;
                case "cut out":
                    arcRectRTSegment = new LineSegment(new Point(rectWidth, radiusY), true);
                    break;
                default:
                    arcRectRTSegment = new PolyLineSegment(new PointCollection() { new Point(rectWidth, 0), new Point(rectWidth, radiusY), new Point(rectWidth, 0) }, true);
                    break;
            }

            LineSegment lineRectRightSegment = new LineSegment(new Point(rectWidth, rectHeight - radiusY), true);

            PathSegment arcRectRBSegment;
            switch (corner.ToLower())
            {
                case "standard":
                    arcRectRBSegment = new PolyLineSegment(new PointCollection() { new Point(rectWidth, rectHeight), new Point(rectWidth - radiusX, rectHeight), new Point(rectWidth, rectHeight) }, true);
                    break;
                case "round":
                    arcRectRBSegment = new ArcSegment(new Point(rectWidth - radiusX, rectHeight), new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true);
                    break;
                case "round out":
                    arcRectRBSegment = new ArcSegment(new Point(rectWidth - radiusX, rectHeight), new Size(radiusX, radiusY), 0, false, SweepDirection.Counterclockwise, true);
                    break;
                case "cut out":
                    arcRectRBSegment = new LineSegment(new Point(rectWidth - radiusX, rectHeight), true);
                    break;
                default:
                    arcRectRBSegment = new PolyLineSegment(new PointCollection() { new Point(rectWidth, rectHeight), new Point(rectWidth - radiusX, rectHeight), new Point(rectWidth, rectHeight) }, true);
                    break;
            }

            LineSegment lineRectBottomSegment = new LineSegment(new Point(radiusX, rectHeight), true);

            PathSegment arcRectLBSegment;
            switch (corner.ToLower())
            {
                case "standard":
                    arcRectLBSegment = new PolyLineSegment(new PointCollection() { new Point(0, rectHeight), new Point(0, rectHeight - radiusY), new Point(0, rectHeight) }, true);
                    break;
                case "round":
                    arcRectLBSegment = new ArcSegment(new Point(0, rectHeight - radiusY), new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true);
                    break;
                case "round out":
                    arcRectLBSegment = new ArcSegment(new Point(0, rectHeight - radiusY), new Size(radiusX, radiusY), 0, false, SweepDirection.Counterclockwise, true);
                    break;
                case "cut out":
                    arcRectLBSegment = new LineSegment(new Point(0, rectHeight - radiusY), true);
                    break;
                default:
                    arcRectLBSegment = new PolyLineSegment(new PointCollection() { new Point(0, rectHeight), new Point(0, rectHeight - radiusY), new Point(0, rectHeight) }, true);
                    break;
            }

            LineSegment lineRectLeftSegment = new LineSegment(new Point(0, radiusY), true);

            pathRectFigure.Segments.Add(arcRectLTSegment);
            pathRectFigure.Segments.Add(lineRectTopSegment);
            pathRectFigure.Segments.Add(arcRectRTSegment);
            pathRectFigure.Segments.Add(lineRectRightSegment);
            pathRectFigure.Segments.Add(arcRectRBSegment);
            pathRectFigure.Segments.Add(lineRectBottomSegment);
            pathRectFigure.Segments.Add(arcRectLBSegment);
            pathRectFigure.Segments.Add(lineRectLeftSegment);

            PathGeometry pathRectGeometry = new PathGeometry();
            pathRectGeometry.Figures.Add(pathRectFigure);
            pathRectGeometry.FillRule = FillRule.Nonzero;

            System.Windows.Shapes.Path pathRect = new System.Windows.Shapes.Path();
            pathRect.Stretch = System.Windows.Media.Stretch.Fill;
            pathRect.StrokeLineJoin = System.Windows.Media.PenLineJoin.Round;
            pathRect.Stroke = new SolidColorBrush(fillColor);
            pathRect.Fill = new SolidColorBrush(fillColor);
            pathRect.StrokeThickness = 0;
            pathRect.Data = pathRectGeometry;

            canvas.Children.Add(pathRect);
            Canvas.SetTop(pathRect, (double)borderWidth / 2);
            Canvas.SetLeft(pathRect, (double)borderWidth / 2);

            #endregion

            System.Windows.Shapes.Path pathBorder = new System.Windows.Shapes.Path();
            pathBorder.Stretch = System.Windows.Media.Stretch.Fill;
            pathBorder.StrokeLineJoin = System.Windows.Media.PenLineJoin.Round;
            pathBorder.Stroke = new SolidColorBrush(borderColor);
            pathBorder.StrokeThickness = (double)(borderWidth);
            pathBorder.StrokeStartLineCap = cap;
            pathBorder.StrokeEndLineCap = cap;

            switch (lineStyle.ToLower())
            {
                case "solid":
                    pathBorder.StrokeDashArray = new DoubleCollection();
                    break;
                case "dot":
                    pathBorder.StrokeDashArray = new DoubleCollection() { 4, 2 };
                    break;
                case "dashdot":
                    pathBorder.StrokeDashArray = new DoubleCollection() { 4, 2, 1, 2 };
                    break;
                case "dashdotdot":
                    pathBorder.StrokeDashArray = new DoubleCollection() { 4, 2, 1, 2, 1, 2 };
                    break;
                default:
                    pathBorder.StrokeDashArray = new DoubleCollection();
                    break;
            }

            pathBorder.StrokeLineJoin = join;
            canvas.Children.Add(pathBorder);

            PathGeometry pathBorderGeometry = new PathGeometry();
            pathBorderGeometry.FillRule = FillRule.Nonzero;
            pathBorder.Data = pathBorderGeometry;

            PathFigure pathOrigin = new PathFigure();
            pathOrigin.StartPoint = new Point(0, 0);
            pathBorderGeometry.Figures.Add(pathOrigin);

            if (_BorderStyle.LeftBorderFlag)
            {
                #region left border

                PathFigure pathLeftBorderFigure = new PathFigure();
                pathLeftBorderFigure.StartPoint = new System.Windows.Point(0, radiusY);
                pathLeftBorderFigure.IsClosed = false;
                LineSegment lineLeftBorderSegment = new LineSegment(new Point(0, rectHeight - radiusY), true);
                pathLeftBorderFigure.Segments.Add(lineLeftBorderSegment);

                pathBorderGeometry.Figures.Add(pathLeftBorderFigure);

                #endregion
            }

            if (_BorderStyle.TopBorderFlag)
            {
                #region top border

                PathFigure pathTopBorderFigure = new PathFigure();
                pathTopBorderFigure.StartPoint = new System.Windows.Point(radiusX, 0);
                pathTopBorderFigure.IsClosed = false;
                LineSegment lineTopBorderSegment = new LineSegment(new Point(rectWidth - radiusX, 0), true);
                pathTopBorderFigure.Segments.Add(lineTopBorderSegment);

                pathBorderGeometry.Figures.Add(pathTopBorderFigure);

                #endregion
            }

            if (_BorderStyle.RightBorderFlag)
            {
                #region right border

                PathFigure pathRightBorderFigure = new PathFigure();
                pathRightBorderFigure.StartPoint = new System.Windows.Point(rectWidth, radiusY);
                pathRightBorderFigure.IsClosed = false;
                LineSegment lineRightBorderSegment = new LineSegment(new Point(rectWidth, rectHeight - radiusY), true);
                pathRightBorderFigure.Segments.Add(lineRightBorderSegment);

                pathBorderGeometry.Figures.Add(pathRightBorderFigure);

                #endregion
            }

            if (_BorderStyle.BottomBorderFlag)
            {
                #region bottom border

                PathFigure pathBottomBorderFigure = new PathFigure();
                pathBottomBorderFigure.StartPoint = new System.Windows.Point(radiusX, rectHeight);
                pathBottomBorderFigure.IsClosed = false;
                LineSegment lineBottomBorderSegment = new LineSegment(new Point(rectWidth - radiusX, rectHeight), true);
                pathBottomBorderFigure.Segments.Add(lineBottomBorderSegment);

                pathBorderGeometry.Figures.Add(pathBottomBorderFigure);

                #endregion
            }

            if (_BorderStyle.LDiagonalFlag)
            {
                #region left diagonal

                PathFigure pathLDiagonalFigure = new PathFigure();
                pathLDiagonalFigure.StartPoint = new System.Windows.Point(radiusX / 2, radiusY / 2);
                pathLDiagonalFigure.IsClosed = false;
                LineSegment lineLDiagonalSegment = new LineSegment(new Point(rectWidth - radiusX / 2, rectHeight - radiusY / 2), true);
                pathLDiagonalFigure.Segments.Add(lineLDiagonalSegment);

                pathBorderGeometry.Figures.Add(pathLDiagonalFigure);

                #endregion
            }

            if (_BorderStyle.RDiagonalFlag)
            {
                #region right diagonal

                PathFigure pathRDiagonalFigure = new PathFigure();
                pathRDiagonalFigure.StartPoint = new System.Windows.Point(rectWidth - radiusX / 2, radiusY / 2);
                pathRDiagonalFigure.IsClosed = false;
                LineSegment lineRDiagonalSegment = new LineSegment(new Point(radiusX / 2, rectHeight - radiusY / 2), true);
                pathRDiagonalFigure.Segments.Add(lineRDiagonalSegment);

                pathBorderGeometry.Figures.Add(pathRDiagonalFigure);

                #endregion
            }

            if (_BorderStyle.LTCornerFlag)
            {
                #region left-top corner

                PathFigure pathLTCornerFigure = new PathFigure();
                pathLTCornerFigure.StartPoint = new System.Windows.Point(0, radiusY);
                pathLTCornerFigure.IsClosed = false;

                PathSegment arcLTCornerSegment;
                switch (corner.ToLower())
                {
                    case "standard":
                        arcLTCornerSegment = new PolyLineSegment(new PointCollection() { new Point(0, 0), new Point(radiusX, 0), new Point(0, 0) }, true);
                        break;
                    case "round":
                        arcLTCornerSegment = new ArcSegment(new Point(radiusX, 0), new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true);
                        break;
                    case "round out":
                        arcLTCornerSegment = new ArcSegment(new Point(radiusX, 0), new Size(radiusX, radiusY), 0, false, SweepDirection.Counterclockwise, true);
                        break;
                    case "cut out":
                        arcLTCornerSegment = new LineSegment(new Point(radiusX, 0), true);
                        break;
                    default:
                        arcLTCornerSegment = new PolyLineSegment(new PointCollection() { new Point(0, 0), new Point(radiusX, 0), new Point(0, 0) }, true);
                        break;
                }

                pathLTCornerFigure.Segments.Add(arcLTCornerSegment);
                pathLTCornerFigure.IsClosed = false;

                pathBorderGeometry.Figures.Add(pathLTCornerFigure);

                #endregion
            }

            if (_BorderStyle.RTCornerFlag)
            {
                #region right-top corner

                PathFigure pathRTCornerFigure = new PathFigure();
                pathRTCornerFigure.StartPoint = new System.Windows.Point(rectWidth - radiusX, 0);
                pathRTCornerFigure.IsClosed = false;

                PathSegment arcRTCornerSegment;
                switch (corner.ToLower())
                {
                    case "standard":
                        arcRTCornerSegment = new PolyLineSegment(new PointCollection() { new Point(rectWidth, 0), new Point(rectWidth, radiusY), new Point(rectWidth, 0) }, true);
                        break;
                    case "round":
                        arcRTCornerSegment = new ArcSegment(new Point(rectWidth, radiusY), new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true);
                        break;
                    case "round out":
                        arcRTCornerSegment = new ArcSegment(new Point(rectWidth, radiusY), new Size(radiusX, radiusY), 0, false, SweepDirection.Counterclockwise, true);
                        break;
                    case "cut out":
                        arcRTCornerSegment = new LineSegment(new Point(rectWidth, radiusY), true);
                        break;
                    default:
                        arcRTCornerSegment = new PolyLineSegment(new PointCollection() { new Point(rectWidth, 0), new Point(rectWidth, radiusY), new Point(rectWidth, 0) }, true);
                        break;
                }

                pathRTCornerFigure.Segments.Add(arcRTCornerSegment);

                pathBorderGeometry.Figures.Add(pathRTCornerFigure);

                #endregion
            }

            if (_BorderStyle.RBCornerFlag)
            {
                #region right-bottom corner

                PathFigure pathRBCornerFigure = new PathFigure();
                pathRBCornerFigure.StartPoint = new System.Windows.Point(rectWidth, rectHeight - radiusY);
                pathRBCornerFigure.IsClosed = false;

                PathSegment arcRBCornerSegment;
                switch (corner.ToLower())
                {
                    case "standard":
                        arcRBCornerSegment = new PolyLineSegment(new PointCollection() { new Point(rectWidth, rectHeight), new Point(rectWidth - radiusX, rectHeight), new Point(rectWidth, rectHeight) }, true);
                        break;
                    case "round":
                        arcRBCornerSegment = new ArcSegment(new Point(rectWidth - radiusX, rectHeight), new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true);
                        break;
                    case "round out":
                        arcRBCornerSegment = new ArcSegment(new Point(rectWidth - radiusX, rectHeight), new Size(radiusX, radiusY), 0, false, SweepDirection.Counterclockwise, true);
                        break;
                    case "cut out":
                        arcRBCornerSegment = new LineSegment(new Point(rectWidth - radiusX, rectHeight), true);
                        break;
                    default:
                        arcRBCornerSegment = new PolyLineSegment(new PointCollection() { new Point(rectWidth, rectHeight), new Point(rectWidth - radiusX, rectHeight), new Point(rectWidth, rectHeight) }, true);
                        break;
                }

                pathRBCornerFigure.Segments.Add(arcRBCornerSegment);

                pathBorderGeometry.Figures.Add(pathRBCornerFigure);

                #endregion
            }

            if (_BorderStyle.LBCornerFlag)
            {
                #region left-bottom corner

                PathFigure pathLBCornerFigure = new PathFigure();
                pathLBCornerFigure.StartPoint = new System.Windows.Point(radiusX, rectHeight);
                pathLBCornerFigure.IsClosed = false;

                PathSegment arcLBCornerSegment;
                switch (corner.ToLower())
                {
                    case "standard":
                        arcLBCornerSegment = new PolyLineSegment(new PointCollection() { new Point(0, rectHeight), new Point(0, rectHeight - radiusY), new Point(0, rectHeight) }, true);
                        break;
                    case "round":
                        arcLBCornerSegment = new ArcSegment(new Point(0, rectHeight - radiusY), new Size(radiusX, radiusY), 0, false, SweepDirection.Clockwise, true);
                        break;
                    case "round out":
                        arcLBCornerSegment = new ArcSegment(new Point(0, rectHeight - radiusY), new Size(radiusX, radiusY), 0, false, SweepDirection.Counterclockwise, true);
                        break;
                    case "cut out":
                        arcLBCornerSegment = new LineSegment(new Point(0, rectHeight - radiusY), true);
                        break;
                    default:
                        arcLBCornerSegment = new PolyLineSegment(new PointCollection() { new Point(0, rectHeight), new Point(0, rectHeight - radiusY), new Point(0, rectHeight) }, true);
                        break;
                }

                pathLBCornerFigure.Segments.Add(arcLBCornerSegment);

                pathBorderGeometry.Figures.Add(pathLBCornerFigure);

                #endregion
            }
        }
    }
}
