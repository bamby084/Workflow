using DataFilter.FilterStrategies;
using DataFilter.Models;
using DataFilter.Models.Enums;
using DataFilter.Models.Filters;
using DataFilter.Services;
using DataFilter.Services.Interfaces;
using DataFilter.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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

namespace DataFilter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ISchemeReader schemeReader = new SchemeReader();

        private Dictionary<string, Func<Filter, ComboBox, TextBox, GroupBox, DockPanel, FilterStrategy>> filterStrategies = 
            new Dictionary<string, Func<Filter, ComboBox, TextBox, GroupBox, DockPanel, FilterStrategy>>()
        {
            { "string", (Filter filter, ComboBox comboBox, TextBox textBox, GroupBox filterGroupBox, DockPanel valueDockPanel) => 
            new StringFilterStrategy((StringFilter)filter, comboBox, textBox, filterGroupBox, valueDockPanel) },
            { "bool", (Filter filter, ComboBox comboBox, TextBox textBox, GroupBox filterGroupBox, DockPanel valueDockPanel) => 
            new BoolFilterStrategy((BoolFilter)filter, comboBox, textBox, filterGroupBox, valueDockPanel) },
            { "int", (Filter filter, ComboBox comboBox, TextBox textBox, GroupBox filterGroupBox, DockPanel valueDockPanel) =>
            new IntFilterStrategy((IntFilter)filter, comboBox, textBox, filterGroupBox, valueDockPanel) }
        };

        public MainWindow()
        {
            InitializeComponent();

            var rootField = schemeReader.ReadSchema(ConfigurationManager.AppSettings["schemeFilePath"]);

            var model = new Model()
            {
                SubFields = new ObservableCollection<Field>(rootField.SubFields)
            };

            DataContext = model;

            treeView.ItemsSource = rootField.SubFields;
        }

        private void AddGroupingFieldButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedField = treeView.SelectedItem as Field;

            selectedField.Filter = filterStrategies[selectedField.Type.ToLower()](null, conditionComboBox, valueTextBox, filterGroupBox, valueDockPanel).CreateNewFilterInstance();

            UpdateAddFieldButtonEnabled();
            UpdateDeleteFieldButtonEnabled();
            UpdateRightPart(selectedField);

            ReloadTreeView();
        }

        private void DeleteGroupingFieldButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedField = treeView.SelectedItem as Field;

            selectedField.Filter = null;

            UpdateAddFieldButtonEnabled();
            UpdateDeleteFieldButtonEnabled();
            UpdateRightPart(selectedField);

            ReloadTreeView();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedField = treeView.SelectedItem as Field;

            SelectField(selectedField);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((Model)DataContext).Unload();
        }

        private void ReloadTreeView()
        {
            ((Model)DataContext).SubFields = new ObservableCollection<Field>(((Model)DataContext).SubFields.ToList());
        }

        private void SelectField(Field selectedField)
        {
            var model = (Model)DataContext;

            foreach(var field in model.SubFields)
            {
                DeselectField(field);
            }

            selectedField.IsSelected = true;

            UpdateAddFieldButtonEnabled();
            UpdateDeleteFieldButtonEnabled();
            UpdateRightPart(selectedField);
        }

        private void DeselectField(Field field)
        {
            field.IsSelected = false;

            foreach (var subField in field.SubFields)
            {
                DeselectField(subField);
            }
        }

        private void UpdateAddFieldButtonEnabled()
        {
            var selectedField = treeView.SelectedItem as Field;

            ((Model)DataContext).IsAddFieldButtonEnabled =
                selectedField != null
                && selectedField.Type.ToLower() != "array"
                && selectedField.Filter == null
                && (selectedField.SubFields == null || !selectedField.SubFields.Any());
        }

        private void UpdateDeleteFieldButtonEnabled()
        {
            var selectedField = treeView.SelectedItem as Field;

            ((Model)DataContext).IsDeleteFieldButtonEnabled =
                selectedField != null
                && selectedField.Filter != null;
        }

        private void UpdateRightPart(Field selectedField)
        {
            if (selectedField == null || selectedField.Filter == null)
            {
                conditionComboBox.ItemsSource = null;
                valueTextBox.Text = "";

                conditionComboBox.IsEnabled = false;
                valueDockPanel.Visibility = Visibility.Collapsed;
                normalizationModeCheckBox.IsEnabled = false;

                filterGroupBox.Header = "Filter";

                return;
            }

            conditionComboBox.IsEnabled = true;
            normalizationModeCheckBox.IsEnabled = true;

            filterStrategies[selectedField.Type.ToLower()](selectedField.Filter, conditionComboBox, valueTextBox, filterGroupBox, valueDockPanel).UpdateFilterView();
        }

        private void ConditionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedField = treeView.SelectedItem as Field;

            if (selectedField.Filter != null)
            {
                filterStrategies[selectedField.Type.ToLower()](selectedField.Filter, conditionComboBox, valueTextBox, filterGroupBox, valueDockPanel).UpdateFilterCondition();
            }
        }

        private void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var selectedField = treeView.SelectedItem as Field;

            if (selectedField.Filter != null)
            {
                filterStrategies[selectedField.Type.ToLower()](selectedField.Filter, conditionComboBox, valueTextBox, filterGroupBox, valueDockPanel).UpdateFilterValue();
            }
        }
    }

    
}
