using Designer.Controls;
using JdSuite.Common.Logging;
using JdSuite.Common.Logging.Enums;
using JdSuite.Common.Module;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Designer.Variables
{

	public class Record : List<PageBase>
	{
		public int RecordIndex;
		public int TotalPages;
	}

	public class PreRecord : Record
	{
		public List<string> Whitelist = new List<string>();
	}

	public class Records<T> : List<T> where T : Record
	{
		public int MasterPageCounter;
		/*public int GlobalSheetCounter;*/
		public int GlobalPageCounter;
		public DateTime JobStart;
	}

	public class SystemVariable
	{
		public static readonly string CONTROL_CHAR = '\u0005'.ToString();
		[Flags]
		public enum EFlag : Int32
		{
			MASTER_PAGE_COUNTER = 1 << 0,
			OVERFLOW = 1 << 1,
			PAGE_COUNTER_RECORD = 1 << 2,
			TOTAL_PAGES_RECORD = 1 << 3,
			PAGE_NAME = 1 << 4,
			RECORD_INDEX = 1 << 5,
			/*GlobalSheetCounter	= 1 << 6,*/
			GLOBAL_PAGE_COUNTER = 1 << 7,
			JOB_START = 1 << 8,

			All = 1 << 32
		}
		public static EFlag INT_MASK = EFlag.MASTER_PAGE_COUNTER | EFlag.GLOBAL_PAGE_COUNTER | EFlag.PAGE_COUNTER_RECORD | EFlag.RECORD_INDEX | EFlag.TOTAL_PAGES_RECORD;
		public static EFlag STRING_MASK = EFlag.PAGE_NAME | EFlag.JOB_START;
		public static EFlag BOOL_MASK = EFlag.OVERFLOW;
		public string Name;
		public EFlag Flag;
		public static List<SystemVariable> LIST = new List<SystemVariable>()
		{
			new SystemVariable($"{CONTROL_CHAR}Master Page Counter",  EFlag.MASTER_PAGE_COUNTER),
			new SystemVariable($"{CONTROL_CHAR}Overflow",             EFlag.OVERFLOW),
			new SystemVariable($"{CONTROL_CHAR}Page Counter/Record",  EFlag.PAGE_COUNTER_RECORD),
			new SystemVariable($"{CONTROL_CHAR}Total Pages/Record",   EFlag.TOTAL_PAGES_RECORD),
			new SystemVariable($"{CONTROL_CHAR}Page Name",            EFlag.PAGE_NAME),
			new SystemVariable($"{CONTROL_CHAR}Record Index",         EFlag.RECORD_INDEX),
			new SystemVariable($"{CONTROL_CHAR}Global Page Counter",  EFlag.GLOBAL_PAGE_COUNTER),
			new SystemVariable($"{CONTROL_CHAR}Job Start",            EFlag.JOB_START)
		};
		private static List<EFlag> FILTER_BLACKLIST = new List<EFlag>
		{
			EFlag.JOB_START,
			EFlag.PAGE_NAME,
			EFlag.GLOBAL_PAGE_COUNTER,
			EFlag.MASTER_PAGE_COUNTER
		};
		public static List<SystemVariable> FILTERS;
		public static Dictionary<string, SystemVariable> MAP = new Dictionary<string, SystemVariable>();

		static SystemVariable()
		{
			foreach(var sVar in LIST) {
				MAP.Add(sVar.Name, sVar);
			}

			FILTERS = LIST.Where((sysVar) => !FILTER_BLACKLIST.Contains(sysVar.Flag)).ToList();
		}

		public SystemVariable(string name, EFlag flag)
		{
			this.Name = name;
			this.Flag = flag;
		}
	}

	internal class Processing
	{
		/// <summary>
		/// Filter pages to generate based on a variable in a record.
		/// </summary>
		/// <param name="dataset">The dataset.</param>
		/// <param name="record">The record.</param>
		/// <param name="selectedVar">The selected variable.</param>
		/// <returns>A whitelist of valid page Ids for the specified record.</returns>
		private static List<string> PreDataFilter(ObservableRangeCollection<VariableData> dataset, XElement record, string selectedVar)
		{
			var pageWhitelist = new List<string>();
			var targetElem = record.Descendants(selectedVar)?.First();
			if (targetElem == null) {
				return pageWhitelist;
			}

			foreach (var filter in dataset) {
				var selPage = filter.SelectedPage.Content as string;
				var selVal = filter.SelectedValue.Content as string;
				if (selVal == VariableData.VALUE_DEFAULT) {
					if (selPage == VariableData.PAGE_EMPTY) {
						pageWhitelist.Clear();
					}
					else {
						pageWhitelist.Add(selPage);
					}
				}
				else if (selVal == targetElem.Value) {
					if (selPage == VariableData.PAGE_EMPTY) {
						pageWhitelist.Clear();
					}
					else {
						pageWhitelist.Add(selPage);
					}
				}
				else {
					pageWhitelist.Clear();
				}
			}

			return pageWhitelist;
		}

		private static List<string> PreSystemFilter(ObservableRangeCollection<VariableData> dataset, int recordIndex, SystemVariable.EFlag flag)
		{
			var pageWhitelist = new List<string>();

			foreach (var filter in dataset) {
				var selPage = filter.SelectedPage.Content as string;
				var selVal = filter.SelectedValue.Content as string;
				if (selVal == VariableData.VALUE_DEFAULT) {
					if (selPage == VariableData.PAGE_EMPTY) {
						pageWhitelist.Clear();
						continue;
					}
					pageWhitelist.Add(selPage);
				}
				switch (flag) {
					case SystemVariable.EFlag.RECORD_INDEX:
						if (selVal == recordIndex.ToString()) {
							if (selPage != VariableData.PAGE_EMPTY) {
								pageWhitelist.Add(selPage);
							}
						}
						break;
				}
			}

			return pageWhitelist;
		}

		/// <summary>
		/// Processes the next page conditional selection.
		/// </summary>
		/// <param name="pages">The set of pages to reference.</param>
		/// <param name="dataset">The Next Page selectors for the current page</param>
		/// <returns>The computed next page, else null if none are found.</returns>
		private static PageBase ProcessNextPageConditional(List<PageBase> pages, ObservableRangeCollection<VariableData> dataset)
		{
			PageBase nextPage = null;

			foreach (var filter in dataset) {
				var selPage = filter.SelectedPage.Content as string;
				var selVal = filter.SelectedValue.Content as string;
				if (selPage == VariableData.PAGE_EMPTY) {
					nextPage = null;
					continue;
				}

				if (selVal == VariableData.VALUE_DEFAULT) {
					nextPage = pages.SingleOrDefault((p) => p.Id == selPage);
					continue;
				}


				var page = pages.SingleOrDefault((p) => p.Id == selPage);
				if (page == null) {
					continue;
				}

				foreach (CanvasVariable cv in page.CanvasControls.Where((c) => c.GetType().IsSubclassOf(typeof(CanvasVariable)))) {
					if (selVal == cv.Label.Content as string) {
						nextPage = page;
						break;
					}
				}
			}

			return nextPage;
		}

		/// <summary>
		/// Modifies a record after it has been generated to provide filter functionality based on System Variables.
		/// </summary>
		/// <param name="dataset">The dataset.</param>
		/// <param name="record">The record.</param>
		/// <param name="flag">The flag.</param>
		/// <returns></returns>
		private static Record PostSystemFilter(ObservableRangeCollection<VariableData> dataset, Record record, SystemVariable.EFlag flag)
		{
			var pageWhitelist = new List<string>();

			foreach (var filter in dataset) {
				var selPage = filter.SelectedPage.Content as string;
				var selVal = filter.SelectedValue.Content as string;
				if (selVal == VariableData.VALUE_DEFAULT) {
					if (selPage == VariableData.PAGE_EMPTY) {
						pageWhitelist.Clear();
					}
					else {
						pageWhitelist.Add(selPage);
					}
					continue;
				}

				switch (flag) {
					case SystemVariable.EFlag.PAGE_COUNTER_RECORD:
						int val = 0;
						if (!int.TryParse(selVal, out val)) {
							pageWhitelist.Clear();
						}
						if (record.ElementAtOrDefault(val) != null) {
							pageWhitelist.Add(selPage);
						}
						break;
					case SystemVariable.EFlag.TOTAL_PAGES_RECORD:
						if (selVal == record.Count.ToString()) {
							pageWhitelist.Add(selPage);
						}
						break;
				}
			}

			record.RemoveAll((page) => !pageWhitelist.Contains(page.Id));
			return record;
		}

		private static List<PageBase> GetNextPageChain(List<PageBase> pages, PageBase startPage)
		{
			var chain = new List<PageBase>();
			chain.Add(startPage);
			var currentPage = startPage;
			var nextPageCondition = currentPage.QueryNextPage();
			while (nextPageCondition != PageBase.NextPageCondition.FALSE) {
				PageBase nextPage = null;
				if (nextPageCondition == PageBase.NextPageCondition.UNKNOWN) {
					// NEXT PAGE VARIABLE
					nextPage = ProcessNextPageConditional(pages, currentPage.LayoutProperties.VariableDataGrid.GridDataset);
					if (nextPage == null) {
						break;
					}
				}
				else {
					// NEXT PAGE SIMPLE
					var pageName = currentPage.GetNextPageSimple();
					if (pageName == null) {
						break;
					}

					nextPage = pages.SingleOrDefault((p) => p.Id == pageName);
					if (nextPage == null) {
						Logger.Log(Severity.WARN, LogCategory.APP, $"Next page '{pageName}' not found for page '{currentPage.Id}'.");
						break;
					}
				}

				if (chain.Contains(nextPage)) {
					Logger.Log(Severity.WARN, LogCategory.APP, $"Cannot append next page {nextPage.Id} to '{currentPage.Id}': Circular reference detected.");
					break;
				}

				chain.Add(nextPage);
				currentPage = nextPage;
				nextPageCondition = currentPage.QueryNextPage();
			}

			return chain;
		}

		private static Page CompilePageFromSource(PageBase page, Record record, XElement element, int globalPageCounter, int pageCounterRecord, DateTime jobStart)
		{
			var pageCopy = new Page(page.LayoutProperties)
			{
				PageWidth = page.PageWidth,
				PageHeight = page.PageHeight,
				Id = page.Id
			};
			pageCopy.Canvas.Background = page.Canvas.Background;
			page.CopyControlsTo(pageCopy);

			foreach (CanvasVariable csd in (pageCopy.CanvasControls.Where((control) => { return control.GetType().IsSubclassOf(typeof(CanvasVariable)); }))) {
				var csdName = csd.GetSchemaElementName();

				if (SystemVariable.MAP.ContainsKey(csdName)) {
					switch (SystemVariable.MAP[csdName].Flag) {
						case SystemVariable.EFlag.PAGE_NAME:
							csd.Label.Content = pageCopy.Id;
							break;

						case SystemVariable.EFlag.RECORD_INDEX:
							csd.Label.Content = record.RecordIndex.ToString();
							break;

						case SystemVariable.EFlag.OVERFLOW:
							// TODO 
							csd.Label.Content = "null";
							break;

						case SystemVariable.EFlag.GLOBAL_PAGE_COUNTER:
							csd.Label.Content = globalPageCounter.ToString();
							break;

						case SystemVariable.EFlag.PAGE_COUNTER_RECORD:
							csd.Label.Content = pageCounterRecord.ToString();
							break;

						case SystemVariable.EFlag.JOB_START:
							csd.Label.Content = jobStart.ToString("G");
							break;
					}
				}
				else {
					var value = element.Descendants(csdName).First()?.Value;
					csd.Label.Content = value ?? "null";
				}
			}

			return pageCopy;
		}

		public static Records<Record> Run(Pages pages, Field schema, XDocument data, bool useFilters)
		{
			var preRecords = Preprocess(pages, schema, data, useFilters);
			return PostProcess(preRecords, pages.LayoutProperties, schema, data, useFilters);
		}

		public static Records<PreRecord> Preprocess(Pages pages, Field schema, XDocument data, bool useFilters)
		{
#if PERF_TEST
			var recordsClock = new Stopwatch();
			var recordClock = new Stopwatch();
			var recordItemClock = new Stopwatch();
			var preprocessClock = new Stopwatch();

			preprocessClock.Start();
#endif
			var PagesProperties = pages.LayoutProperties as PagesPropertiesPanel;
			var records = new Records<PreRecord>();
			records.JobStart = DateTime.Now;
			records.GlobalPageCounter = pages.Count;
			var filterGrid = PagesProperties.VariableDataGrid;
			var selVariable = ((ComboBoxItem)filterGrid.ComboBoxVariable.SelectedItem).Content as string;
			var pageNames = pages.Select((page) => page.Id).ToList();
			bool postFilter = SystemVariable.MAP.ContainsKey(selVariable)
				&& SystemVariable.MAP[selVariable].Flag != SystemVariable.EFlag.RECORD_INDEX;
			var elements = data.Root.Elements().ToArray();

			// Generate all available page templates for each record that match preprocess filters
			for (int i = 0; i < elements.Count(); i++) {
#if PERF_TEST
				recordClock.Restart();
#endif
				var elem = elements[i];
				var record = new PreRecord();

				if (SystemVariable.MAP.ContainsKey(selVariable) && SystemVariable.MAP[selVariable].Flag == SystemVariable.EFlag.RECORD_INDEX) {
					record.Whitelist.AddRange(PreSystemFilter(filterGrid.GridDataset, i, SystemVariable.MAP[selVariable].Flag));
				}
				else if (!postFilter && useFilters) {
					record.Whitelist.AddRange(PreDataFilter(filterGrid.GridDataset, elem, selVariable));
				}
				else {
					record.Whitelist.AddRange(pageNames);
				}

				record.RecordIndex = records.Count;

				var filteredPages = pages.Where((page) => record.Whitelist.Contains(page.Id));
				List<PageBase> sourceTemplates = null;
				if (pages.LayoutProperties.IsPageOrderVariable) {
					sourceTemplates = new List<PageBase>();
					foreach (var page in filteredPages.Where((p) => p.QueryNextPage() != PageBase.NextPageCondition.FALSE)) {
						sourceTemplates.AddRange(GetNextPageChain(pages.ToList(), page));
					}

					sourceTemplates = sourceTemplates.Distinct().ToList();
				}

				// If there's no next page chains, just add the filtered pages as a source
				if (sourceTemplates == null || sourceTemplates.Count <= 0) {
					sourceTemplates = filteredPages.ToList();
				}
				
				for (int x = 0; x < sourceTemplates.Count; x++) {
#if PERF_TEST
					recordItemClock.Restart();
#endif
					var page = sourceTemplates[x];
					record.Add(CompilePageFromSource(page, record, elem, pages.Count, x, records.JobStart));
#if PERF_TEST
					recordItemClock.Stop();
					Console.WriteLine("recordItem={0}", recordItemClock.Elapsed);
#endif
				}
				record.TotalPages = record.Count;
				records.Add(record);
#if PERF_TEST
				recordClock.Stop();
				Console.WriteLine("Record={0}", recordClock.Elapsed);
#endif
			}
#if PERF_TEST
			preprocessClock.Stop();
			Console.WriteLine("Records(totalPreprocess)={0}", preprocessClock.Elapsed);
#endif
			return records;
		}
		
		public static Records<Record> PostProcess(Records<PreRecord> preRecords, PagesPropertiesPanel pagesProperties, Field schema, XDocument data, bool useFilters)
		{
#if PERF_TEST
			var processClock = new Stopwatch();
			processClock.Start();
#endif
			var filterGrid = pagesProperties.VariableDataGrid;
			var selVariable = ((ComboBoxItem)filterGrid.ComboBoxVariable.SelectedItem).Content as string;
			var pageNames = preRecords.First().Select((page) => page.Id).ToList();
			bool postFilter = SystemVariable.MAP.ContainsKey(selVariable)
				&& SystemVariable.MAP[selVariable].Flag != SystemVariable.EFlag.RECORD_INDEX;

			// Post filter
			if (postFilter && useFilters) {
				foreach (var record in preRecords) {
					PostSystemFilter(filterGrid.GridDataset, record, SystemVariable.MAP[selVariable].Flag);
				}
			}

			// Build Next Page chains
			var records = new Records<Record>();
			foreach (var preRecord in preRecords) {
				var record = new Record();
				record.RecordIndex = preRecord.RecordIndex;
				foreach (var procPage in preRecord.Where((p) => preRecord.Whitelist.Contains(p.Id))) {
					record.AddRange(GetNextPageChain(preRecord.ToList(), procPage));
				}
				record.TotalPages = record.Count;
				records.Add(record);
			} 	

			records.MasterPageCounter = records.Sum((record) => record.Count);
			records.GlobalPageCounter = preRecords[0].Count;

			// Populate SystemVariables in the records
			foreach (var record in records) {
				foreach (var page in record) {
					foreach (CanvasVariable csd in (page.CanvasControls.Where((control) => { return control.GetType().IsSubclassOf(typeof(CanvasVariable)); }))) {
						var csdName = csd.GetSchemaElementName();
						if (SystemVariable.MAP.ContainsKey(csdName)) {
							switch (SystemVariable.MAP[csdName].Flag) {
								case SystemVariable.EFlag.MASTER_PAGE_COUNTER:
									csd.Label.Content = records.Count.ToString();
									break;

								case SystemVariable.EFlag.TOTAL_PAGES_RECORD:
									csd.Label.Content = record.TotalPages.ToString();
									break;
							}
						}
					}
				}
			}

	#if PERF_TEST
			processClock.Stop();
			Console.WriteLine("Process={0}", processClock.Elapsed);
	#endif

			foreach (var record in records) {
				foreach (var page in record) {
					page.Canvas.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
					page.Canvas.Arrange(new System.Windows.Rect(page.Canvas.DesiredSize));
					page.Canvas.UpdateLayout();
				}
			}
			return records;
		}
	}
}