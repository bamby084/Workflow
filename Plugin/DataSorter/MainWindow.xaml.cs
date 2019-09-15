using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JdSuite.DataSorting.ViewModels;
using Microsoft.Win32;
using JdSuite.Common.Module;
using JdSuite.Common;

namespace JdSuite.DataSorting
{
    public partial class MainWindow : Window
    {

        NLog.ILogger logger = NLog.LogManager.GetLogger("DataSorting.MainWindow");

        public ViewModel WindowViewModel { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            //this.Icon =   .SetValue(Window.IconProperty, Properties.Resources.sorting_icon_9);

            WindowViewModel = new ViewModel()
            {
                FieldNodes = new ObservableCollection<Field>()
            };

           // WindowViewModel.DataDirectory = @"E:\Canvas\Dropbox\BlockApp\Data";

            DataContext = WindowViewModel;

            gridSortingField.ItemsSource = WindowViewModel.SortingFields;
             


        }

        public void LoadSchema(string FileName)
        {
            logger.Trace("Loading Schema from file:{0}", FileName);
            JdSuite.Common.Module.Field rootField = JdSuite.Common.Module.Field.Parse(FileName);

            WindowViewModel.FieldNodes.Add(rootField);

        }

        public void LoadSchema(Field schemaNode)
        {
            logger.Trace("Loading schema from field");
            WindowViewModel.FieldNodes.Add(schemaNode);
        }

        private void AddSortingFieldButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedField = treeView.SelectedItem as Field;

            if (selectedField == null)
            {
                return;
            }

            bool Found = false;
            List<Field> Chain = new List<Field>();

            Field.FindChain(WindowViewModel.FieldNodes.FirstOrDefault(), ref selectedField, ref Found, ref Chain);

            Chain.Reverse();
            string xpathString = "/" + string.Join("/", Chain.Select(x => x.Name));


            var model = (ViewModel)DataContext;

            model.SortingFields.Add(new SortingField()
            {
                // Id = selectedField.Id,
                Name = selectedField.Name,
                SortingType = SortingType.Ascending,
                ComparisonMode = ComparisonMode.CaseSensitive,
                RemoveDuplicate = false,
                XPath = xpathString
            });

            // selectedField.IsSortedBy = true;

            model.IsAnyFieldSorted = true;
        }

        private void DeleteSortingFieldButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedField = gridSortingField.SelectedItem as SortingField;

            if (selectedField == null)
            {
                return;
            }

            var model = (ViewModel)DataContext;

            model.SortingFields.Remove(selectedField);

            if (!model.SortingFields.Any())
            {
                model.IsAnyFieldSorted = false;
            }

            // model.SubFields.Select(field => field[selectedField.Id]).First(field => field != null).IsSortedBy = false;
        }

        private void MoveUpSortingFieldButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = gridSortingField.SelectedIndex;

            if (selectedIndex < 1)
            {
                return;
            }

            var model = (ViewModel)DataContext;

            var itemToMoveUp = gridSortingField.SelectedItem as SortingField;
            model.SortingFields.RemoveAt(selectedIndex);
            model.SortingFields.Insert(selectedIndex - 1, itemToMoveUp);
            this.gridSortingField.SelectedIndex = selectedIndex - 1;
        }

        private void MoveDownSortingFieldButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = gridSortingField.SelectedIndex;

            var model = (ViewModel)DataContext;

            if (selectedIndex == model.SortingFields.Count - 1 || selectedIndex == -1)
            {
                return;
            }

            var itemToMoveUp = gridSortingField.SelectedItem as SortingField;
            model.SortingFields.RemoveAt(selectedIndex);
            model.SortingFields.Insert(selectedIndex + 1, itemToMoveUp);
            this.gridSortingField.SelectedIndex = selectedIndex + 1;
        }
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedField = treeView.SelectedItem as Field;

            ((ViewModel)DataContext).IsAddFieldButtonEnabled =
                selectedField != null && selectedField.Type.ToLower() != "array" &&
                (selectedField.ChildNodes == null || !selectedField.ChildNodes.Any());
        }

        private void SortingFieldsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel model = (ViewModel)DataContext;

            var selectedField = gridSortingField.SelectedItem as SortingField;

            model.IsSortedFieldSelected = selectedField != null;

            model.IsSortedFieldMovable = model.SortingFields.Count > 1 && model.IsSortedFieldSelected;
        }

         

        private void BtnSort_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            if (System.IO.Directory.Exists(WindowViewModel.DataDirectory))
                dlg.InitialDirectory = WindowViewModel.DataDirectory;

            dlg.Filter = "XML Schema files (*.xml)|*.xml|All files (*.*)|*.*";
            dlg.CheckPathExists = true;
            dlg.DefaultExt = "xml";

            try
            {
                if (dlg.ShowDialog() == true)
                {

                    if (!File.Exists(WindowViewModel.InputDataFile))
                    {
                        MessageBox.Show("Please provide a valid input xml data file");
                        return;
                    }

                   
                    XMLSorter sorter = new XMLSorter();
                    sorter.DataFile = WindowViewModel.InputDataFile;
                    sorter.OutputFileName =WindowViewModel.OutputDataFile;

                    foreach (var item in WindowViewModel.SortingFields)
                    {
                        sorter.SortingFields.Add(item);
                    }

                    sorter.LoadData();
                    sorter.Sort();
                    sorter.Save();
                    MessageBox.Show("XML sorted and saved successfully");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                JdSuite.Common.MessageService.ShowError("XML sorting error", ex.Message);
            }
        }

        


        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (WindowViewModel.SortingFields.Count < 1)
            {
                MessageService.ShowError("Error", "No field is selected for sorting");
                this.DialogResult = false;
            }
            else
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        
    }
}
