using Designer.Tools;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Designer
{
	/// <summary>
	/// Interaction logic for PageRuler.xaml
	/// </summary>
	public partial class PageRuler : UserControl
	{
		private const int RULER_WIDTH = 20; // In WPF Points
		private LineGeometry Eighth, Fourth, Half, Full;
		private PageBase Page;
		private ScrollViewer Scroller;
		private Canvas TopRuler, LeftRuler;

		public PageRuler()
		{
			InitializeComponent();

			// LINES
			Eighth = new LineGeometry();
			Eighth.StartPoint = new Point();
			Eighth.EndPoint = new Point(0, RULER_WIDTH * (1d / 8d));

			Fourth = new LineGeometry();
			Fourth.StartPoint = new Point();
			Fourth.EndPoint = new Point(0, RULER_WIDTH * (1d / 4d));

			Half = new LineGeometry();
			Half.StartPoint = new Point();
			Half.EndPoint = new Point(0, RULER_WIDTH * (1d / 2d));

			Full = new LineGeometry();
			Full.StartPoint = new Point();
			Full.EndPoint = new Point(0, RULER_WIDTH);

			// RULERS
			TopRuler = new Canvas();
			TopRuler.Background = CanvasRef.Background;
			CanvasRef.Children.Add(TopRuler);

			LeftRuler = new Canvas();
			LeftRuler.Background = TopRuler.Background;
			CanvasRef.Children.Add(LeftRuler);
		}

		/// <summary>
		/// Binds this PageRuler to the specified Page and ScrollViewer
		/// for update notifications.
		/// </summary>
		/// <param name="page">The page.</param>
		/// <param name="scrollViewer">The scroll viewer.</param>
		public void Set(PageBase page, ScrollViewer scrollViewer)
		{
			if (Scroller != null) {
				Scroller.ScrollChanged -= ScrollViewer_ScrollChanged;
			}
			Scroller = scrollViewer;
			Scroller.ScrollChanged += ScrollViewer_ScrollChanged;

			if (Page != null) {
				Page.Canvas.SizeChanged -= Page_SizeChanged;
				Page.Canvas.LayoutTransform.Changed -= PageTransform_Changed;
			}
			Page = page;
			page.Canvas.SizeChanged += Page_SizeChanged;
			page.Canvas.LayoutTransform.Changed += PageTransform_Changed;

			Reconfigure();
		}

		private void AlignRulers()
		{
			var startPos = GetRelativePosStart();
			Canvas.SetLeft(TopRuler, startPos.X);
			Canvas.SetTop(TopRuler, 0);
			Canvas.SetLeft(LeftRuler, 0);
			Canvas.SetTop(LeftRuler, startPos.Y);
		}

		private Point GetRelativePosStart()
		{
			var pageRelPos = new Point();
			try {
				pageRelPos = Page.Canvas.TransformToAncestor(Scroller).Transform(new Point(0, 0));
			}
			catch (Exception) {
				return pageRelPos;
			}
			pageRelPos.X = Math.Max(pageRelPos.X, 0);
			pageRelPos.Y = Math.Max(pageRelPos.Y, 0);
			pageRelPos.X -= Scroller.HorizontalOffset;
			pageRelPos.Y -= Scroller.VerticalOffset;
			pageRelPos.X += RULER_WIDTH;
			pageRelPos.Y += RULER_WIDTH;
			return pageRelPos;
		}

		private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Reconfigure();
		}

		private void PageTransform_Changed(object sender, EventArgs e)
		{
			Reconfigure();
		}

		private void Reconfigure()
		{
			if (Page == null || Scroller == null) {
				return;
			}

			TopRuler.Children.Clear();
			LeftRuler.Children.Clear();

			var pageTransform = (ScaleTransform)Page.LayoutTransform;
			var pageWInch = Page.PageWidth / ImageHelper.DPI;
			var pageHInch = Page.PageHeight / ImageHelper.DPI;

			var spacing = 1d / 8d;

			// Top ruler
			TopRuler.Height = RULER_WIDTH;
			TopRuler.Width = Page.PageWidth * pageTransform.ScaleX;
			for (double i = 0; i <= pageWInch; i += spacing) {
				Path line = new Path();
				line.Stroke = Brushes.Black;
				var visualOffset = (i * pageTransform.ScaleX) * ImageHelper.DPI;
				if (i % 1 <= Double.Epsilon * 100) {
					line.StrokeThickness = 1.5;
					line.Data = Full;
					if (pageWInch - i >= Double.Epsilon * 100) {
						var label = new Label();
						label.Content = ((int)i).ToString();
						label.FontSize = 8;
						TopRuler.Children.Add(label);
						Canvas.SetLeft(label, visualOffset);
						Canvas.SetTop(label, 2);
					}
				}
				else if (i % (1d / 2d) == 0) {
					line.StrokeThickness = 1;
					line.Data = Half;
				}
				else if (i % (1d / 4d) == 0) {
					line.StrokeThickness = 1;
					line.Data = Fourth;
				}
				else if (i % (1d / 8d) == 0) {
					line.StrokeThickness = 1;
					line.Data = Eighth;
				}

				TopRuler.Children.Add(line);
				Canvas.SetLeft(line, visualOffset);
				Canvas.SetTop(line, 0);
			}

			// Left ruler
			LeftRuler.Height = Page.PageHeight * pageTransform.ScaleY;
			LeftRuler.Width = RULER_WIDTH;
			for (double i = 0; i <= pageHInch; i += spacing) {
				Path line = new Path();
				line.Stroke = Brushes.Black;
				var visualOffset = (i * pageTransform.ScaleY) * ImageHelper.DPI;
				if (i % 1 <= Double.Epsilon * 100) {
					line.StrokeThickness = 1.5;
					line.Data = Full;
					if (pageHInch - i >= Double.Epsilon * 100) {
						var label = new Label();
						label.Content = ((int)i).ToString();
						label.FontSize = 8;
						LeftRuler.Children.Add(label);
						Canvas.SetLeft(label, RULER_WIDTH / 4d);
						Canvas.SetTop(label, visualOffset);
					}
				}
				else if (i % (1d / 2d) == 0) {
					line.StrokeThickness = 1;
					line.Data = Half;
				}
				else if (i % (1d / 4d) == 0) {
					line.StrokeThickness = 1;
					line.Data = Fourth;
				}
				else if (i % (1d / 8d) == 0) {
					line.StrokeThickness = 1;
					line.Data = Eighth;
				}
				line.LayoutTransform = new RotateTransform
				{
					Angle = 90
				};
				LeftRuler.Children.Add(line);
				Canvas.SetLeft(line, 0);
				Canvas.SetTop(line, visualOffset);
			}

			AlignRulers();
		}

		private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			AlignRulers();
		}
	}
}