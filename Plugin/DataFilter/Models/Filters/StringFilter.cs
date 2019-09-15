using DataFilter.Models.Enums;

namespace DataFilter.Models.Filters
{
    public class StringFilter: Filter
    {
        public ConditionType Condition { get; set; }

        public string Value { get; set; }
    }
}
