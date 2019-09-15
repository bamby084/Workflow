using Designer.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Designer
{
    public class BlockStyle : ILayoutNode
    {
        private Border Border;
        public Canvas Canvas;
        private Page Owner;
        private Border[] CtrlBorder;

        private double _Left = 10.0 * ImageHelper.DPI * Page.INCHES_PER_MM;
        private double _Top = 10.0 * ImageHelper.DPI * Page.INCHES_PER_MM;
        private double _Width;
        private double _Height;

        public double Left { get => _Left; }
        public double Top { get => _Top; }
        public double Width { get => _Width; }
        public double Height { get => _Height; }

        private void calcActrualSizeFromMargin()
        {
            double width = Owner.PageWidth;
            double height = Owner.PageHeight;

            _Width = width - _Left * 2;
            _Height = height - _Top * 2;
        }

        protected BlockStylePropertiesPanel _LayoutProperties;
        private string _Id = "";
        public string Id
        {
            get => _Id;
            set
            {
                _Id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Id"));
            }
        }

        public FlowStyles FlowStyles = new FlowStyles();

        public string PageId { get; set; }
        public string BorderId { get; set; }

        private Action _OnRemove;
        public BlockStylePropertiesPanel LayoutProperties { get => _LayoutProperties; }
        private MenuItem _MenuItem;

        public BlockStyle() { }

        

        public BlockStyle(int id, Page page)
        {
            Owner = page;
            Canvas = page.Canvas;

            calcActrualSizeFromMargin();

            SetId(id);


            CreateBorderCanvas();
            RefreshCtrlBorderPos(new Point(_Left, _Top), new Size(_Width, _Height));

        }

        public void SetPage(Page page)
        {
            Owner = page;
            Canvas = page.Canvas;
        }

        public void CreateBorderCanvas()
        {
            Canvas.Children.Add(pathShading);
            Canvas.Children.Add(pathRect);
            Canvas.Children.Add(pathBorder);
            
            Border = new Border();
            Border.BorderBrush = new SolidColorBrush(Colors.DarkBlue);
            Border.BorderThickness = new Thickness(1);

            Border.Width = _Width; Border.Height = _Height;
            Canvas.Children.Add(Border);
            Canvas.SetLeft(Border, _Left);
            Canvas.SetTop(Border, _Top);
            Border.Tag = HitType.Body;
            


            CtrlBorder = new Border[8];
            for (int i = 0; i < 8; i++)
            {
                Border border = new Border();
                border.BorderBrush = new SolidColorBrush(Colors.DarkBlue);
                border.BorderThickness = new Thickness(1);
                border.Background = new SolidColorBrush(Colors.DarkBlue);
                border.Width = 8; border.Height = 8;
                CtrlBorder[i] = border;

                Canvas.Children.Add(border);
            }

            _LayoutProperties = new BlockStylePropertiesPanel(this);
            _LayoutProperties.PropertyChanged += LayoutProperties_PropertyChanged;

            _LayoutProperties.Left = _Left;
            _LayoutProperties.Top = _Top;
            _LayoutProperties.Width = _Width;
            _LayoutProperties.Height = _Height;

            _LayoutProperties.cmbBorderList.Items.Add(new { Id = "Empty" });
            foreach (BorderStyle bs in DesignerPage.self.BorderStyles)
            {
                _LayoutProperties.cmbBorderList.Items.Add(bs);
                if (BorderId != null && BorderId != "" && BorderId == bs.Id)
                    _LayoutProperties.cmbBorderList.SelectedItem = bs;
            }

            _LayoutProperties.cmbBorderList.Items.Add(new { Id = "Create New Border..." });
            if (_LayoutProperties.cmbBorderList.SelectedItem == null)
                _LayoutProperties.cmbBorderList.SelectedIndex = 0;

            _MenuItem = new MenuItem();

            bool visible = (bool)DesignerPage.self.chkBlockArrow.IsChecked;
            ShowControlBorder(visible);
        }

        public void DeleteBorder()
        {
            Owner.Canvas.Children.Remove(Border);
            foreach (Border border in CtrlBorder)
            {
                Owner.Canvas.Children.Remove(border);
            }

            Owner.Canvas.Children.Remove(pathBorder);
            Owner.Canvas.Children.Remove(pathRect);
            Owner.Canvas.Children.Remove(pathShading);
        }

        private void RefreshCtrlBorderPos(Point pos, Size size)
        {
            for (int i = 0; i < 8; i++)
            {
                Border border = CtrlBorder[i];
                if (i == 0)
                {
                    Canvas.SetLeft(border, pos.X -4);
                    Canvas.SetTop(border, pos.Y - 4);
                    border.Tag = HitType.LT;
                }
                else if (i == 1)
                {
                    Canvas.SetLeft(border, pos.X + size.Width / 2 - 4);
                    Canvas.SetTop(border, pos.Y - 4);
                    border.Tag = HitType.T;
                }
                else if (i == 2)
                {
                    Canvas.SetLeft(border, pos.X + size.Width - 4);
                    Canvas.SetTop(border, pos.Y - 4);
                    border.Tag = HitType.RT;
                }
                else if (i == 3)
                {
                    Canvas.SetLeft(border, pos.X + size.Width - 4);
                    Canvas.SetTop(border, pos.Y + size.Height / 2 - 4);
                    border.Tag = HitType.R;
                }
                else if (i == 4)
                {
                    Canvas.SetLeft(border, pos.X + size.Width - 4);
                    Canvas.SetTop(border, pos.Y + size.Height - 4);
                    border.Tag = HitType.RB;
                }
                else if (i == 5)
                {
                    Canvas.SetLeft(border, pos.X + size.Width / 2 - 4);
                    Canvas.SetTop(border, pos.Y + size.Height - 4);
                    border.Tag = HitType.B;
                }
                else if (i == 6)
                {
                    Canvas.SetLeft(border, pos.X - 4);
                    Canvas.SetTop(border, pos.Y + size.Height - 4);
                    border.Tag = HitType.LB;
                }
                else if (i == 7)
                {
                    Canvas.SetLeft(border, pos.X - 4);
                    Canvas.SetTop(border, pos.Y + size.Height / 2 - 4);
                    border.Tag = HitType.L;
                }

            }
        }

        public BorderStyle borderStyle;

        internal void SetBorderStyle(BorderStyle bs)
        {
            if (bs == null)
            {
                Border.Visibility = Visibility.Visible;
                return;
            }

            borderStyle = bs;
            drawPreviewRectangle(bs, Canvas);
        }

        public System.Windows.Shapes.Path pathShading = new System.Windows.Shapes.Path();
        public System.Windows.Shapes.Path pathRect = new System.Windows.Shapes.Path();
        public System.Windows.Shapes.Path pathBorder = new System.Windows.Shapes.Path();

        public void drawPreviewRectangle(BorderStyle _BorderStyle, Canvas canvas)
        {
            //canvas.Children.Clear();

            if (_BorderStyle == null)
                return;

            double rectHeight = _Height;
            double rectWidth = _Width;

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

            
            pathShading.Stretch = System.Windows.Media.Stretch.Fill;
            pathShading.StrokeLineJoin = System.Windows.Media.PenLineJoin.Round;
            pathShading.Stroke = new SolidColorBrush(shadingFillColor);
            pathShading.Fill = new SolidColorBrush(shadingFillColor);
            pathShading.StrokeThickness = (double)borderWidth;
            pathShading.Data = pathShadingGeometry;

            //canvas.Children.Add(pathShading);
            Canvas.SetLeft(pathShading, _Left + shadingOffsetX + (double)borderWidth / 2);
            Canvas.SetTop(pathShading, _Top + shadingOffsetY + (double)borderWidth / 2);

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

            //System.Windows.Shapes.Path pathRect = new System.Windows.Shapes.Path();
            pathRect.Stretch = System.Windows.Media.Stretch.Fill;
            pathRect.StrokeLineJoin = System.Windows.Media.PenLineJoin.Round;
            pathRect.Stroke = new SolidColorBrush(fillColor);
            pathRect.Fill = new SolidColorBrush(fillColor);
            pathRect.StrokeThickness = 0;
            pathRect.Data = pathRectGeometry;

            //canvas.Children.Add(pathRect);
            Canvas.SetTop(pathRect, _Top + (double)borderWidth / 2);
            Canvas.SetLeft(pathRect, _Left + (double)borderWidth / 2);

            #endregion

           // System.Windows.Shapes.Path pathBorder = new System.Windows.Shapes.Path();
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
            //canvas.Children.Add(pathBorder);
            Canvas.SetLeft(pathBorder, _Left);
            Canvas.SetTop(pathBorder, _Top);

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

            Border.Visibility = Visibility.Hidden;
        }

        private void LayoutProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Left")
            {
                _Left = LayoutProperties.Left;
                Canvas.SetLeft(Border, _Left);
            }
            else if (e.PropertyName == "Top")
            {
                _Top = LayoutProperties.Top;
                Canvas.SetTop(Border, _Top);
            }
            else if (e.PropertyName == "Width")
            {
                _Width = LayoutProperties.Width;
                Border.Width = _Width;
            }
            else if (e.PropertyName == "Height")
            {
                _Height = LayoutProperties.Height;
                Border.Height = _Height;
            }

            RefreshCtrlBorderPos(new Point(_Left, _Top), new Size(_Width, _Height));
            
            if (borderStyle != null)
            {
                Border.Visibility = Visibility.Hidden;
                drawPreviewRectangle(borderStyle, Canvas);
            }
            else
            {
                Border.Visibility = Visibility.Visible;
            }
        }

        private void SetId(int id)
        {
            Id = "Block" + id.ToString();
        }

        public MenuItem MenuItem => _MenuItem;
        public TvItemCategory TreeViewCategory => TvItemCategory.BLOCK_STYLES;
        public string TreeViewName { get => "BlockStyle"; }
        public Action OnRemove { get => _OnRemove; set => _OnRemove = value; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            
        }

        public FrameworkElement GetPropertyLayout()
        {
            return LayoutProperties;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Id = reader.GetAttribute("Id");
            PageId = reader.GetAttribute("PageId");
            BorderId = reader.GetAttribute("BorderId");
            _Left = double.Parse(reader.GetAttribute("Left"));
            _Top = double.Parse(reader.GetAttribute("Top"));
            _Width = double.Parse(reader.GetAttribute("Width"));
            _Height = double.Parse(reader.GetAttribute("Height"));

        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Id", Id);
            writer.WriteAttributeString("PageId", Owner.Id);
            if (borderStyle != null)
                writer.WriteAttributeString("BorderId", borderStyle.Id);
            else
                writer.WriteAttributeString("BorderId", "");

            writer.WriteAttributeString("Left", LayoutProperties.Left.ToString());
            writer.WriteAttributeString("Top", LayoutProperties.Top.ToString());
            writer.WriteAttributeString("Width", LayoutProperties.Width.ToString());
            writer.WriteAttributeString("Height", LayoutProperties.Height.ToString());
        }

        public void ShowControlBorder(bool show)
        {
            Visibility visible = Visibility.Hidden;
            if (show == true)
            {
                visible = Visibility.Visible;
                _LayoutProperties.IsEnabled = true;
                Border.BringIntoView();
            }
            else
                _LayoutProperties.IsEnabled = false;

            foreach (Border border in CtrlBorder)
            {
                border.Visibility = visible;
            }
        }

        private enum HitType
        {
            None, Body, LT, RT, LB, RB, L, R, T, B
        };
        private HitType MouseHitType = HitType.None;
        private bool DragInProgress = false;
        private Point LastPoint;


        public void Add(Canvas uiElement)
        {
        }

        public void Canvas_MouseLeftButtonDown()
        {
            Point pos = Mouse.GetPosition(Canvas);
            MouseHitType = SetHitType(pos);
            SetMouseCursor();
            if (MouseHitType == HitType.None)
                return;
            LastPoint = pos;
            DragInProgress = true;

            Canvas.CaptureMouse();
        }

        public void Canvas_MouseLeftButtonUp()
        {
            DragInProgress = false;
            Canvas.ReleaseMouseCapture();

            foreach (FlowStyle fs in FlowStyles)
            {
                fs.RefreshFlowControl();
            }
        }

        public void Canvas_MouseMove()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!DragInProgress)
                {
                    MouseHitType = SetHitType(Mouse.GetPosition(Canvas));
                    SetMouseCursor();
                }
                else
                { 
                    Point pos = Mouse.GetPosition(Canvas);

                    if (pos.X <= 0) pos.X = 0;
                    if (pos.Y <= 0) pos.Y = 0;

                    if (pos.X >= Owner.PageWidth) pos.X = Owner.PageWidth;
                    if (pos.Y >= Owner.PageHeight) pos.Y = Owner.PageHeight;
                
                    double offset_x = pos.X - LastPoint.X;
                    double offset_y = pos.Y - LastPoint.Y;

                    double new_x = Canvas.GetLeft(Border);
                    double new_y = Canvas.GetTop(Border);

                    if (new_x <= 0 && offset_x <= 0) offset_x = 0;
                    if (new_y <= 0 && offset_y <= 0) offset_y = 0; 

                    double new_width = Border.Width;
                    double new_height = Border.Height;

                    if (new_width <= 40)
                    {
                        new_width = 40;
                        offset_x = 0;
                    }

                    if (new_height <= 40)
                    {
                        new_height = 40;
                        offset_y = 0;
                    }

                    switch (MouseHitType)
                    {
                        case HitType.Body:
                            new_x += offset_x;
                            new_y += offset_y;
                            break;
                        case HitType.LT:
                            new_x += offset_x;
                            new_y += offset_y;
                            new_width -= offset_x;
                            new_height -= offset_y;
                            break;
                        case HitType.RT:
                            new_y += offset_y;
                            new_width += offset_x;
                            new_height -= offset_y;
                            break;
                        case HitType.RB:
                            new_width += offset_x;
                            new_height += offset_y;
                            break;
                        case HitType.LB:
                            new_x += offset_x;
                            new_width -= offset_x;
                            new_height += offset_y;
                            break;
                        case HitType.L:
                            new_x += offset_x;
                            new_width -= offset_x;
                            break;
                        case HitType.R:
                            new_width += offset_x;
                            break;
                        case HitType.B:
                            new_height += offset_y;
                            break;
                        case HitType.T:
                            new_y += offset_y;
                            new_height -= offset_y;
                            break;
                    }

                    if ((new_width > 0) && (new_height > 0))
                    {


                        _Left = new_x; _Top = new_y;
                        _Width = new_width; _Height = new_height;

                        Canvas.SetLeft(Border, new_x);
                        Canvas.SetTop(Border, new_y);
                        Border.Width = new_width;
                        Border.Height = new_height;

                        LayoutProperties.Width = new_width;
                        LayoutProperties.Height = new_height;
                        LayoutProperties.Left = new_x;
                        LayoutProperties.Top = new_y;


                        RefreshCtrlBorderPos(new Point(new_x, new_y), new Size(new_width, new_height));
                        LastPoint = pos;

                        if (borderStyle != null)
                        {
                            Border.Visibility = Visibility.Hidden;

                            drawPreviewRectangle(borderStyle, Canvas);
                        }
                        else
                        {
                            Border.Visibility = Visibility.Visible;
                        }
                    }
                }
            });
        }
        
        private HitType SetHitType(Point point)
        {
            double left = Canvas.GetLeft(Border);
            double top = Canvas.GetTop(Border);
            double right = left + Border.Width;
            double bottom = top + Border.Height;
            if (point.X < left) return HitType.None;
            if (point.X > right) return HitType.None;
            if (point.Y < top) return HitType.None;
            if (point.Y > bottom) return HitType.None;

            const double GAP = 10;
            if (point.X - left < GAP)
            {
                if (point.Y - top < GAP) return HitType.LT;
                if (bottom - point.Y < GAP) return HitType.LB;

                double yy = Canvas.GetTop(CtrlBorder[7]);
                if (Math.Abs(point.Y - yy + 4) < GAP)
                    return HitType.L;

                return HitType.Body;
            }

            if (right - point.X < GAP)
            {
                if (point.Y - top < GAP) return HitType.RT;
                if (bottom - point.Y < GAP) return HitType.RB;

                double yy = Canvas.GetTop(CtrlBorder[3]);
                if (Math.Abs(point.Y - yy + 4) < GAP)
                    return HitType.R;

                return HitType.Body;
            }

            if (point.Y - top < GAP)
            {
                double xx = Canvas.GetLeft(CtrlBorder[1]);
                if (Math.Abs(point.X - xx) < GAP)
                    return HitType.T;

                return HitType.Body;
            }

            if (bottom - point.Y < GAP)
            {
                double xx = Canvas.GetLeft(CtrlBorder[5]);
                if (Math.Abs(point.X - xx) < GAP)
                    return HitType.B;

                return HitType.Body;
            }

            return HitType.None;
        }

        private void SetMouseCursor()
        {
            Cursor desired_cursor = Cursors.Arrow;
            switch (MouseHitType)
            {
                case HitType.None:
                    desired_cursor = Cursors.Arrow;
                    break;
                case HitType.Body:
                    desired_cursor = Cursors.ScrollAll;
                    break;
                case HitType.LT:
                case HitType.RB:
                    desired_cursor = Cursors.SizeNWSE;
                    break;
                case HitType.LB:
                case HitType.RT:
                    desired_cursor = Cursors.SizeNESW;
                    break;
                case HitType.T:
                case HitType.B:
                    desired_cursor = Cursors.SizeNS;
                    break;
                case HitType.L:
                case HitType.R:
                    desired_cursor = Cursors.SizeWE;
                    break;
            }

            if (DesignerPage.self.Cursor != desired_cursor)
                DesignerPage.self.Cursor = desired_cursor;
        }
    }

    public class BlockStyles : ObservableRangeCollection<BlockStyle>, IXmlSerializable
    {

        private BlockStylePropertiesPanel layoutProperties;
        public BlockStylePropertiesPanel LayoutProperties
        {
            get
            {
                if (layoutProperties == null)
                {
                    MainWindow window = MainWindow.Instance;
                    layoutProperties = new BlockStylePropertiesPanel();
                    layoutProperties.PropertyChanged += LayoutProperties_PropertyChanged;
                }

                return layoutProperties;
            }

            set => layoutProperties = value;
        }

        public BlockStyles() : base()
        {
            CollectionChanged += BlockStyles_CollectionChanged;
        }

        private void UpdateChildPropertyStates(IList<BlockStyles> children)
        {
            //var state = LayoutProperties.IsPageOrderVariable;
            // foreach (var page in children)
            // {
            //     page.LayoutProperties.TabNextPage.IsEnabled = state;
            // }
        }

        private void BlockStyles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    //UpdateChildPropertyStates(e.NewItems.Cast<ParagraphStyle>().ToList());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
            }

        }

        public void LayoutProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (BlockStyle bs in this)
            {
                Application.Current.Dispatcher.BeginInvoke(
                            new Action<DesignerPage>((c) => {
                                bs.drawPreviewRectangle(bs.borderStyle, bs.Canvas);
                            }),
                                        System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                                        DesignerPage.self
                                );
            }
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
            writer.WriteStartElement("Blocks");
            writer.WriteAttributeString("Count", Count.ToString());
            writer.WriteAttributeString("Counter", DesignerPage.self._para_id.ToString());
            foreach (var block in this)
            {
                writer.WriteStartElement(block.GetType().ToString());
                block.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }

    public static class ResizeHelper
    {
        

    }
}
