using Designer.Tools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Designer.Controls
{
	public static class PlaceholderImages
	{
		public static WriteableBitmap image; // A blank one pixel image

		static PlaceholderImages()
		{
			string path = ConfigurationManager.ConnectionStrings["IconDirectory"]
					.ConnectionString;
			path = Path.Combine(path, "placeholder.png");
			path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);

			image = new WriteableBitmap(1, 1, 96, 96, PixelFormats.Cmyk32, BitmapPalettes.Halftone256);
			var rect = new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight);
			byte[] colorData = { 0, 0, 0, 0 }; // CMYK Light Gray
			image.WritePixels(rect, colorData, 4, 0);
			image.Freeze();
		}
	}

	public abstract class CanvasControl : BasicLayoutNode
	{
		protected enum DragZone
		{
			TOP_LEFT,
			TOP_RIGHT,
			BOTTOM_RIGHT,
			BOTTOM_LEFT,
			TOP_EDGE,
			RIGHT_EDGE,
			BOTTOM_EDGE,
			LEFT_EDGE
		}

		/// <summary> The root element of the canvas control </summary>
		public Border Border;

		public Canvas Canvas;

		public bool isDraggingBorder = false;

		protected DragZone BorderDragZone;

		/// <summary> Define drag area based on units away from the border </summary>
		private static readonly double DRAG_RADIUS = 10;

		private string ControlName;

		private int id;

		private Xceed.Wpf.Toolkit.DecimalUpDown XUpDown = new Xceed.Wpf.Toolkit.DecimalUpDown();

		private Xceed.Wpf.Toolkit.DecimalUpDown YUpDown = new Xceed.Wpf.Toolkit.DecimalUpDown();

		public CanvasControl(string controlName, int id) : base(controlName + " " + id.ToString())
		{
			this.id = id;
			ControlName = controlName;
			Border = ControlFactory.GenerateBorder();
			Border.PreviewMouseMove += Border_MouseMove;
			Border.MouseLeftButtonDown += Border_MouseLeftButtonDown;
			Border.PreviewMouseLeftButtonDown += Border_PreviewMouseLeftButtonDown;

			XUpDown.ValueChanged += delegate
			{
				Canvas.SetLeft(Border, (double)XUpDown.Value.GetValueOrDefault(0));
			};

			YUpDown.ValueChanged += delegate
			{
				Canvas.SetTop(Border, (double)YUpDown.Value.GetValueOrDefault(0));
			};

			AddProperty(new LayoutNodeProperty("X", XUpDown));
			AddProperty(new LayoutNodeProperty("Y", YUpDown));
		}

		/// <summary>
		/// Determines whether this instance contains the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>
		///   <c>true</c> if this instance contains the specified element; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool Contains(UIElement element)
		{
			if (element == Border) {
				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets the elements assigned to this control.
		/// </summary>
		/// <returns></returns>
		/// <summary>
		/// Gets the property layout associated with this canvas control.
		/// </summary>
		/// <returns>The property layout associated with this canvas control.</returns>
		public abstract List<FrameworkElement> GetElements();

		public override void ReadXml(XmlReader reader)
		{
			base.ReadXml(reader);
			var left = double.Parse(reader.GetAttribute("Left"));
			var top = double.Parse(reader.GetAttribute("Top"));

			SetPosition(new Point(left, top));
		}

		/// <summary>
		/// Removes this control from the previous canvas.
		/// </summary>
		public void RemoveFromCanvas()
		{
			this.Canvas = null;
			var parent = Border.Parent as Canvas;
			parent?.Children.Remove(Border);
		}

		/// <summary>
		/// Sets the canvas for this control.
		/// Will remove itself from the current canvas if it is attached to one.
		/// </summary>
		/// <param name="parent">The new parent canvas.</param>
		public void SetCanvas(Canvas parent)
		{
			if (Border.Parent != parent) {
				if (Border.Parent != null) {
					RemoveFromCanvas();
				}
				this.Canvas = parent;
				parent.Children.Add(Border);
			}
		}

		/// <summary>
		/// Sets the id for this control, while including the control name in the resulting ID.
		/// </summary>
		/// <param name="id">The identifier.</param>
		public void SetId(int id)
		{
			this.id = id;
			Id = ControlName + " " + id.ToString();
		}

		/// <summary>
		/// Sets the internal name of the control. This does not always correspond to the user-facing ID.
		/// </summary>
		/// <param name="name">The name.</param>
		public void SetName(string name)
		{
			ControlName = name;
			SetId(id);
		}

		/// <summary>
		/// Sets the position of this control on the canvas.
		/// </summary>
		/// <param name="relPos">The relative position.</param>
		public void SetPosition(Point relPos)
		{
			Canvas.SetLeft(Border, relPos.X);
			Canvas.SetTop(Border, relPos.Y);
			XUpDown.Value = (Decimal)Math.Truncate(1000 * relPos.X) / 1000;
			YUpDown.Value = (Decimal)Math.Truncate(1000 * relPos.Y) / 1000;
		}

		/// <summary>
		/// Stops dragging the border for this control. Used by Designer
		/// when the mouse is not over this control but is still being dragged.
		/// </summary>
		public void StopDraggingBorder()
		{
			if (isDraggingBorder) {
				isDraggingBorder = false;
				Canvas.MouseMove -= CanvasControl_MouseMove;
				Canvas.PreviewMouseLeftButtonUp -= CanvasControl_PreviewMouseLeftButtonUp;
			}
		}

		public override void WriteXml(XmlWriter writer)
		{
			base.WriteXml(writer);
			writer.WriteAttributeString("Left", Canvas.GetLeft(Border).ToString());
			writer.WriteAttributeString("Top", Canvas.GetTop(Border).ToString());
		}

		protected abstract Point GetRenderedSize();

		/// <summary>
		/// Notifies the child control to resize when the parent border is dragged.
		/// </summary>
		/// <param name="newWidth">The new width.</param>
		/// <param name="newHeight">The new height.</param>
		protected abstract void OnResize(double newWidth, double newHeight);

		/// <summary>
		/// Called when the mouse is click-dragged across the parent border
		/// </summary>
		/// <param name="mousePos">The mouse position relative to the canvas.</param>
		protected virtual void OnResizeBorder(Point mousePos)
		{
			var topLeftPos = Border.TransformToAncestor(Canvas).Transform(new Point(0, 0));
			var childSize = GetRenderedSize();
			var bottomRightPos = Point.Add(childSize, (Vector)topLeftPos);
			Rect newBounds = new Rect();
			try {
				var bounds = new Rect(topLeftPos, bottomRightPos);
				switch (BorderDragZone) {
					case DragZone.TOP_LEFT:
						newBounds = new Rect(mousePos, bounds.BottomRight);
						break;

					case DragZone.TOP_RIGHT:
						newBounds = new Rect(bounds.BottomLeft, mousePos);
						break;

					case DragZone.BOTTOM_RIGHT:
						newBounds = new Rect(bounds.TopLeft, mousePos);
						break;

					case DragZone.BOTTOM_LEFT:
						newBounds = new Rect(bounds.TopRight, mousePos);
						break;

					case DragZone.TOP_EDGE:
						var newHeight = bounds.Height + (bounds.Top - mousePos.Y);
						newBounds = bounds;
						newBounds.Height = newHeight;
						newBounds.Y = mousePos.Y;
						break;

					case DragZone.RIGHT_EDGE:
						var newWidth = bounds.Width + (mousePos.X - bounds.Right);
						newBounds = bounds;
						newBounds.Width = newWidth;
						break;

					case DragZone.BOTTOM_EDGE:
						newHeight = bounds.Height + (mousePos.Y - bounds.Bottom);
						newBounds = bounds;
						newBounds.Height = newHeight;
						break;

					case DragZone.LEFT_EDGE:
						newWidth = bounds.Width + (bounds.Left - mousePos.X);
						newBounds = bounds;
						newBounds.Width = newWidth;
						newBounds.X = mousePos.X;
						break;

					default:
						return;
				}
			}
			// assigning a possible negative value to Rect throws an exception
			// If the value is negative, disregard resize action
			catch (ArgumentException) {
				return;
			}

			OnResize(newBounds.Width, newBounds.Height);
			Canvas.SetLeft(Border, newBounds.X);
			Canvas.SetTop(Border, newBounds.Y);
		}

		private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Canvas.PreviewMouseLeftButtonUp += CanvasControl_PreviewMouseLeftButtonUp;
			e.Handled = true;
		}

		private void Border_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed) {
				// Determine which side of the border is being dragged
				if (!isDraggingBorder) {
					var relMousePos = e.GetPosition(Border);

					// Calculate which edge is being dragged
					var topEdgeOffset = Math.Abs(relMousePos.Y);
					var bottomEdgeOffset = Math.Abs(Border.ActualHeight - relMousePos.Y);
					var leftEdgeOffset = Math.Abs(relMousePos.X);
					var rightEdgeOffset = Math.Abs(Border.ActualWidth - relMousePos.X);

					if (leftEdgeOffset < DRAG_RADIUS && topEdgeOffset < DRAG_RADIUS) {
						BorderDragZone = DragZone.TOP_LEFT;
					}
					else if (rightEdgeOffset < DRAG_RADIUS && topEdgeOffset < DRAG_RADIUS) {
						BorderDragZone = DragZone.TOP_RIGHT;
					}
					else if (rightEdgeOffset < DRAG_RADIUS && bottomEdgeOffset < DRAG_RADIUS) {
						BorderDragZone = DragZone.BOTTOM_RIGHT;
					}
					else if (leftEdgeOffset < DRAG_RADIUS && bottomEdgeOffset < DRAG_RADIUS) {
						BorderDragZone = DragZone.BOTTOM_LEFT;
					}
					else if (topEdgeOffset < DRAG_RADIUS) {
						BorderDragZone = DragZone.TOP_EDGE;
					}
					else if (rightEdgeOffset < DRAG_RADIUS) {
						BorderDragZone = DragZone.RIGHT_EDGE;
					}
					// Bottom edge
					else if (bottomEdgeOffset < DRAG_RADIUS) {
						BorderDragZone = DragZone.BOTTOM_EDGE;
					}
					// Left edge
					else if (leftEdgeOffset < DRAG_RADIUS) {
						BorderDragZone = DragZone.LEFT_EDGE;
					}
					else {
						return;
					}

					isDraggingBorder = true;
					Canvas.MouseMove += CanvasControl_MouseMove;
					return;
				}
			}
		}

		private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			MainWindow.Instance.DesignerControl.SetActiveProperties(GetPropertyLayout());
		}

		private void CanvasControl_MouseMove(object sender, MouseEventArgs e)
		{
			OnResizeBorder(e.GetPosition(Canvas));
		}

		private void CanvasControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			StopDraggingBorder();
		}
	}
    public class CanvasElement : CanvasControl
    {
        public System.Windows.Shapes.Rectangle Element;
 
        public double Width
        {
            get => Element.Width;
            set => Element.Width = value;
        }

        public double Height
        {
            get => Element.Height;
            set => Element.Height = value;
        }

        Xceed.Wpf.Toolkit.DecimalUpDown wUpDown = new Xceed.Wpf.Toolkit.DecimalUpDown();
        Xceed.Wpf.Toolkit.DecimalUpDown hUpDown = new Xceed.Wpf.Toolkit.DecimalUpDown();

        public CanvasElement() : this(-1) { }
        public CanvasElement(int id) : base("Element", id)
        {
            Element = new System.Windows.Shapes.Rectangle();
            Width = 200;
            Height = 200;
            Border.Child = Element;

            wUpDown.Value = (Decimal)Width;
            hUpDown.Value = (Decimal)Height;

            wUpDown.ValueChanged += delegate
            {
                Width = (double)wUpDown.Value.GetValueOrDefault(0);
            };

            hUpDown.ValueChanged += delegate
            {
                Height = (double)hUpDown.Value.GetValueOrDefault(0);
            };

            AddProperty(new LayoutNodeProperty("Width:", wUpDown, "mm"));
            AddProperty(new LayoutNodeProperty("Height:", hUpDown, "mm"));

        }

        public override TvItemCategory TreeViewCategory => TvItemCategory.ELEMENTS;

        public override string TreeViewName => "Element";

        public override bool Contains(UIElement element)
        {
            if (Element == element)
                return true;
            return base.Contains(element);
        }

        public override List<FrameworkElement> GetElements()
        {
            List<FrameworkElement> list = new List<FrameworkElement> {
                Element
            };
            return list;
        }

        protected override Point GetRenderedSize()
        {
            return new Point(Element.Width, Element.Height);
        }

        protected override void OnResize(double newWidth, double newHeight)
        {
            Width = newWidth;
            Height = newHeight;
            wUpDown.Value = (decimal)newWidth;
            hUpDown.Value = (decimal)newHeight;
        }

        protected override void OnResizeBorder(Point mousePos)
        {
            base.OnResizeBorder(mousePos);
        }
    }

    public class CanvasImage : CanvasControl
	{
		public bool AspectRatioLocked = false;
		public Image Image;
		public string ImagePath = "";
		public ScaleTransform Transform;
		private Button BrowseButton = new Button();

		/// <summary> Empty constructor for XML Loading.
		/// Do not use for other purposes </summary>
		public CanvasImage() : this(-1) { }

		public CanvasImage(int id) : base("Image", id)
		{
			Image = new Image();
			Image.Source = PlaceholderImages.image;
			Transform = new ScaleTransform(100, 100);
			Image.LayoutTransform = Transform;
			Border.Child = Image;

			BrowseButton.HorizontalAlignment = HorizontalAlignment.Left;
			BrowseButton.Content = "Browse";
			BrowseButton.Click += ImgSrc_Click;

			AddProperty(new LayoutNodeProperty("Image Source", BrowseButton));

			{
				var element = new CheckBox();
				element.Checked += delegate
				{
					AspectRatioLocked = true;
					OnResize(Image.ActualWidth, Image.ActualHeight);
				};
				element.Unchecked += delegate { AspectRatioLocked = false; };

				AddProperty(new LayoutNodeProperty("Lock Aspect Ratio", element));
			}
		}

		public override TvItemCategory TreeViewCategory => TvItemCategory.ELEMENTS;

		public override string TreeViewName => "Image";

		public override bool Contains(UIElement element)
		{
			if (Image == element) {
				return true;
			}

			return base.Contains(element);
		}

		public override List<FrameworkElement> GetElements()
		{
			List<FrameworkElement> list = new List<FrameworkElement> {
				Image
			};
			return list;
		}

		public override void ReadXml(XmlReader reader)
		{
			base.ReadXml(reader);
			var ImageSrc = reader.GetAttribute("ImageSrc");
			var ImageScaleX = double.Parse(reader.GetAttribute("ImageScaleX"));
			var ImageScaleY = double.Parse(reader.GetAttribute("ImageScaleY"));
			var AspectLocked = bool.Parse(reader.GetAttribute("AspectLocked"));

			if (ImageSrc.Length > 0 && File.Exists(ImageSrc)) {
				ImportImage(ImageSrc);
			}
			Transform.ScaleX = ImageScaleX;
			Transform.ScaleY = ImageScaleY;
			AspectRatioLocked = AspectLocked;
		}

		public override void WriteXml(XmlWriter writer)
		{
			base.WriteXml(writer);
			writer.WriteAttributeString("ImageSrc", ImagePath);
			writer.WriteAttributeString("ImageScaleX", Transform.ScaleX.ToString());
			writer.WriteAttributeString("ImageScaleY", Transform.ScaleY.ToString());
			writer.WriteAttributeString("AspectLocked", AspectRatioLocked.ToString());
		}

		protected override Point GetRenderedSize()
		{
			return new Point(Image.DesiredSize.Width, Image.DesiredSize.Height);
		}

		protected override void OnResize(double newWidth, double newHeight)
		{
			if (AspectRatioLocked && Image.Source is BitmapImage) {
				double x, y;
				if (newWidth < newHeight) {
					x = newWidth / Image.Source.Width;
					y = newWidth / Image.Source.Height;
				}
				else {
					x = newHeight / Image.Source.Width;
					y = newHeight / Image.Source.Height;
				}

				Transform.ScaleX = x;
				Transform.ScaleY = y;
			}
			else {
				Transform.ScaleX = newWidth / Image.Source.Width;
				Transform.ScaleY = newHeight / Image.Source.Height;
			}
		}

		protected override void OnResizeBorder(Point mousePos)
		{
			if (AspectRatioLocked) {
				var size = GetRenderedSize();
				var topLeftPos = Border.TransformToAncestor(Canvas).Transform(new Point(0, 0));
				var topRightPos = new Point(topLeftPos.X + size.X, topLeftPos.Y);
				var bottomRightPos = Point.Add(topLeftPos, (Vector)size);
				var bottomLeftPos = new Point(topLeftPos.X, topLeftPos.Y + size.Y);

				// Due to the nature of locked aspect, disable dragging from
				// the edges
				switch (BorderDragZone) {
					case DragZone.LEFT_EDGE:
						return;

					case DragZone.TOP_EDGE:
						return;

					case DragZone.RIGHT_EDGE:
						return;

					case DragZone.BOTTOM_EDGE:
						return;
				}

				base.OnResizeBorder(mousePos);
				Border.UpdateLayout();

				var newSize = GetRenderedSize();

				// Reinforce anchor point based on where it was dragged from
				// since a locked aspect ratio doesn't always follow the mouse
				switch (BorderDragZone) {
					case DragZone.TOP_LEFT:
						Canvas.SetLeft(Border, bottomRightPos.X - newSize.X);
						Canvas.SetTop(Border, bottomRightPos.Y - newSize.Y);
						break;

					case DragZone.TOP_RIGHT:
						Canvas.SetLeft(Border, bottomLeftPos.X);
						Canvas.SetTop(Border, bottomLeftPos.Y - newSize.Y);
						break;

					case DragZone.BOTTOM_RIGHT:
						// Do nothing
						break;

					case DragZone.BOTTOM_LEFT:
						Canvas.SetLeft(Border, topRightPos.X - newSize.X);
						Canvas.SetTop(Border, topRightPos.Y);
						break;

					default:
						break;
				}
			}
			else {
				base.OnResizeBorder(mousePos);
			}
		}

		private void ImgSrc_Click(object sender, RoutedEventArgs e)
		{
			// Create OpenFileDialog
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

			// Set filter for file extension and default file extension
			dlg.DefaultExt = ".png";

			string descriptions = "Image files (*.bmp, *.gif, *.ico, *.jpeg, *.png, *.tiff, *.dds, *.wdp, ";
			string extensions = "*.bmp;*.gif;*.ico;*.jpeg;*.png;*.tiff;*.dds;*.wdp;";
			foreach (var decoder in ImageHelper.GetAdditionalDecoders()) {
				string baseStr = decoder.FileExtensions.Replace(".", "*.").ToLower();
				extensions += baseStr.Replace(',', ';');
				descriptions += baseStr.Replace(",", ", ");
			}
			descriptions += ")|";
			dlg.Filter = descriptions + extensions;

			// Display OpenFileDialog by calling ShowDialog method
			bool? result = dlg.ShowDialog();

			// Get the selected file name and display in a TextBox
			if (result == true) {
				// Import image
				string fileName = dlg.FileName;
				ImportImage(fileName);
			}
		}

		private void ImportImage(string path)
		{
			var name = Path.GetFileName(path);

			if (!LoadImage(path)) {
				BrowseButton.BorderBrush = (SolidColorBrush)Window.GetWindow(BrowseButton).FindResource("ErrorInputBrush");
			}
			else {
				ImagePath = path;
				BrowseButton.Content = name;
				BrowseButton.BorderBrush = null;
			}
		}

		private bool LoadImage(string path)
		{
			try {
				Image.Source = new BitmapImage(new Uri(path));
			}
			catch (ArgumentNullException) {
				return false;
			}
			catch (FileNotFoundException) {
				return false;
			}
			catch (UriFormatException) {
				return false;
			}

			OnResize(Border.DesiredSize.Width, Border.DesiredSize.Height);
			return true;
		}
	}

	public class CanvasRichTextBox : CanvasControl
	{
		public RichTextBox TextBox = new RichTextBox();
		private LayoutNodeProperty ContentProp;
		private TextBox ContentPropTb = new TextBox();

		/// <summary> Empty constructor for XML Loading.
		/// Do not use for other purposes </summary>
		public CanvasRichTextBox() : this(-1) { }

		public CanvasRichTextBox(int id) : base("Rich TextBox", id)
		{
			TextBox.Document = new FlowDocument();
			TextBox.FontSize = TextBox.FontSize;
			TextBox.Width = 200;
			TextBox.Height = 75;
			SetTextBox(TextBox);

			ContentPropTb.IsReadOnly = true;
			ContentPropTb.HorizontalAlignment = HorizontalAlignment.Left;
			ContentPropTb.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
			ContentProp = new LayoutNodeProperty("Content", ContentPropTb);
			AddProperty(ContentProp);
		}

		public override TvItemCategory TreeViewCategory => TvItemCategory.ELEMENTS;

		public override string TreeViewName => "Text";

		public override bool Contains(UIElement element)
		{
			if (element == TextBox) {
				return true;
			}

			return base.Contains(element);
		}

		public override List<FrameworkElement> GetElements()
		{
			List<FrameworkElement> list = new List<FrameworkElement> {
				TextBox
			};
			return list;
		}

		public override void ReadXml(XmlReader reader)
		{
			base.ReadXml(reader);
			using (var sub = reader.ReadSubtree()) {
				sub.MoveToContent();
				SetTextBox((RichTextBox)XamlReader.Parse(sub.ReadInnerXml()));
			}
		}

		public override void WriteXml(XmlWriter writer)
		{
			base.WriteXml(writer);
			XamlWriter.Save(TextBox, writer);
		}

		protected override Point GetRenderedSize()
		{
			return new Point(TextBox.RenderSize.Width, TextBox.RenderSize.Height);
		}

		protected override void OnResize(double newWidth, double newHeight)
		{
			TextBox.Width = newWidth;
			TextBox.Height = newHeight;
		}

		public void SetBackgroundColor(Brush color)
		{
			TextBox.Background = color;
		}

		private void SetTextBox(RichTextBox textBox)
		{
			TextBox = textBox;
			Border.Child = TextBox;
			TextBox.TextChanged += delegate
			{
				ContentPropTb.Text = new TextRange(TextBox.Document.ContentStart,
					TextBox.Document.ContentEnd).Text;
			};
			ContentPropTb.Text = new TextRange(TextBox.Document.ContentStart,
					TextBox.Document.ContentEnd).Text;
		}
	}

	/// <summary>
	/// A control used to show abstract representations of variables
	/// </summary>
	/// <seealso cref="Designer.Controls.CanvasControl" />
	public abstract class CanvasVariable : CanvasControl
	{
		public Label Label = new Label();
		private Xceed.Wpf.Toolkit.DecimalUpDown UpDownProp = new Xceed.Wpf.Toolkit.DecimalUpDown();

		public override string TreeViewName => Label.Content as string;

		/// <summary> Empty constructor for XML Loading.
		/// Do not use for other purposes </summary>
		public CanvasVariable() : this(-1, "Variable") { }

		public CanvasVariable(int id, string name) : base(name, id)
		{
			Border.BorderBrush = Brushes.Gray;
			Border.Child = Label;
			Label.Content = name;
			UpDownProp.Value = (Decimal)Label.FontSize;
			UpDownProp.ValueChanged += delegate
			{
				Label.FontSize = Decimal.ToDouble(UpDownProp.Value.Value);
			};

			AddProperty(new LayoutNodeProperty("Font Size", UpDownProp));
		}

		public string GetSchemaElementName()
		{
			return Label.Content as string;
		}

		public override List<FrameworkElement> GetElements()
		{
			return new List<FrameworkElement>
			{
				Label
			};
		}

		public override void ReadXml(XmlReader reader)
		{
			base.ReadXml(reader);
			Label.Content = XmlConvert.DecodeName(reader.GetAttribute("Content"));
			Label.FontSize = double.Parse(reader.GetAttribute("FontSize"));
		}

		public override void WriteXml(XmlWriter writer)
		{
			base.WriteXml(writer);
			writer.WriteAttributeString("Content", XmlConvert.EncodeNmToken((string)Label.Content));
			writer.WriteAttributeString("FontSize", Label.FontSize.ToString());
		}

		protected override Point GetRenderedSize()
		{
			return new Point(Label.RenderSize.Width, Label.RenderSize.Height);
		}

		protected override void OnResize(double newWidth, double newHeight)
		{
			Label.Width = newWidth;
			Label.Height = newHeight;
		}

		protected override void OnResizeBorder(Point mousePos)
		{
			base.OnResizeBorder(mousePos);
		}
	}

	public class CanvasVariableData : CanvasVariable
	{
		public override TvItemCategory TreeViewCategory => TvItemCategory.VARIABLE_DATA;
	}

	public class CanvasVariableSystem : CanvasVariable
	{
		public override TvItemCategory TreeViewCategory => TvItemCategory.VARIABLE_SYSTEM;
	}

	internal static class ControlFactory
	{
		public static Border GenerateBorder()
		{
			Border border = new Border();
			border.Style = (Style)MainWindow.Instance.DesignerControl.FindResource("CanvasElementBorder");
			return border;
		}
	}
}