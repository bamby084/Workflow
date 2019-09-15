using JdSuite.Common;
using JdSuite.Common.Logging;
using JdSuite.Common.Logging.Enums;
using JdSuite.Common.Module;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Designer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IXmlSerializable
	{
		private static MainWindow instance;
		private static bool WindowClosed = false;
		private Guid ModuleGuid;
		public const string APP_NAME = "Designer";
		public const double SCROLL_FACTOR = 0.001d;
		public Production ProductionControl = new Production();
		public Development DevelopmentControl = new Development();
		public Field Schema = null;
		public XDocument SchemaData = null;
		public Workflow Workflow;

		private MainWindow()
		{
			Logger.AppName = APP_NAME;
			InitializeComponent();
			ProductionControl.Visibility = Visibility.Hidden;
			DevelopmentControl.Visibility = Visibility.Hidden;
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			WindowClosed = true;
		}

		public static MainWindow Instance {
			get {
				if (WindowClosed) {
					WindowClosed = false;
					instance = null;
				}

				if (instance == null) {
					instance = new MainWindow();
				}

				return instance;
			}
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Loads the specified *.flo file and schema configuration. If the Designer saved
		/// state is not found, a new layout will be loaded.
		/// </summary>
		/// <param name="workflow">Active workflow.</param>
		/// <param name="guid">Relevant module guid.</param>
		/// <param name="schema">The schema.</param>
		/// <param name="data">The data.</param>
		public void Load(Workflow workflow, Guid guid, Field schema, XDocument data)
		{
			ModuleGuid = guid;
			Workflow = workflow;
			SchemaData = data;
			Schema = schema;

			var element = workflow.GetAppState(APP_NAME, guid);
			ResetState();
			if (element != null) {
				using (var reader = element.CreateReader()) {
#if DEBUG
					ReadXml(reader);
					return;
#else
					try {
						ReadXml(reader);
						return;
					}
					catch (Exception e) {
						var msg = $"Could not load saved workflow: {e.Message}. Falling back to a new workflow.";
						Logger.Log(Severity.ERROR, LogCategory.APP, msg);
						MessageBox.Show(msg, "", MessageBoxButton.OK, MessageBoxImage.Error);
					}
#endif
				}
			}

			DesignerControl.LoadDefault();
		}

		public void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement();
			var serializer = new XmlSerializer(typeof(Pages));
			DesignerControl.Load((Pages)serializer.Deserialize(reader));
            if (reader.NodeType != XmlNodeType.EndElement)
                DesignerControl.LoadTreeItems(reader);

			reader.ReadEndElement();
		}

		/// <summary>
		/// Resets the state of the application to just after launch with parameters.
		/// </summary>
		public void ResetState()
		{
			Title = APP_NAME;
			DesignerControl = new DesignerPage();
			DevelopmentControl = new Development();
			ProductionControl = new Production();

			ProductionControl.Visibility = Visibility.Hidden;
			DevelopmentControl.Visibility = Visibility.Hidden;

			SetActive(DesignerControl);
			if (Schema != null) {
				DesignerControl.UnpackSchema(Schema);
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Pages");
            DesignerControl.Pages.WriteXml(writer);
            writer.WriteStartElement("Placeholders");
            {
                DesignerControl.FontStyles.WriteXml(writer);
                DesignerControl.ImageStyles.WriteXml(writer);
                DesignerControl.ColorStyles.WriteXml(writer);
                DesignerControl.ParagraphStyles.WriteXml(writer);
                DesignerControl.TextStyles.WriteXml(writer);
                DesignerControl.LineStyles.WriteXml(writer);
                DesignerControl.BorderStyles.WriteXml(writer);
                DesignerControl.BlockStyles.WriteXml(writer);
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

		public void GotoDesignerView()
		{
			SetActive(DesignerControl);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.Key == Key.F5 && DesignerControl.Pages.Count > 0) {
				SetActive(ProductionControl);
			}
			else if (e.Key == Key.F6 && DesignerControl.Pages.Count > 0 && SchemaData != null) {
				DevelopmentControl = new Development();
				DevelopmentControl.Load(DesignerControl.Pages, Schema, SchemaData);
				SetActive(DevelopmentControl);
			}
			else if (e.Key == Key.F6) {
				string msg = "";
				if (DesignerControl.Pages.Count <= 0) {
					msg += "Page template does not exist. Please design one first.{0}";
				}
				if (SchemaData == null) {
					msg += "Data file and/or schema is not loaded.{0}";
				}

				if (msg.Length > 0) {
					MessageBox.Show(string.Format(msg, Environment.NewLine), "", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
			else if (e.Key == Key.Escape) {
				if (Content != DesignerControl) {
					SetActive(DesignerControl);
				}
			}
		}

		public void CacheWorkflow()
		{
			using (var stream = new MemoryStream()) {
				using (var writer = new XmlTextWriter(stream, System.Text.Encoding.Default)) {
					this.WriteXml(writer);

                    

                    writer.Flush();
					stream.Position = 0;
					using (XmlReader reader = XmlReader.Create(stream)) {
						Workflow.SetAppState(APP_NAME, XElement.Load(reader), ModuleGuid);
					}
				}
			}
		}

		public XElement SerializeState()
		{
			using (var stream = new MemoryStream()) {
				using (var writer = new XmlTextWriter(stream, System.Text.Encoding.Default)) {
					writer.Formatting = Formatting.Indented;
					this.WriteXml(writer);
					writer.Flush();
					stream.Position = 0;
					using (XmlReader reader = XmlReader.Create(stream)) {
						return XElement.Load(reader);
					}
				}
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			if (Content != DesignerControl) {
				GotoDesignerView();
				e.Cancel = true;
				return;
			}

			try {
				MainWindow.Instance.CacheWorkflow();
			}
			catch (Exception ex) {
				var msg = "Could not save changes to the current workflow. " + ex.Message;
				Logger.Log(Severity.ERROR, LogCategory.APP, msg);
				var result = MessageBox.Show(msg, "", MessageBoxButton.OKCancel, MessageBoxImage.Error);
				if (result == MessageBoxResult.Cancel) {
					e.Cancel = true;
				}
			}
		}

		private void SetActive(UserControl control)
		{
			((UserControl)Content).Visibility = Visibility.Hidden;
			control.Visibility = Visibility.Visible;
			Content = control;
		}
	}
}