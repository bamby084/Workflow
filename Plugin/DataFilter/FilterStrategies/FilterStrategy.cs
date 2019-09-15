using System.Windows.Controls;
using DataFilter.Models.Filters;

namespace DataFilter.FilterStrategies
{
    public abstract class FilterStrategy
    {
        public FilterStrategy(ComboBox conditionComboBox,
            TextBox valueTextBox,
            GroupBox filterGroupBox,
            DockPanel valueDockPanel)
        {
            ConditionComboBox = conditionComboBox;
            ValueTextBox = valueTextBox;
            FilterGroupBox = filterGroupBox;
            ValueDockPanel = valueDockPanel;
        }

        protected ComboBox ConditionComboBox { get; set; }

        protected TextBox ValueTextBox { get; set; }

        protected GroupBox FilterGroupBox { get; set; }

        protected DockPanel ValueDockPanel { get; set; }

        public abstract void UpdateFilterView();

        public abstract void UpdateFilterCondition();

        public abstract void UpdateFilterValue();

        public abstract Filter CreateNewFilterInstance();
    }
}
