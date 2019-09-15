using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AppWorkflow
{
	/// <summary>
	/// Interaction logic for NewWorkflow.xaml
	/// </summary>
	public partial class NewWorkflow : Window
	{
		public bool IsConfirmed = false;

		public NewWorkflow()
		{
			InitializeComponent();
		}

		private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog();
			dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			dialog.IsFolderPicker = true;
			if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
				if (TextboxName.Text.Length > 0) {
					var fullPath = Path.Combine(dialog.FileName, TextboxName.Text + ".flo");
					if (File.Exists(fullPath)) {
						MessageBox.Show("Unable to create a new workflow: Workflow already exists in specified directory.", "", MessageBoxButton.OK, MessageBoxImage.Error);
						Focus();
						return;
					}
				}
				TextboxLocation.Text = dialog.FileName;
			}
			Focus();
		}

		private void ButtonOK_Click(object sender, RoutedEventArgs e)
		{
			IsConfirmed = true;
			Close();
		}

		private void Textbox_TextChanged()
		{
			if (TextboxName.Text.Length > 0 && Directory.Exists(TextboxLocation.Text)) {
				ButtonOK.IsEnabled = true;
			}
			else {
				ButtonOK.IsEnabled = false;
			}
		}

		private void TextboxLocation_TextChanged(object sender, TextChangedEventArgs e)
		{
			Textbox_TextChanged();
		}

		private void TextboxName_TextChanged(object sender, TextChangedEventArgs e)
		{
			Textbox_TextChanged();
		}
	}
}