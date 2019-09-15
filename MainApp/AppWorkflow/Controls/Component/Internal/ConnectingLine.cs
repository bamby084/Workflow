using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AppWorkflow.Controls
{
    public class ConnectingLine
    {
        public Polygon Arrowhead;
        public Polyline Line;

        // The maximum amount of vertical deviation allowed without adding
        // a vertical segment in the line. In
        private static readonly double MAX_DEVIATION = 15d;

        public ConnectingLine(Brush brush)
        {
            Line = new Polyline();
            Line.StrokeThickness = 40;
            Line.FillRule = FillRule.Nonzero;
            Line.Points = new PointCollection();

            Arrowhead = new Polygon();
            Arrowhead.StrokeThickness = 1;
            Arrowhead.Points = new PointCollection();
            Arrowhead.Points.Add(new Point(0.0, 0.0));
            Arrowhead.Points.Add(new Point(1.0, 0.5));
            Arrowhead.Points.Add(new Point(0.0, 1.0));
            Arrowhead.LayoutTransform = new ScaleTransform(7, 7);

            SetBrush(brush);
        }

        public Visibility Visibility
        {
            get { return Line.Visibility; }
        }

        public void Redraw(Point outputPos, Point inputPos)
        {
            Vector fromStartToEnd = Point.Subtract(inputPos, outputPos);
            if (Math.Abs(fromStartToEnd.Y) > MAX_DEVIATION)
            {
                var midPoint = Vector.Multiply(0.5d, fromStartToEnd);
                var vertPoint1 = new Point(
                    midPoint.X + outputPos.X,
                    outputPos.Y
                    );
                var vertPoint2 = new Point(vertPoint1.X, inputPos.Y);

                Line.Points = new PointCollection();
                Line.Points.Add(outputPos);
                Line.Points.Add(vertPoint1);
                Line.Points.Add(vertPoint2);
                Line.Points.Add(inputPos);
            }
            else
            {
                Line.Points = new PointCollection();
                Line.Points.Add(outputPos);
                Line.Points.Add(inputPos);
            }
        }

        public void SetBrush(Brush brush)
        {
            Line.Stroke = brush;
            Arrowhead.Stroke = brush;
            Arrowhead.Fill = brush;
        }

        /// <summary>
        /// Sets the canvas position for this control. The arrowhead will be
        /// moved to point at the outputPos.
        /// </summary>
        /// <param name="outputPos">The output position.</param>
        /// <param name="inputPos">The input position.</param>
        public void SetCanvasPos(Point outputPos, Point inputPos)
        {
            Point startOffset = Line.RenderedGeometry.Transform.Transform(Line.Points[0]);
            Canvas.SetLeft(Line, outputPos.X - startOffset.X);
            Canvas.SetTop(Line, outputPos.Y - startOffset.Y);
            SetArrowheadCanvasPos(inputPos);
        }

        /// <summary>
        /// Sets the context menu for the arrowhead and line.
        /// </summary>
        /// <param name="menu">The menu to assign. can be null.</param>
        public void SetContextMenu(ContextMenu menu)
        {
            Line.ContextMenu = menu;
            Arrowhead.ContextMenu = menu;
        }

        public void SetVisibility(Visibility visibility)
        {
            Line.Visibility = visibility;
            Arrowhead.Visibility = visibility;
        }

        private void SetArrowheadCanvasPos(Point pos)
        {
            var offset = Arrowhead.LayoutTransform.Transform(Arrowhead.Points[1]);
            var point = Point.Subtract(pos, offset);
            Canvas.SetLeft(Arrowhead, point.X);
            Canvas.SetTop(Arrowhead, point.Y);
        }
    }
}
