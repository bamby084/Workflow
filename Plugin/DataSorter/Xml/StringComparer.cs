using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdSuite.DataSorting
{
    public class StringComparer : BaseComparer<string>
    {
        static NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(FloatComparer));

        StringComparison options;

        public StringComparer(SortingField sf)
        {
            

            if (sf.ComparisonMode == ComparisonMode.IgnoreCase)
            {
                options = StringComparison.OrdinalIgnoreCase;
            }
            else if (sf.ComparisonMode == ComparisonMode.CaseSensitive)
            {
                options = StringComparison.Ordinal;
            }else
            {
                throw new ArgumentOutOfRangeException("SortingField.ComparisonMode", $"SortingField {sf.XPath} has invalid ComparisonMode {sf.ComparisonMode} for Stringcomparer");
            }
        }

        public override int Compare(string left, string right)
        {
            return string.Compare(left, right, options);
        }
    }
}
