using DataFilter.Models.Enums;

namespace DataFilter.Models.Filters
{
    public class IntFilter: Filter
    {
        public ConditionType Condition { get; set; }

        public string Value { get; set; }
    }
}
