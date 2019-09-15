using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DataFilter.Models.Enums;
using DataFilter.Models.Filters;

namespace DataFilter.FilterStrategies
{
    public class BoolFilterStrategy : FilterStrategy
    {
        private Dictionary<string, BoolValue> conditionTypes = new Dictionary<string, BoolValue>()
        {
            { "Yes", BoolValue.Yes },
            { "No", BoolValue.No }
        };

        public BoolFilterStrategy(BoolFilter filter,
            ComboBox conditionComboBox,
            TextBox valueTextBox,
            GroupBox filterGroupBox,
            DockPanel valueDockPanel)
            : base(conditionComboBox, valueTextBox, filterGroupBox, valueDockPanel)
        {
            this.Filter = filter;
        }

        protected BoolFilter Filter { get; set; }

        public override void UpdateFilterView()
        {
            var itemsSource = conditionTypes.Select(x =>
                    new ComboBoxItem()
                    {
                        Content = x.Key,
                        IsSelected = x.Value == Filter.Condition
                    });

            ConditionComboBox.ItemsSource = itemsSource;

            ConditionComboBox.Text = (string)(itemsSource.FirstOrDefault(x => x.IsSelected))?.Content;

            ValueDockPanel.Visibility = Visibility.Collapsed;

            FilterGroupBox.Header = "Bool";
        }

        public override void UpdateFilterCondition()
        {
            if (ConditionComboBox.SelectedValue != null)
            {
                Filter.Condition = conditionTypes[(string)((ComboBoxItem)ConditionComboBox.SelectedValue).Content];
            }
        }

        public override void UpdateFilterValue()
        {

        }

        public override Filter CreateNewFilterInstance()
        {
            return new BoolFilter();
        }
    }
}
