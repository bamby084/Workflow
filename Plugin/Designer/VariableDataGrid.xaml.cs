using Designer.Variables;
using JdSuite.Common.Logging;
using JdSuite.Common.Logging.Enums;
using JdSuite.Common.Module;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Designer
{
	public class VariableData : INotifyPropertyChanged, IXmlSerializable
	{
		public static readonly string CONTROL_CHAR = '\u0005'.ToString();
		public static readonly string PAGE_EMPTY = CONTROL_CHAR + "Empty";
		public static readonly string VALUE_DEFAULT = CONTROL_CHAR + "Default";
		private static readonly string CONTROL_CHAR_XML = XmlConvert.EncodeName(CONTROL_CHAR);
		private bool NewPageSelected = false;
		private ObservableCollection<ComboBoxItem> pages = new ObservableCollection<ComboBoxItem>();
		private Pages PagesList;
		private ComboBoxItem selectedPage;
		private ComboBoxItem selectedValue;
		private ComboBoxItem Selector = new ComboBoxItem();
		private ObservableCollection<ComboBoxItem> values = new ObservableCollection<ComboBoxItem>();
		private string xmlRestoredPage = null;

		// Binding from code to UI
		public ObservableCollection<ComboBoxItem> Pages {
			get => pages;
			set {
				pages = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Pages"));
			}
		}

		// Binding from UI to code
		public ComboBoxItem SelectedPage {
			get => selectedPage; set {
				selectedPage = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedPage"));
			}
		}

		// Binding from UI to code
		public ComboBoxItem SelectedValue {
			get => selectedValue; set {
				selectedValue = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedValue"));
			}
		}

		// Binding from code to UI
		public ObservableCollection<ComboBoxItem> Values { get => values; set => this.values = value; }

		public event PropertyChangedEventHandler PropertyChanged;

		public VariableData(Pages pages) : base()
		{
			{
				var item = new ComboBoxItem();
				item.Content = VALUE_DEFAULT;
				Values.Add(item);
				SelectedValue = item;
			}
			Selector.Selected += Selector_Selected;
			Selector.Content = CONTROL_CHAR + "New Selector";
			Values.Add(Selector);

			this.PagesList = pages;
			RegeneratePageCombobBox();
			SelectedPage = Pages[0];
			PagesList.CollectionChanged += PagesList_CollectionChanged;
		}

		private VariableData() : this(MainWindow.Instance.DesignerControl.Pages)
		{
		}

		~VariableData()
		{
			if (PagesList != null) {
				PagesList.CollectionChanged -= PagesList_CollectionChanged;
			}
		}

		public ComboBoxItem AddValue(string newValue)
		{
			var cbi = new ComboBoxItem();
			cbi.Content = newValue;
			Values.Insert(1, cbi);
			return cbi;
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void PromptNewSelector()
		{
			var SelectorDialog = new PPP_NewSelector();
			SelectorDialog.ShowDialog();
			if (!SelectorDialog.Successful) {
				return;
			}

			SelectedValue = AddValue(SelectorDialog.Field.Text);
		}

		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			xmlRestoredPage = null;
			string selVal = null;
			var vals = new List<string>();

			while (reader.MoveToNextAttribute()) {
				var attr = reader.Name;
				var val = reader.Value;
				if (val.Contains(CONTROL_CHAR_XML)) {
					val = XmlConvert.DecodeName(val);
				}
				if (attr == "SelectedPage") {
					xmlRestoredPage = val;
				}
				else if (attr == "SelectedValue") {
					selVal = val;
				}
				else {
					vals.Add(val);
				}
			}
			reader.Skip();

			foreach (string val in vals) {
				var cbi = AddValue(val);
				if (selVal == val) {
					SelectedValue = cbi;
				}
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			var selPage = (string)SelectedPage.Content;
			if (selPage.Contains(CONTROL_CHAR)) {
				selPage = XmlConvert.EncodeName(selPage);
			}
			writer.WriteAttributeString("SelectedPage", selPage);
			var selVal = (string)SelectedValue.Content;
			if (selVal.Contains(CONTROL_CHAR)) {
				selVal = XmlConvert.EncodeName(selVal);
			}
			writer.WriteAttributeString("SelectedValue", selVal);
			for (int i = 0; i < Values.Count; i++) {
				var val = values[i].Content as string;
				if (val.Contains(CONTROL_CHAR)) {
					continue;
				}
				writer.WriteAttributeString($"Value_{i}", val);
			}
		}

		private ComboBoxItem AddPage(string newPage)
		{
			var cbi = new ComboBoxItem();
			cbi.Content = newPage;
			pages.Add(cbi);
			return cbi;
		}

		private ComboBoxItem AddPage(PageBase page)
		{
			var cbi = AddPage(page.Id);
			Binding binding = new Binding("Id");
			binding.Source = page;
			binding.Mode = BindingMode.OneWay;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			cbi.SetBinding(ComboBoxItem.ContentProperty, binding);
			return cbi;
		}

		private void NewPage_Selected(object sender, RoutedEventArgs e)
		{
			NewPageSelected = true;
			Application.Current.Dispatcher.BeginInvoke(
				new Action<VariableData>((c) => {
					MainWindow.Instance.DesignerControl.AddNewPage();
				}),
				this
			);
		}

		private void PagesList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			var prevSelectedPage = SelectedPage.Content as string;
			RegeneratePageCombobBox();

			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) {
				bool wasEmpty = PagesList.Count - e.NewItems.Count == 0;

				if (xmlRestoredPage != null && e.NewItems.Count > 0) {
					foreach (var page in Pages) {
						if (page.Content as string == xmlRestoredPage) {
							SelectedPage = page;
							xmlRestoredPage = null;
							return;
						}
					}
				}

				// Default behavior is to select "Empty"
				if (wasEmpty || e.NewItems.Count <= 0) {
					SelectedPage = Pages[0];
				}
				else if (NewPageSelected) {
					foreach (var page in Pages) {
						if (page.Content as string == ((PageBase)e.NewItems[0]).Id) {
							SelectedPage = page;
							break;
						}
					}
				}
				else {
					foreach (var page in Pages) {
						if (page.Content as string == prevSelectedPage) {
							SelectedPage = page;
							break;
						}
					}
				}
			}
			else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove) {
				bool wasOldSelected = false;
				foreach (PageBase old in e.OldItems) {
					if (prevSelectedPage == old.Id) {
						SelectedPage = Pages.Count > 2 ? Pages[1] : Pages[0];
						wasOldSelected = true;
						break;
					}
				}

				if (!wasOldSelected) {
					foreach (var cbi in Pages) {
						if (cbi.Content as string == prevSelectedPage) {
							SelectedPage = cbi;
						}
					}
				}
			}
		}

		private void RegeneratePageCombobBox()
		{
			Pages.Clear();

			AddPage(PAGE_EMPTY);

			foreach (PageBase item in PagesList) {
				AddPage(item);
			}

			AddPage(CONTROL_CHAR + "Create New Page").Selected += NewPage_Selected;
		}

		private void Selector_Selected(object sender, RoutedEventArgs e)
		{
			SelectedPage = Pages[0];
			Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
			{
				PromptNewSelector();
			});
		}
	}

	/// <summary>
	/// Interaction logic for VariableDataGrid.xaml
	/// </summary>
	public partial class VariableDataGrid : UserControl, IXmlSerializable
	{
		public enum VariableType
		{
			INTEGER,
			STRING,
			BOOLEAN
		}

		public Dictionary<string, List<VariableData>> VariableDataSources = new Dictionary<string, List<VariableData>>();
		private static List<ComboBoxItem> SystemVariables = new List<ComboBoxItem>();
		private XDocument Data;
		private ObservableRangeCollection<VariableData> gridDataset = new ObservableRangeCollection<VariableData>();
		private Pages Pages;
		private Field Schema;
		private ObservableRangeCollection<ComboBoxItem> variableItems = new ObservableRangeCollection<ComboBoxItem>();

		public ObservableRangeCollection<VariableData> GridDataset { get => gridDataset; set => gridDataset = value; }

		public ObservableRangeCollection<ComboBoxItem> VariableItems { get => variableItems; set => variableItems = value; }

		static VariableDataGrid()
		{
			foreach (var item in SystemVariable.FILTERS) {
				var cbi = new ComboBoxItem();
				cbi.Content = item.Name;
				SystemVariables.Add(cbi);
			}
		}

		public VariableDataGrid()
		{
			var mainWindow = MainWindow.Instance;
			Pages = mainWindow.DesignerControl.Pages;
			Schema = mainWindow.Schema;
			Data = mainWindow.SchemaData;

			InitializeComponent();
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void LoadVariables(Field schema, VariableType variableType)
		{
			ComboBoxVariable.SelectedIndex = -1;
			VariableItems.Clear();

			// LOAD RELEVANT SYSTEM VARIABLES
			ComboBoxItem[] assocSysVars = null;
			switch (variableType) {
				case VariableType.BOOLEAN:
					assocSysVars = SystemVariables.Where((cbi) =>
					{
						var flag = SystemVariable.MAP[cbi.Content as string].Flag;
						return (flag & SystemVariable.BOOL_MASK) > 0;
					}).ToArray();
					break;

				case VariableType.INTEGER:
					assocSysVars = SystemVariables.Where((cbi) =>
					{
						var flag = SystemVariable.MAP[cbi.Content as string].Flag;
						return (flag & SystemVariable.INT_MASK) > 0;
					}).ToArray();
					break;

				case VariableType.STRING:
					assocSysVars = SystemVariables.Where((cbi) =>
					{
						var flag = SystemVariable.MAP[cbi.Content as string].Flag;
						return (flag & SystemVariable.STRING_MASK) > 0;
					}).ToArray();
					break;
			}
			foreach (var var in assocSysVars) {
				VariableItems.Add(var);
			}

			// LOAD DATA VARIABLES
			//foreach (Fields element in Schema.GetAllDecendent()) {
				string name = Schema.Name;
				string type = Schema.Type;

				string matchStr = null;
				switch (variableType) {
					case VariableType.INTEGER:
						matchStr = "int";
						break;

					case VariableType.STRING:
						matchStr = "string";
						break;

					case VariableType.BOOLEAN:
						matchStr = "boolean";
						break;
				}

				//if (name == null || type == null || !type.ToLower().Contains(matchStr)) { continue; }

				var item = new ComboBoxItem();
				item.Content = name;
				VariableItems.Add(item);
			//}

			ComboBoxVariable.SelectedIndex = 0;
		}

		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			string selVar = XmlConvert.DecodeName(reader.GetAttribute("SelectedVariable"));

			// START VARIABLE_TABLES
			reader.ReadStartElement();
			reader.MoveToContent();
			int count = 0;
			if (!int.TryParse(reader.GetAttribute("Count"), out count)) {
				Logger.Log(Severity.ERROR, LogCategory.CONTROL,
					"PagesPropertiesPanel deserialization: Parsing VariableTables Count failed. " +
					"Corruption detected. Abandoning attempt.");
				return;
			}

			VariableDataSources.Clear();
			var serializer = new XmlSerializer(typeof(List<VariableData>));
			// START ARRAY_OF_VARIABLE_DATA
			reader.ReadStartElement();
			for (int i = 0; i < count; i++) {
				reader.MoveToContent();
				var tableName = XmlConvert.DecodeName(reader.Name);
				reader.ReadStartElement();
				var list = serializer.Deserialize(reader) as List<VariableData>;
				VariableDataSources.Add(tableName, list);
				reader.ReadEndElement();
			}
			// END ARRAY_OF_VARIABLE_DATA
			reader.ReadEndElement();
			// END VARIABLE_TABLES

			ComboBoxVariable.SelectedIndex = -1;
			for (int i = 0; i < ComboBoxVariable.Items.Count; i++) {
				var item = ComboBoxVariable.Items[i] as ComboBoxItem;
				if ((string)item.Content == selVar) {
					ComboBoxVariable.SelectedIndex = i;
					break;
				}
			}

			if (ComboBoxVariable.SelectedIndex < 0) {
				ComboBoxVariable.SelectedIndex = 0;
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			var sv = (string)((ComboBoxItem)ComboBoxVariable.SelectedValue).Content;
			writer.WriteAttributeString("SelectedVariable", XmlConvert.EncodeName(sv));

			writer.WriteStartElement("VariableTables");
			writer.WriteAttributeString("Count", VariableDataSources.Count.ToString());
			var serializer = new XmlSerializer(typeof(List<VariableData>));
			foreach (var pair in VariableDataSources) {
				writer.WriteStartElement(XmlConvert.EncodeName(pair.Key));
				serializer.Serialize(writer, pair.Value);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		private void ButtonAdd_Click(object sender, RoutedEventArgs e)
		{
			VariableData newVD = new VariableData(Pages);
			newVD.PromptNewSelector();

			VariableDataSources[(string)((ComboBoxItem)ComboBoxVariable.SelectedItem).Content].Add(newVD);
			gridDataset.Add(newVD);
		}

		private void ButtonRemove_Click(object sender, RoutedEventArgs e)
		{
			if (gridDataset.Count <= 0) {
				return;
			}

			if (DataGridVariable.SelectedItem is VariableData data) {
				gridDataset.Remove(data);
			}
			else {
				gridDataset.Remove(gridDataset[gridDataset.Count - 1]);
			}
		}

		private void ComboBoxVariable_SelectionChanged(object sender, RoutedEventArgs e)
		{
			ComboBoxItem item = ComboBoxVariable.SelectedItem as ComboBoxItem;
			if (item == null) {
				return;
			}

			// Change DataGrid source depending on selected variable
			string variableName = (string)item.Content;
			if (!VariableDataSources.ContainsKey(variableName)) {
				var list = new List<VariableData>();

				// Add the first row data
				list.Add(new VariableData(Pages));

				VariableDataSources.Add(variableName, list);
			}

			gridDataset.Clear();
			foreach (var data in VariableDataSources[variableName]) {
				gridDataset.Add(data);
			}
		}
	}
}