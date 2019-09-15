using Designer.Tools;
using Designer.Variables;
using JdSuite.Common.Logging;
using JdSuite.Common.Logging.Enums;
using JdSuite.Common.Module;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Designer
{
	/// <summary>
	/// Interaction logic for PagePropertiesPanel.xaml
	/// </summary>
	public partial class PagePropertiesPanel : UserControl, IXmlSerializable, INotifyPropertyChanged
	{
		public enum PageOrientation
		{
			LANDSCAPE,
			PORTRAIT
		}

		public static Dictionary<string, SolidColorBrush> BgColorMap = new Dictionary<string, SolidColorBrush>();
		public static Dictionary<string, Size> StockPageSizes = new Dictionary<string, Size>();
		private static readonly SolidColorBrush DefaultButtonColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
		private static readonly SolidColorBrush StickyButtonColor = Brushes.LightGray;
		private List<ComboBoxItem> _BgColors = new List<ComboBoxItem>();
		private ObservableRangeCollection<ComboBoxItem> _CbPageSimpleList = new ObservableRangeCollection<ComboBoxItem>();
		private ComboBoxItem _CbPageSimpleSelectedPage;
		private PageBase _Page;
		private string _PageName = "";
		private ComboBoxItem _SelectedBgColor;
		private List<ComboBoxItem> pageSizes = new List<ComboBoxItem>();
		private Field Schema;
		private Pages Pages;
		private bool _IsNextPageVariable = false;
		private string XmlSimpleSelectedPage = null;

		public List<ComboBoxItem> BgColors { get => _BgColors; set => _BgColors = value; }

		public ObservableRangeCollection<ComboBoxItem> CbPageSimpleList {
			get => _CbPageSimpleList;
			set {
				_CbPageSimpleList = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CbPageSimpleList"));
			}
		}

		public decimal CustomHeight {
			get {
                if (Page is ContainerEx c)
                {
                    return (decimal)(Page.PageHeight / (PageBase.INCHES_PER_MM * ImageHelper.DPI));
                }
                return (decimal)(Page.PageHeight / ImageHelper.DPI);
			}
			set {
                if (Page is ContainerEx c)
                {
                    Page.PageHeight = decimal.ToDouble(value) * PageBase.INCHES_PER_MM * ImageHelper.DPI;
                }
                else
                    Page.PageHeight = decimal.ToDouble(value) * ImageHelper.DPI;
				PropertyChanged(this, new PropertyChangedEventArgs("CustomHeight"));
			}
		}

		public decimal CustomWidth {
			get {
                if (Page is ContainerEx c)
                {
                    return (decimal)(Page.PageWidth / (PageBase.INCHES_PER_MM * ImageHelper.DPI));
                }

				return (decimal)(Page.PageWidth / ImageHelper.DPI);
			}
			set {
                if (Page is ContainerEx c)
                {
                    Page.PageWidth = decimal.ToDouble(value) * PageBase.INCHES_PER_MM * ImageHelper.DPI;
                }
                else
                    Page.PageWidth = decimal.ToDouble(value) * ImageHelper.DPI;

				PropertyChanged(this, new PropertyChangedEventArgs("CustomWidth"));
			}
		}

		public PageBase Page { get => _Page; set => _Page = value; }

		public string PageName {
			get {
				return _PageName;
			}
			set {
				_PageName = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PageName"));
			}
		}

		public List<ComboBoxItem> PageSizes {
			get => pageSizes;
			set {
				pageSizes = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PageSizes"));
			}
		}

		public ComboBoxItem SelectedBgColor {
			get => _SelectedBgColor;
			set {
				_SelectedBgColor = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedBgColor"));
			}
		}

		public ComboBoxItem CbPageSimpleSelectedPage { get => _CbPageSimpleSelectedPage;
			set {
				_CbPageSimpleSelectedPage = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CbPageSimpleSelectedPage"));
			}
		}

		public bool IsNextPageVariable { get => _IsNextPageVariable; private set => _IsNextPageVariable = value; }
		public bool IsNextPageEnabled { get => TabNextPage.IsEnabled; }

		public event PropertyChangedEventHandler PropertyChanged;

		static PagePropertiesPanel()
		{
			foreach (var prop in typeof(Brushes).GetProperties()) {
				if (prop.PropertyType == typeof(SolidColorBrush)) {
					BgColorMap.Add(prop.Name, prop.GetMethod.Invoke(null, null) as SolidColorBrush);
				}
			}

			StockPageSizes.Add("Letter", new Size(215.9, 279.4));
			StockPageSizes.Add("Legal", new Size(215.9, 355.6));
			StockPageSizes.Add("Ledger", new Size(279.4, 431.8));
			StockPageSizes.Add("Tabloid", new Size(431.8, 279.4));
			StockPageSizes.Add("Executive", new Size(184.1, 266.7));
			StockPageSizes.Add("ANSI C", new Size(559, 432));
			StockPageSizes.Add("ANSI D", new Size(864, 559));
			StockPageSizes.Add("ANSI E", new Size(1118, 864));
		}

		public PagePropertiesPanel(PageBase page)
		{
			Schema = MainWindow.Instance.Schema;
			Page = page;
			Page.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "Id") {
					PageName = $"{Page.Id} - Page";
				}
				else if (e.PropertyName == "PageWidth") {
					PropertyChanged(this, new PropertyChangedEventArgs("CustomWidth"));
				}
				else if (e.PropertyName == "PageHeight") {
					PropertyChanged(this, new PropertyChangedEventArgs("CustomHeight"));
				}
			};

			{
				var cbi = new ComboBoxItem();
				cbi.Content = "Custom";
				PageSizes.Add(cbi);
			}
			foreach (var entry in StockPageSizes) {
				var cbi = new ComboBoxItem();
				cbi.Content = entry.Key;
				PageSizes.Add(cbi);
			}

			InitializeComponent();

			CbSize.SelectedIndex = 1;
			CbSize.SelectionChanged += CbSize_SelectionChanged;
			CbTypeSelector.SelectedIndex = 0;

			Pages = MainWindow.Instance.DesignerControl.Pages;
			Pages.CollectionChanged += Pages_CollectionChanged;
			Pages_CollectionChanged(null, null);
			CbPageSimpleSelectedPage = CbPageSimpleList[0]; 

			PageName = $"{page.Id} - Page";
			{
				Binding binding = new Binding("PageName");
				binding.Source = this;
				binding.Mode = BindingMode.OneWay;
				binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				LabelPage.SetBinding(Label.ContentProperty, binding);
			}

			foreach (var entry in PagePropertiesPanel.BgColorMap) {
				var cbi = new ComboBoxItem();
				cbi.Content = entry.Key;
				BgColors.Add(cbi);
				if (entry.Key == "White") {
					SelectedBgColor = cbi;
				}
			}
		}

		private void Pages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			var prevSelectedContent = XmlSimpleSelectedPage != null ? XmlSimpleSelectedPage : CbPageSimpleSelectedPage?.Content as string;

			CbPageSimpleList.Clear();
			CbPageSimpleSelectedPage = null;
			CbPageSimpleList.Add(new ComboBoxItem() { Content = $"{SystemVariable.CONTROL_CHAR}Empty" });
			foreach(var page in Pages) {
				if (page.Id != Page.Id) {
					CbPageSimpleList.Add(new ComboBoxItem() { Content = page.Id });
				}
			}

			if (prevSelectedContent != null) {
				foreach(var cbi in CbPageSimpleList) {
					if (prevSelectedContent == cbi.Content as string) {
						CbPageSimpleSelectedPage = cbi;
						if (XmlSimpleSelectedPage != null) {
							XmlSimpleSelectedPage = null;
						}
						break;
					}
				}
			}
			else {
				CbPageSimpleSelectedPage = CbPageSimpleList[0];
			}
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public PageOrientation GetSelectedPageOrientation()
		{
			if (ButtonLandscape.Background == StickyButtonColor) {
				return PageOrientation.LANDSCAPE;
			}
			else {
				return PageOrientation.PORTRAIT;
			}
		}

		/// <summary>
		/// If Next Page Simple is selected, this gets the next page, if any.
		/// </summary>
		/// <returns>The next page Id, or null if there is no next page.</returns>
		public string GetNextPageSimple()
		{
			if (IsNextPageVariable || !TabNextPage.IsEnabled) {
				return null;
			}

			var pageName = ((ComboBoxItem)CbPageSimple.SelectedItem).Content as string;
			if (pageName == VariableData.PAGE_EMPTY) {
				return null;
			}
			return pageName;
		}

		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			var sts = reader.GetAttribute("SelectedType");
			string sp = null;
			if (!sts.Contains("Simple")) {
				reader.ReadStartElement();
				VariableDataGrid.ReadXml(reader);
				reader.ReadEndElement();
			}
			else {
				sp = XmlConvert.DecodeName(reader.GetAttribute("SelectedPage"));
			}

			reader.ReadStartElement();
			DataGridSheetName.ReadXml(reader);
			reader.ReadEndElement();

			for (int i = 0; i < CbTypeSelector.Items.Count; i++) {
				var item = CbTypeSelector.Items[i] as ComboBoxItem;
				if ((string)item.Content == sts) {
					CbTypeSelector.SelectedIndex = i;
					break;
				}
			}

			if (sp == null) {
				return;
			}

			XmlSimpleSelectedPage = sp;
			foreach (var item in CbPageSimpleList) {
				if ((string)item.Content == sp) {
					CbPageSimpleSelectedPage = item;
					break;
				}
			}
		}

		public void SetPageBackgroundColor(string color)
		{
			foreach (var cbi in BgColors) {
				if (cbi.Content as string == color) {
					SelectedBgColor = cbi;
					return;
				}
			}

			Logger.Log(Severity.WARN, LogCategory.APP, $"Unknown page background color requested: {color}");
		}

		public void WriteXml(XmlWriter writer)
		{
			var sts = (string)((ComboBoxItem)CbTypeSelector.SelectedValue).Content;
			writer.WriteAttributeString("SelectedType", sts);

			if (!sts.Contains("Simple")) {
				writer.WriteStartElement("VariableDataGrid");
				VariableDataGrid.WriteXml(writer);
				writer.WriteEndElement();
			}
			else {
				writer.WriteAttributeString("SelectedPage", XmlConvert.EncodeName((string)CbPageSimpleSelectedPage.Content));
			}

			writer.WriteStartElement("SheetNames");
			DataGridSheetName.WriteXml(writer);
			writer.WriteEndElement();
		}

		internal void SetSelectedPageOrientation(PageOrientation orientation)
		{
			var prevOrientation = GetSelectedPageOrientation();
			if (orientation == PageOrientation.LANDSCAPE) {
				ButtonLandscape.Background = StickyButtonColor;
				ButtonPortrait.Background = DefaultButtonColor;
			}
			else {
				ButtonPortrait.Background = StickyButtonColor;
				ButtonLandscape.Background = DefaultButtonColor;
			}

			if (prevOrientation == orientation) {
				return;
			}

			var result = Page.ResizePage((Page.PageHeight / ImageHelper.DPI) * PageBase.MM_PER_INCH, (Page.PageWidth / ImageHelper.DPI) * PageBase.MM_PER_INCH);
			if (!result) {
				SetSelectedPageOrientation(prevOrientation);
			}
		}

		private void ButtonLandscape_Click(object sender, RoutedEventArgs e)
		{
			SetSelectedPageOrientation(PageOrientation.LANDSCAPE);
		}

		private void ButtonPortrait_Click(object sender, RoutedEventArgs e)
		{
			SetSelectedPageOrientation(PageOrientation.PORTRAIT);
		}

		private void CbSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (CbSize.SelectedIndex < 0) {
				return;
			}

			var pageSize = ((ComboBoxItem)CbSize.SelectedItem).Content as string;
			if (pageSize == "Custom") {
				LabelPageWidth.Visibility = Visibility.Visible;
				LabelPageHeight.Visibility = Visibility.Visible;
				LabelPageWidthM.Visibility = Visibility.Visible;
				LabelPageHeightM.Visibility = Visibility.Visible;
				UpDownPageWidth.Visibility = Visibility.Visible;
				UpDownPageHeight.Visibility = Visibility.Visible;
				ButtonLandscape.Visibility = Visibility.Collapsed;
				ButtonPortrait.Visibility = Visibility.Collapsed;
				return;
			}

			LabelPageWidth.Visibility = Visibility.Collapsed;
			LabelPageHeight.Visibility = Visibility.Collapsed;
			LabelPageWidthM.Visibility = Visibility.Collapsed;
			LabelPageHeightM.Visibility = Visibility.Collapsed;
			UpDownPageWidth.Visibility = Visibility.Collapsed;
			UpDownPageHeight.Visibility = Visibility.Collapsed;
			ButtonLandscape.Visibility = Visibility.Visible;
			ButtonPortrait.Visibility = Visibility.Visible;

			var newSize = StockPageSizes[pageSize];

			bool result = true;
			if (GetSelectedPageOrientation() == PageOrientation.LANDSCAPE) {
				result = Page.ResizePage(newSize.Height, newSize.Width);
			}
			else {
				result = Page.ResizePage(newSize.Width, newSize.Height);
			}

			if (!result) {
				CbSize.SelectedItem = e.RemovedItems[0];
			}
		}

		private void CbTypeCondition_Selected(object sender, RoutedEventArgs e)
		{
			SetDataGridVisibility(true);
			VariableDataGrid.LoadVariables(Schema, VariableDataGrid.VariableType.BOOLEAN);
		}

		private void CbTypeInteger_Selected(object sender, RoutedEventArgs e)
		{
			SetDataGridVisibility(true);
			VariableDataGrid.LoadVariables(Schema, VariableDataGrid.VariableType.INTEGER);
		}

		private void CbTypeSimple_Selected(object sender, RoutedEventArgs e)
		{
			SetDataGridVisibility(false);
		}

		private void CbTypeText_Selected(object sender, RoutedEventArgs e)
		{
			SetDataGridVisibility(true);
			VariableDataGrid.LoadVariables(Schema, VariableDataGrid.VariableType.STRING);
		}

		private void SetDataGridVisibility(bool visible)
		{
			IsNextPageVariable = visible;
			if (visible) {
				CbPageSimple.Visibility = Visibility.Collapsed;
				LabelPageSimple.Visibility = Visibility.Collapsed;
				VariableDataGrid.Visibility = Visibility.Visible;
			}
			else {
				CbPageSimple.Visibility = Visibility.Visible;
				LabelPageSimple.Visibility = Visibility.Visible;
				VariableDataGrid.Visibility = Visibility.Collapsed;
			}
		}
	}
}