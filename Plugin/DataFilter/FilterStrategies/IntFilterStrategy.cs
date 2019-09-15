using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DataFilter.Models.Enums;
using DataFilter.Models.Filters;

namespace DataFilter.FilterStrategies
{
    public class IntFilterStrategy : FilterStrategy
    {
        public IntFilterStrategy(IntFilter filter,
            ComboBox conditionComboBox,
            TextBox valueTextBox,
            GroupBox filterGroupBox,
            DockPanel valueDockPanel)
            : base(conditionComboBox, valueTextBox, filterGroupBox, valueDockPanel)
        {
            this.Filter = filter;
        }

        private Dictionary<string, ConditionType> conditionTypes = new Dictionary<string, ConditionType>()
        {
            { "Contains",ConditionType.Contains },
            { "Equal to", ConditionType.Equal },
            { "Begins with", ConditionType.BeginsWith },
        };

        protected IntFilter Filter { get; set; }

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

            ValueDockPanel.Visibility = Visibility.Visible;
            ValueTextBox.Text = Filter.Value;

            FilterGroupBox.Header = "Int";
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
            Filter.Value = ValueTextBox.Text;
        }

        public override Filter CreateNewFilterInstance()
        {
            return new IntFilter();
        }
    }
}
