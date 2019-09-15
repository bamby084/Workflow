using Designer.Controls;
using Designer.Tools;
using JdSuite.Common.Logging;
using JdSuite.Common.Logging.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Designer
{
	public interface ILayoutNode : IXmlSerializable, IDisposable, INotifyPropertyChanged
	{
		/// <summary>
		/// Gets or sets the user-editable identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		string Id { get; set; }

		/// <summary>
		/// Gets the MenuItem for this instance for the Layout Tree context menu.
		/// </summary>
		/// <value>
		/// The menu item.
		/// </value>
		MenuItem MenuItem { get; }

		/// <summary>
		/// Gets the TreeView category.
		/// </summary>
		/// <value>
		/// The TreeView category.
		/// </value>
		TvItemCategory TreeViewCategory { get; }

		/// <summary>
		/// Gets the header of the related TreeViewItem for context menu usage.
		/// </summary>
		/// <value>
		/// The header of the TreeViewItem.
		/// </value>
		string TreeViewName { get; }

		/// <summary>
		/// Event called when the instance is removed from a canvas.
		/// </summary>
		Action OnRemove { get; set; }

		/// <summary>
		/// Gets the property layout for display in the Designer properties panel.
		/// </summary>
		/// <returns>The property layout</returns>
		FrameworkElement GetPropertyLayout();
	}

	public static class PageExtensions
	{
		/// <summary>
		/// Instantiates a new Pages class with the exact state of the current instance.
		/// </summary>
		/// <param name="pages">The pages to copy.</param>
		/// <returns>A complete copy of the pages from the parameter.</returns>
		public static Pages DeepCopy(this Pages pages)
		{
			var xmlSerializer = new XmlSerializer(typeof(Pages));

			using (var stream = new MemoryStream()) {
				xmlSerializer.Serialize(stream, pages);
				stream.Position = 0;
				return (Pages)xmlSerializer.Deserialize(stream);
			}
		}
	}

	public abstract class BasicLayoutNode : ILayoutNode
	{
		private Action _OnRemove;
		private string id;
		private TabControl LayoutRoot = new TabControl();
		private Grid PropertyGrid = new Grid();
		private StackPanel StackPanel = new StackPanel();
		private TabItem Tab = new TabItem();
		private MenuItem _MenuItem;

		public string Id {
			get => id;
			set {
				id = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Id"));
			}
		}

		public Action OnRemove { get => _OnRemove; set => _OnRemove = value; }
		public MenuItem MenuItem { get => _MenuItem; }
		public abstract TvItemCategory TreeViewCategory { get; }
		public abstract string TreeViewName { get; }

		public event PropertyChangedEventHandler PropertyChanged;

		public BasicLayoutNode(string id)
		{
			Id = id;
			_MenuItem = new MenuItem();
			{
				Binding binding = new Binding("Id");
				binding.Source = this;
				binding.Mode = BindingMode.OneWay;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				_MenuItem.SetBinding(MenuItem.HeaderProperty, binding);
			}

			LayoutRoot.Background = (SolidColorBrush)MainWindow.Instance.FindResource("AppBgColor");
			Tab.Header = "General";
			LayoutRoot.Items.Add(Tab);

			PropertyGrid.Background = (SolidColorBrush)MainWindow.Instance.FindResource("AppBgColor");
			PropertyGrid.ColumnDefinitions.Add(new ColumnDefinition
			{
				Width = new GridLength(0, GridUnitType.Auto)
			});
			PropertyGrid.ColumnDefinitions.Add(new ColumnDefinition
			{
				Width = new GridLength(200, GridUnitType.Pixel)
			});
            PropertyGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(0, GridUnitType.Auto)
            });

            {
				var textBox = new TextBox();
				Binding binding = new Binding("Id");
				binding.Source = this;
				binding.Mode = BindingMode.TwoWay;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				textBox.SetBinding(TextBox.TextProperty, binding);
				AddProperty(new LayoutNodeProperty("ID", textBox));
			}

			StackPanel.Children.Add(PropertyGrid);
			Tab.Content = StackPanel;
			LayoutRoot.SelectedIndex = 0;
		}

		public FrameworkElement GetPropertyLayout()
		{
			return LayoutRoot;
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public virtual void ReadXml(XmlReader reader)
		{
			Id = XmlConvert.DecodeName(reader.GetAttribute("Id"));
		}

		public virtual void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("Id", XmlConvert.EncodeName(Id));
		}

		/// <summary>
		/// Releases unmanaged and managed resources. Classes that inherit from
		/// this class are responsible for calling this before the object
		/// goes out of scope.
		/// </summary>
		void IDisposable.Dispose()
		{
			OnRemove?.Invoke();
		}

		protected void AddProperty(LayoutNodeProperty property)
		{
			PropertyGrid.RowDefinitions.Add(new RowDefinition());

			int row = PropertyGrid.RowDefinitions.Count - 1;
			Grid.SetRow(property.Name, row);
			Grid.SetColumn(property.Name, 0);
			property.Name.Margin = new Thickness(0, 0, 5, 0);
			PropertyGrid.Children.Add(property.Name);

			Grid.SetRow(property.Element, row);
			Grid.SetColumn(property.Element, 1);
			property.Element.VerticalAlignment = VerticalAlignment.Center;
			property.Element.HorizontalAlignment = HorizontalAlignment.Left;
            property.Element.Width = 200;
			PropertyGrid.Children.Add(property.Element);

            if (property.Unit != null)
            {
                Grid.SetRow(property.Unit, row);
                Grid.SetColumn(property.Unit, 2);
                property.Unit.VerticalAlignment = VerticalAlignment.Center;
                property.Unit.HorizontalAlignment = HorizontalAlignment.Left;
                property.Unit.HorizontalContentAlignment = HorizontalAlignment.Left;
                PropertyGrid.Children.Add(property.Unit);
            }
		}
	}

	public class LayoutNodeProperty
	{
		public FrameworkElement Element;
		public Label Name;
        public Label Unit = null;

		public LayoutNodeProperty(string displayName, FrameworkElement element)
		{
			Name = new Label();
			Name.Content = displayName;
			Element = element;
		}

        public LayoutNodeProperty(string displayName, FrameworkElement element, string unit)
        {
            Name = new Label();
            Name.Content = displayName;
            Element = element;

            Unit = new Label();
            Unit.Content = unit;
        }
    }

	public class Page : PageBase
	{
		/// <summary> Empty constructor for XML Loading.
		/// Do not use for other purposes </summary>
		public Page() : this(-1) { }

		public Page(int id) : base(id)
        {
            _LayoutProperties.TabContainer.Visibility = Visibility.Collapsed;
        }

		/// <summary>
		/// Constructor used for quick-cloning temporary pages during processing.
		/// </summary>
		/// <param name="pageProperties">The page properties panel.</param>
		public Page(PagePropertiesPanel pageProperties) : this(-1)
		{
			_LayoutProperties = pageProperties;
            _LayoutProperties.TabContainer.Visibility = Visibility.Collapsed;
        }

		public override string TreeViewName => "Page";
	}

	public abstract class PageBase : ILayoutNode
	{
	    public enum NextPageCondition
		{
			[Description("Next Page is disabled or no page is set.")]
			FALSE,
			[Description("Next Page is readily available through Simple.")]
			TRUE,
			[Description("Next Page is variable and indeterminate at this point.")]
			UNKNOWN
		};
		public const double INCHES_PER_MM = 0.03937008d;
		public const double MM_PER_INCH = 25.4d;
		public Border Border = new Border();

		public Canvas Canvas = new Canvas()
		{
			LayoutTransform = new ScaleTransform()
		};

		public ObservableCollection<CanvasControl> CanvasControls = new ObservableCollection<CanvasControl>();

        private string _Id = "";
		private Action _OnRemove;
		private double _PageHeight = 0;
		private double _PageWidth = 0;
		protected PagePropertiesPanel _LayoutProperties;

		public string Id {
			get => _Id;
			set {
				_Id = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Id"));
			}
		}

		public ScaleTransform LayoutTransform {
			get {
				return (ScaleTransform)Canvas.LayoutTransform;
			}
		}

		public Action OnRemove { get => _OnRemove; set => _OnRemove = value; }

		/// <summary>
		/// Gets or sets the height of the page.
		/// </summary>
		/// <value>
		/// The height of the page in DPI.
		/// </value>
		public double PageHeight {
			get => _PageHeight;
			set {
				_PageHeight = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PageHeight"));
			}
		}

		/// <summary>
		/// Gets or sets the width of the page.
		/// </summary>
		/// <value>
		/// The width of the page in DPI.
		/// </value>
		public double PageWidth {
			get => _PageWidth;
			set {
				_PageWidth = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PageWidth"));
			}
		}

		public PagePropertiesPanel LayoutProperties { get => _LayoutProperties; }

		private MenuItem _MenuItem;
		public MenuItem MenuItem => _MenuItem;

		public TvItemCategory TreeViewCategory => TvItemCategory.PAGES;

		public abstract string TreeViewName { get; }

		public event PropertyChangedEventHandler PropertyChanged;

		public PageBase() : this(-1)
		{
		}

		public PageBase(int id)
		{
			_MenuItem = new MenuItem();
			{
				Binding binding = new Binding("Id");
				binding.Source = this;
				binding.Mode = BindingMode.OneWay;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				_MenuItem.SetBinding(MenuItem.HeaderProperty, binding);
				_MenuItem.Click += MenuItem_Click;
			}
			SetId(id);
			_LayoutProperties = new PagePropertiesPanel(this);
			Canvas.AllowDrop = true;
			Canvas.Background = Brushes.White;
			Canvas.PreviewMouseLeftButtonUp += Canvas_PreviewMouseLeftButtonUp;
			var DesignerControl = MainWindow.Instance.DesignerControl;
			Border.Style = (Style)DesignerControl.FindResource("DefaultBorder");

			Border.Child = Canvas;

			{
				var binding = new Binding("PageWidth");
				binding.Source = this;
				binding.Mode = BindingMode.TwoWay;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				Canvas.SetBinding(Canvas.WidthProperty, binding);
			}
			{
				var binding = new Binding("PageHeight");
				binding.Source = this;
				binding.Mode = BindingMode.TwoWay;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				Canvas.SetBinding(Canvas.HeightProperty, binding);
			}

			LayoutProperties.PropertyChanged += LayoutProperties_PropertyChanged;
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			MainWindow.Instance.DesignerControl.RequestSetActivePage(this);
		}

		/// <summary>
		/// Adds a control to the canvas. If the position is not already
		/// set on the control, the control will show up at (0,0).
		/// </summary>
		/// <param name="control">The control to add to the canvas.</param>
		public void AddCanvasItem(CanvasControl control)
		{
			CanvasControls.Add(control);
			var DesignerControl = MainWindow.Instance.DesignerControl;
			DesignerControl.SetActiveProperties(control.GetPropertyLayout());
			control.SetCanvas(Canvas);
		}


		/// <summary>
		/// Adds a control to the canvas.
		/// </summary>
		/// <param name="control">The control to add to the canvas.</param>
		/// <param name="pos">The drop postion relative to the canvas.</param>
		public void AddCanvasItem(CanvasControl control, Point pos)
		{
			AddCanvasItem(control);
			control.SetCanvas(Canvas);
			control.SetPosition(pos);
		}

 		public void CopyControlsTo(Page page)
		{
			using (var stream = new MemoryStream()) {
				using (var writer = XmlWriter.Create(stream)) {
					writer.WriteStartElement("root");
					this.WriteXmlCanvasControls(writer);
					writer.WriteEndElement();
				}

				stream.Position = 0;

				using (var reader = XmlReader.Create(stream)) {
					page.ReadXmlCanvasControls(reader);
				}
			}
		}

		public void Dispose()
		{
			_OnRemove?.Invoke();
		}

		public FrameworkElement GetPropertyLayout()
		{
			return LayoutProperties;
		}

		public FrameworkElement GetRootElement()
		{
			return Border;
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			Id = reader.GetAttribute("Id");

			string pageSize = reader.GetAttribute("PageSize");
			string orientation = null;
			if (pageSize == "Custom") {
				double width = 0, height = 0;
				if (!double.TryParse(reader.GetAttribute("CustomWidth"), out width)) {
					var msg = $"Unable to parse custom document width from XML for page ID {Id}";
					Logger.Log(Severity.ERROR, LogCategory.CONTROL, msg);
					throw new Exception(msg);
				}
				if (!double.TryParse(reader.GetAttribute("CustomHeight"), out height)) {
					var msg = $"Unable to parse custom document height from XML for page ID {Id}";
					Logger.Log(Severity.ERROR, LogCategory.CONTROL, msg);
					throw new Exception(msg);
				}

				PageWidth = width;
				PageHeight = height;
			}
			else {
				orientation = reader.GetAttribute("PageOrientation");
			}

			var bgColor = reader.GetAttribute("BackgroundColor");
			LayoutProperties.SetPageBackgroundColor(bgColor);

			ReadXmlCanvasControls(reader);

			LayoutProperties.CbSize.SelectedIndex = -1;
			foreach (var cbi in LayoutProperties.PageSizes) {
				string name = cbi.Content as string;
				if (name == pageSize) {
					LayoutProperties.CbSize.SelectedItem = cbi;
					break;
				}
			}

			if (orientation != null) {
				if (orientation == "Landscape") {
					LayoutProperties.SetSelectedPageOrientation(PagePropertiesPanel.PageOrientation.LANDSCAPE);
				}
				else {
					LayoutProperties.SetSelectedPageOrientation(PagePropertiesPanel.PageOrientation.PORTRAIT);
				}
			}

			LayoutProperties.ReadXml(reader);
		}

		public void ReadXmlCanvasControls(XmlReader reader)
		{
			reader.ReadStartElement();
			var CcCount = int.Parse(reader.GetAttribute("Count"));
			if (CcCount <= 0) {
				reader.Skip();
			}
			else {
				Type type = null;
				for (int i = 0; i < CcCount; i++) {
					if (!reader.IsEmptyElement) {
						reader.ReadStartElement();
					}

					type = Type.GetType(reader.Name);
					var instance = (CanvasControl)Activator.CreateInstance(type);
					instance.ReadXml(reader);
					AddCanvasItem(instance);

					if (reader.IsEmptyElement) {
						reader.Skip();
					}
					else {
						reader.ReadEndElement();
					}
				}
				reader.ReadEndElement();
			}
		}

		/// <summary>
		/// Resizes the page to the specified width and height
		/// while keeping controls in their relative positions.
		/// </summary>
		/// <param name="width">The width in Millimeters.</param>
		/// <param name="height">The height in Millimeters.</param>
		public bool ResizePage(double width, double height)
		{
			var newWidth = (width * INCHES_PER_MM) * ImageHelper.DPI;
			var newHeight = (height * INCHES_PER_MM) * ImageHelper.DPI;

			// determine if there's any controls outside of the new boundary
			foreach (UIElement child in Canvas.Children) {
				var offset = new Size(Canvas.GetLeft(child), Canvas.GetTop(child));
				if (offset.Width > newWidth || offset.Height > newHeight) {
					var result = MessageBox.Show("Some elements are outside of the requested page size boundaries." +
						" The elements outside of the new page boundaries will be removed. " +
						"Do you wish to proceed?", "", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
					if (result == MessageBoxResult.No) {
						return false;
					}
					break;
				}
			}

			// resize canvas
			PageWidth = newWidth;
			PageHeight = newHeight;

			// remove overflowing controls
			foreach (UIElement child in Canvas.Children) {
				var offset = new Size(Canvas.GetLeft(child), Canvas.GetTop(child));
				if (offset.Width > newWidth || offset.Height > newHeight) {
					Application.Current.Dispatcher.BeginInvoke(new Action<PageBase>((sender =>
					{
						Canvas.RemoveChild(child);
					})), DispatcherPriority.ApplicationIdle, new object[] { this });
				}
			}

			return true;
		}

		/// <summary>
		/// Scales this page to fit the specified viewport.
		/// </summary>
		/// <param name="viewport">The viewport.</param>
		public void ScaleToFit(FrameworkElement viewport)
		{
			var vpWidth = viewport.ActualWidth;
			var vpHeight = viewport.ActualHeight;
			double factor = 1;
			if (PageWidth < PageHeight) {
				factor = (viewport.ActualHeight - 20)/ Canvas.ActualHeight;
			}
			else {
				factor = (viewport.ActualWidth - 20)/ Canvas.ActualWidth;
			}

            if (this is ContainerEx c)
            {
                factor = 1;
            }

			LayoutTransform.ScaleX = factor;
			LayoutTransform.ScaleY = factor;
		}

		public virtual void SetId(int id)
		{
			Id = "Page " + id.ToString();
		}

		public NextPageCondition QueryNextPage()
		{
			if (!LayoutProperties.IsNextPageEnabled) {
				return NextPageCondition.FALSE;
			}

			if (LayoutProperties.IsNextPageVariable) {
				return NextPageCondition.UNKNOWN;
			}

			return LayoutProperties.GetNextPageSimple() != null 
				? NextPageCondition.TRUE 
				: NextPageCondition.FALSE;
		}

		/// <summary>
		/// Gets the next page if Simple is selected in the Next Page Tab of the Page Properties.
		/// </summary>
		/// <returns>Page Id, or null if not available</returns>
		public string GetNextPageSimple()
		{
			if (QueryNextPage() != NextPageCondition.TRUE) {
				return null;
			}

			return LayoutProperties.GetNextPageSimple();
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("Id", Id);
			var pageSize = (string)((ComboBoxItem)LayoutProperties.CbSize.SelectedItem).Content;
			writer.WriteAttributeString("PageSize", pageSize);
			if (pageSize == "Custom") {
				writer.WriteAttributeString("CustomWidth", PageWidth.ToString());
				writer.WriteAttributeString("CustomHeight", PageHeight.ToString());
			}
			else {
				writer.WriteAttributeString("PageOrientation",
					LayoutProperties.GetSelectedPageOrientation() == PagePropertiesPanel.PageOrientation.LANDSCAPE
					? "Landscape"
					: "Portrait"
				);
			}

			writer.WriteAttributeString("BackgroundColor", LayoutProperties.SelectedBgColor.Content as string);

			WriteXmlCanvasControls(writer);

			writer.WriteStartElement("Properties");
			LayoutProperties.WriteXml(writer);
			writer.WriteEndElement();
		}

		public void WriteXmlCanvasControls(XmlWriter writer)
		{
			writer.WriteStartElement("CanvasControls");
			writer.WriteAttributeString("Count", CanvasControls.Count.ToString());
			foreach (var control in CanvasControls) {
				writer.WriteStartElement(control.GetType().ToString());
				control.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			foreach (var control in CanvasControls) {
				if (control.isDraggingBorder) {
					control.StopDraggingBorder();
				}
			}

        }

		/// <summary>
		/// Gets the CanvasControl for target element.
		/// </summary>
		/// <param name="element">The target element.</param>
		/// <returns>A CanvasControl if one is found, else null.</returns>
		private CanvasControl GetControlForElement(UIElement element)
		{
			foreach (var c in CanvasControls) {
				if (c.Contains(element)) {
					return c;
				}
			}

			return null;
		}

        private void LayoutProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "SelectedBgColor") {
				Canvas.Background = PagePropertiesPanel.BgColorMap[LayoutProperties.SelectedBgColor.Content as string];
				foreach(CanvasRichTextBox control in CanvasControls.Where((control) => control.GetType() == typeof(CanvasRichTextBox))) {
					control.SetBackgroundColor(Canvas.Background);
				}
			}
		}
	}

	public class Pages : ObservableRangeCollection<PageBase>, IXmlSerializable
	{
		private PagesPropertiesPanel layoutProperties;

		public PagesPropertiesPanel LayoutProperties {
			get {
				if (layoutProperties == null) {
					MainWindow window = MainWindow.Instance;
					layoutProperties = new PagesPropertiesPanel();
					layoutProperties.PropertyChanged += LayoutProperties_PropertyChanged;
				}

				return layoutProperties;
			}
			set => layoutProperties = value;
		}

		public Pages() : base()
		{
			CollectionChanged += Pages_CollectionChanged;
		}

		private void UpdateChildPropertyStates(IList<PageBase> children)
		{
			var state = LayoutProperties.IsPageOrderVariable;
			foreach (var page in children) {
				page.LayoutProperties.TabNextPage.IsEnabled = state;
			}
		}

		private void Pages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					UpdateChildPropertyStates(e.NewItems.Cast<PageBase>().ToList());
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

		private void LayoutProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsPageOrderVariable") {
				UpdateChildPropertyStates(this);
			}
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement();
			var docCount = int.Parse(reader.GetAttribute("Count"));
			if (docCount <= 0) {
				reader.Skip(); // skip Document
			}
			else {
				reader.ReadStartElement();
				for (int i = 0; i < docCount; i++) {
					var type = Type.GetType(reader.Name);
					var instance = (PageBase)Activator.CreateInstance(type);
					instance.ReadXml(reader);
					Add(instance);
					reader.ReadEndElement();
				}

				reader.ReadEndElement(); // end of Document
			}

			LayoutProperties.ReadXml(reader);
			if (reader.IsEmptyElement) {
				reader.Skip();
			}
			else {
				reader.ReadEndElement();
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Document");
			writer.WriteAttributeString("Count", Count.ToString());
			foreach (var page in this) {
				writer.WriteStartElement(page.GetType().ToString());
				page.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();

			writer.WriteStartElement("Properties");
			LayoutProperties.WriteXml(writer);
			writer.WriteEndElement();
		}
	};
}