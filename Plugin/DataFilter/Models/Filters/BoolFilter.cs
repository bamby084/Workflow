using DataFilter.Models.Enums;

namespace DataFilter.Models.Filters
{
    public class BoolFilter: Filter
    {
        public BoolValue Condition { get; set; }
    }
}
