using Designer.Controls;
using Designer.Tools;
using iText.IO.Image;
using iText.Kernel.Font;
using JdSuite.Common.Logging;
using JdSuite.Common.Logging.Enums;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Designer
{
	public static class ITextExtensions
	{
		public const float POINTS_PER_INCH = 72f;

		public static iText.Layout.Element.Image ToIText(this CanvasImage canvasImage)
		{
			var info = new UIElementInfo(canvasImage.Image, canvasImage.Canvas);

			var src = ImageDataFactory.Create(canvasImage.ImagePath);
			var image = new iText.Layout.Element.Image(src, info.Left, info.Bottom);
			image.SetFixedPosition(info.Left, info.Bottom);
			image.ScaleAbsolute(info.ElementWidth, info.ElementHeight);

			return image;
		}

		public static iText.Layout.Element.Paragraph ToIText(this CanvasRichTextBox canvasTextBox)
		{
			
			var info = new UIElementInfo(canvasTextBox.TextBox, canvasTextBox.Canvas);
			var text = new TextRange(canvasTextBox.TextBox.Document.ContentStart,
					canvasTextBox.TextBox.Document.ContentEnd).Text;
			var textBox = new iText.Layout.Element.Text(text);

			textBox.SetFontSize((float)(canvasTextBox.TextBox.FontSize.ToITextPoint()));
			textBox.SetFont(GetFont(canvasTextBox.TextBox.FontFamily.Source));
			var paragraph = new iText.Layout.Element.Paragraph(textBox);
			float magicPadding = 17f; // Some sort of rtf padding that is hard to pin-point 
			paragraph.SetFixedPosition(info.Left, info.Bottom - magicPadding, iText.Layout.Properties.UnitValue.CreatePercentValue(100));
			paragraph.SetMultipliedLeading(0.9f);
			paragraph.SetWidth(info.ElementWidth * 1.3f);
			paragraph.SetHeight(info.ElementHeight * 1.3f);
			return paragraph;
		}

		public static iText.Layout.Element.Paragraph ToIText(this CanvasVariable csd)
		{
			var info = new UIElementInfo(csd.Label, csd.Canvas);
			var textBox = new iText.Layout.Element.Text(csd.Label.Content as string);
			textBox.SetFontSize((float)(csd.Label.FontSize.ToITextPoint()));
			textBox.SetFont(GetFont(csd.Label.FontFamily.Source));
			var paragraph = new iText.Layout.Element.Paragraph(textBox);
			paragraph.SetFixedPosition(info.Left, info.Bottom, iText.Layout.Properties.UnitValue.CreatePercentValue(100));
			return paragraph;
		}

		public static List<iText.Layout.Element.IElement> ToIText(this CanvasControl control)
		{
			List<iText.Layout.Element.IElement> list = new List<iText.Layout.Element.IElement>();

			if (control is CanvasRichTextBox rtb) {
				list.Add(rtb.ToIText());
			}
			else if (control is CanvasImage image) {
				list.Add(image.ToIText());
			}
			else if (control.GetType().IsSubclassOf(typeof(CanvasVariable))) {
				CanvasVariable cv = control as CanvasVariable;
				list.Add(cv.ToIText());
			}
			else {
				Logger.Log(Severity.ERROR, LogCategory.CONTROL,
					"Conversion from " + control.GetType().ToString() +
					" to iText element array not implemented.");
			}

			return list;
		}

		/// <summary>
		/// Converts WPF points to iText points
		/// </summary>
		/// <param name="num">The number to convert.</param>
		/// <returns>An iText point.</returns>
		public static float ToITextPoint(this double num)
		{
			float inches = (float)(num / ImageHelper.DPI);
			return inches * POINTS_PER_INCH;
		}

		/// <summary>
		/// Attempts to retrieve the specified font from the System's Fonts folder
		/// and registers the font with WPF.
		/// </summary>
		/// <param name="fontName">Name of the font.</param>
		/// <returns></returns>
		private static PdfFont GetFont(string fontName)
		{
			if (!PdfFontFactory.IsRegistered(fontName)) {
				var fontPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\" + fontName.Replace(" ", "") + ".ttf";
				PdfFontFactory.Register(fontPath);
			}

			return PdfFontFactory.CreateRegisteredFont(fontName);
		}

		private class UIElementInfo
		{
			public float Bottom;
			public float ElementHeight;
			public float ElementWidth;
			public float Left;
			public float PageHeight;
			public float PageWidth;
			public float Top;

			public UIElementInfo(FrameworkElement control, Canvas canvas)
			{
				var offset = control.TransformToAncestor(canvas).Transform(new Point(0, 0));
				Left = offset.X.ToITextPoint();
				Top = offset.Y.ToITextPoint();
				PageWidth = canvas.ActualWidth.ToITextPoint();
				PageHeight = canvas.ActualHeight.ToITextPoint();
				if (control.LayoutTransform != null) {
					ElementWidth = control.LayoutTransform.Transform(new Point(control.ActualWidth, 0)).X.ToITextPoint();
					ElementHeight = control.LayoutTransform.Transform(new Point(0, control.ActualHeight)).Y.ToITextPoint();
				}
				else {
					ElementWidth = control.ActualWidth.ToITextPoint();
					ElementHeight = control.ActualHeight.ToITextPoint();
				}
				Bottom = PageHeight - Top - ElementHeight;
			}
		}
	}
}