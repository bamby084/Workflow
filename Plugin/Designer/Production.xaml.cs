using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.Simple;
using Designer.Controls;
using Designer.Variables;
using iText.Kernel.Events;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using JdSuite.Common.Logging;
using JdSuite.Common.Logging.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Designer
{
	/// <summary>
	/// Interaction logic for Production.xaml
	/// </summary>
	public partial class Production : UserControl
	{
		private FileSystemWatcher fileSystemWatcher;
		private ObservableCollection<LogEntry> LogEntries = new ObservableCollection<LogEntry>();
		private string OutputFilePath = null;
		private string SelectedOutputEngine = null;
		private DateTime StartDate = DateTime.Now;
		private Pages DocTemplate;

		public Production()
		{
			InitializeComponent();
			LogView.ItemsSource = LogEntries;
			LogEntries.Add(new LogEntry("Test", Severity.ERROR, LogCategory.APP, "Test description"));

            // Watch log file for changes
            fileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(Logger.LogPath))
            {
                NotifyFilter = NotifyFilters.LastAccess
                | NotifyFilters.LastWrite | NotifyFilters.FileName
                | NotifyFilters.DirectoryName | NotifyFilters.Size,
                Filter = Path.GetFileName(Logger.LogPath)
            };
            fileSystemWatcher.Changed += delegate
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RefreshLogEntries();
                });
            };
            fileSystemWatcher.EnableRaisingEvents = true;

            RefreshLogEntries();
        }

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == VisibilityProperty) {
				if ((Visibility)e.NewValue == Visibility.Visible) {
					var window = MainWindow.Instance;
					CurWorkflowComboBoxItem.Content = window.Workflow.Title;
					var pages = window.DesignerControl.Pages;
					bool shouldFilter = ((PagesPropertiesPanel)pages.LayoutProperties).IsPageOrderVariable;
					var records = Processing.Run(pages, window.Schema, window.SchemaData, shouldFilter);
					DocTemplate = new Pages();
					foreach (var record in records) {
						DocTemplate.AddRange(record);
					}
				}
			}
		}

		private void Engine_Selected(object sender, RoutedEventArgs e)
		{
			SelectedOutputEngine = (string)((ComboBoxItem)ComboBoxEngine.SelectedItem).Content;
		}

		private void RefreshLogEntries()
		{
			LogEntries.Clear();

			using (var mutex = new Mutex(false, Logger.FILE_MUTEX)) {
				mutex.WaitOne();
				using (StreamReader reader = new StreamReader(Logger.LogPath)) {
					while (!reader.EndOfStream) {
						if (LogEntry.TryParse(reader.ReadLine(), out LogEntry entry)) {
							if (entry.Date.CompareTo(StartDate) >= 0) {
								LogEntries.Add(entry);
							}
						}
					}
				}
				mutex.ReleaseMutex();
			}

			LogView.Items.Refresh();
		}

		private void Start_Click(object sender, RoutedEventArgs e)
		{
			if (OutputFilePath == null || OutputFilePath.Length <= 0) {
				MessageBox.Show("Please provide a file path before attempting to generate output.", "", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			Logger.Log(Severity.INFO, LogCategory.APP, "Job started (engine PDF)");
			var stopWatch = Stopwatch.StartNew();
			Common.Logging.LogManager.Adapter = new ITextLoggingShimAdapter();
			try {
				using (PdfWriter writer = new PdfWriter(OutputFilePath)) {
					PdfDocument pdf = new PdfDocument(writer);
					pdf.AddEventHandler(PdfDocumentEvent.START_PAGE, new PageStartEventHandler(DocTemplate));
					Document document = null;
					foreach (var page in DocTemplate) {
						var pageSize = new iText.Kernel.Geom.PageSize(
							page.PageWidth.ToITextPoint(), 
							page.PageHeight.ToITextPoint()
						);

						if (document == null) {
							document = new Document(pdf, pageSize);
							document.SetMargins(0, 0, 0, 0);
						}
						else {
							document.Add(new iText.Layout.Element.AreaBreak(pageSize));
						}


						foreach (var canvasCtrl in page.CanvasControls) {
							var elements = new List<iText.Layout.Element.IElement>();
							elements = canvasCtrl.ToIText();
							foreach (var element in elements) {
								if (element is iText.Layout.Element.Paragraph paragraph) {
									document.Add(paragraph);
								}
								else if (element is iText.Layout.Element.Image image) {
									document.Add(image);
								}
							}
						}

					}
					document.Close();
					pdf.Close();
				}
			}
			catch (Exception err) {
				Logger.Log(Severity.ERROR, LogCategory.APP, err.Message);
			}
			stopWatch.Stop();
			Logger.Log(Severity.INFO, LogCategory.APP, String.Format("Job finished (duration {0})", stopWatch.Elapsed.ToString("hh':'mm':'ss':'fff")));
		}

		private void ButtonOutBrowse_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

			// Set filter for file extension and default file extension
			dlg.DefaultExt = "." + SelectedOutputEngine.ToLower();
			dlg.Filter = $"{SelectedOutputEngine.ToUpper()} Files (*.{SelectedOutputEngine.ToLower()})|*.{SelectedOutputEngine.ToLower()}";

			// Display SaveFileDialog by calling ShowDialog method
			bool? result = dlg.ShowDialog();

			if (result == true) {
				OutputFilePath = dlg.FileName;
				TextBoxFileName.Text = OutputFilePath;
			}
		}
	}

	internal class PageStartEventHandler : IEventHandler
	{
		private readonly Pages Pages;
		public PageStartEventHandler(Pages pages)
		{
			Pages = pages;
		}

		public void HandleEvent(Event @event)
		{
			PdfDocumentEvent docEvent = (PdfDocumentEvent) @event;
			PdfPage page = docEvent.GetPage();
			int pageNumber = docEvent.GetDocument().GetNumberOfPages();
			var bgColor = ((SolidColorBrush)Pages[pageNumber - 1].Canvas.Background).Color;
			var pageColor = new iText.Kernel.Colors.DeviceRgb(
				Convert.ToInt32(bgColor.R),
				Convert.ToInt32(bgColor.G),
				Convert.ToInt32(bgColor.B)
			);
	 
			PdfCanvas canvas = new PdfCanvas(page);
			iText.Kernel.Geom.Rectangle rect = page.GetPageSize();
			canvas.SaveState()
				  .SetFillColor(pageColor)
				  .Rectangle(rect.GetLeft(), rect.GetBottom(), rect.GetWidth(), rect.GetHeight())
			      .FillStroke()
			      .RestoreState();
		}
	}

	internal class ITextLoggingShimAdapter : AbstractSimpleLoggerFactoryAdapter
	{
		public ITextLoggingShimAdapter() : base(LogLevel.All, true, true, true, "hh':'mm':'ss':'fff")
		{
		}

		protected ITextLoggingShimAdapter(NameValueCollection properties) : base(properties)
		{
		}

		protected ITextLoggingShimAdapter(LogLevel level, bool showDateTime, bool showLogName, bool showLevel, string dateTimeFormat) : base(level, showDateTime, showLogName, showLevel, dateTimeFormat)
		{
		}

		protected override ILog CreateLogger(string name, LogLevel level, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
		{
			return new ITextLoggingShim(name, level, showLevel, showDateTime, showLogName, dateTimeFormat);
		}

		public class ITextLoggingShim : AbstractSimpleLogger
		{
			public ITextLoggingShim(string logName, LogLevel logLevel, bool showlevel, bool showDateTime, bool showLogName, string dateTimeFormat) : base(logName, logLevel, showlevel, showDateTime, showLogName, dateTimeFormat)
			{
			}

			protected override void WriteInternal(LogLevel level, object message, Exception exception)
			{
				Severity severity = Severity.INFO;
				switch (level) {
					case LogLevel.All:
						severity = Severity.INFO;
						break;

					case LogLevel.Trace:
						severity = Severity.DEBUG;
						break;

					case LogLevel.Debug:
						severity = Severity.DEBUG;
						break;

					case LogLevel.Info:
						severity = Severity.INFO;
						break;

					case LogLevel.Warn:
						severity = Severity.WARN;
						break;

					case LogLevel.Error:
						severity = Severity.ERROR;
						break;

					case LogLevel.Fatal:
						severity = Severity.ERROR;
						break;

					case LogLevel.Off:
						return;
				}

				StringBuilder sb = new StringBuilder();
				FormatOutput(sb, level, message, exception);
				Logger.Log(severity, LogCategory.APP, "[Engine] " + sb.ToString());
			}
		}
	}
}