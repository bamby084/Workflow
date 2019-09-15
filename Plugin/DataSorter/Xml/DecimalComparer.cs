using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdSuite.DataSorting
{
    public class DecimalComparer : BaseComparer<string>
    {
        static NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(FloatComparer));

        public override int Compare(string left, string right)
        {
            if (!decimal.TryParse(left, out var leftVal))
            {
                logger.Warn($"Cannot convert value {left} to Decimal");
            }

            if (!decimal.TryParse(right, out var rightVal))
            {
                logger.Warn($"Cannot convert value {right} to Decimal");
            }

            return leftVal.CompareTo(rightVal);
        }
    }
}
