using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Designer.Tools
{
	public static class VisualHelper
	{
		public static T FindParentOfType<T>(this DependencyObject child) where T : DependencyObject
		{
			DependencyObject parentDepObj = child;
			do {
				parentDepObj = VisualTreeHelper.GetParent(parentDepObj);
				T parent = parentDepObj as T;
				if (parent != null) return parent;
			}
			while (parentDepObj != null);
			return null;
		}

		/// <summary>
		/// Gets the absolute position.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <param name="relativeToScreen">if set to <c>true</c> [relative to screen].</param>
		/// <returns></returns>
		public static Rect GetAbsolutePosition(this FrameworkElement element, bool relativeToScreen = false)
		{
			var absolutePos = element.PointToScreen(new System.Windows.Point(0, 0));
			if (relativeToScreen) {
				return new Rect(absolutePos.X, absolutePos.Y, element.ActualWidth, element.ActualHeight);
			}
			var posMW = Application.Current.MainWindow.PointToScreen(new System.Windows.Point(0, 0));
			absolutePos = new System.Windows.Point(absolutePos.X - posMW.X, absolutePos.Y - posMW.Y);
			return new Rect(absolutePos.X, absolutePos.Y, element.ActualWidth, element.ActualHeight);
		}

		public static Rect GetBoundingBox(FrameworkElement element)
		{
			return element.RenderTransform.TransformBounds(new Rect(element.RenderSize)); ;
		}

		public static ScaleTransform GetImageScaleWithBounds(double maxSize, BitmapImage image)
		{
			double constrainedWidth;
			double constrainedHeight;
			double widthAspect = image.PixelWidth / (double)image.PixelHeight;
			double heightAspect = image.PixelHeight / (double)image.PixelWidth;
			if (image.PixelWidth > image.PixelHeight) {
				constrainedWidth = maxSize;
				constrainedHeight = maxSize * heightAspect;
			}
			else {
				constrainedWidth = maxSize * widthAspect;
				constrainedHeight = maxSize;
			}

			return new ScaleTransform(
					constrainedWidth / image.PixelWidth,
					constrainedHeight / image.PixelHeight
				);
		}

		public static bool IsMouseOver(UIElement element, UIElement reference)
		{
			return IsPointOver(Mouse.GetPosition(reference), element, reference);
		}

		public static bool IsPointOver(Point point, UIElement element, Visual reference)
		{
			bool isOver = false;
			VisualTreeHelper.HitTest(Window.GetWindow(reference), null, (HitTestResult result) =>
			{
				if (result.VisualHit == element) {
					isOver = true;
					return HitTestResultBehavior.Stop;
				}
				return HitTestResultBehavior.Continue;
			}, new PointHitTestParameters(point));

			return isOver;
		}

		public static void RemoveChild(this DependencyObject parent, UIElement child)
		{
			var panel = parent as Panel;
			if (panel != null) {
				panel.Children.Remove(child);
				return;
			}

			var decorator = parent as Decorator;
			if (decorator != null) {
				if (decorator.Child == child) {
					decorator.Child = null;
				}
				return;
			}

			var contentPresenter = parent as ContentPresenter;
			if (contentPresenter != null) {
				if (contentPresenter.Content == child) {
					contentPresenter.Content = null;
				}
				return;
			}

			var contentControl = parent as ContentControl;
			if (contentControl != null) {
				if (contentControl.Content == child) {
					contentControl.Content = null;
				}
				return;
			}

			// maybe more
		}
	}

	public class ImageHelper
	{
		/// <summary>
		/// GUID of the component registration group for WIC decoders
		/// </summary>
		private const string WICDecoderCategory = "{7ED96837-96F0-4812-B211-F13C24117ED3}";

		/// <summary>
		/// Gets a list of additionally registered WIC decoders
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<DecoderInfo> GetAdditionalDecoders()
		{
			var result = new List<DecoderInfo>();

			string baseKeyPath;

			// If we are a 32 bit process running on a 64 bit operating system,
			// we find our config in Wow6432Node subkey
			if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess) {
				baseKeyPath = "Wow6432Node\\CLSID";
			}
			else {
				baseKeyPath = "CLSID";
			}

			RegistryKey baseKey = Registry.ClassesRoot.OpenSubKey(baseKeyPath, false);
			if (baseKey != null) {
				var categoryKey = baseKey.OpenSubKey(WICDecoderCategory + "\\instance", false);
				if (categoryKey != null) {
					// Read the guids of the registered decoders
					var codecGuids = categoryKey.GetSubKeyNames();

					foreach (var codecGuid in codecGuids) {
						// Read the properties of the single registered decoder
						var codecKey = baseKey.OpenSubKey(codecGuid);
						if (codecKey != null) {
							DecoderInfo decoderInfo = new DecoderInfo();
							decoderInfo.FriendlyName = Convert.ToString(codecKey.GetValue("FriendlyName", ""));
							decoderInfo.FileExtensions = Convert.ToString(codecKey.GetValue("FileExtensions", ""));
							result.Add(decoderInfo);
						}
					}
				}
			}

			return result;
		}

		public const double DPI = 96d;	

		/// <summary>
		/// Represents information about a WIC decoder
		/// </summary>
		public struct DecoderInfo
		{
			public string FileExtensions;
			public string FriendlyName;
		}
	}
}