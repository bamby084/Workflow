using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdSuite.DataSorting
{
    public class IntegerComparer : BaseComparer<string>
    {
        static NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(FloatComparer));

        public override int Compare(string left, string right)
        {
           if(! int.TryParse(left, out var leftVal))
            {
                logger.Warn($"Cannot convert value {left} to integer");
            }

            if(!int.TryParse(right, out var rightVal))
            {
                logger.Warn($"Cannot convert value {right} to integer");
            }

            return leftVal.CompareTo(rightVal);

            // return leftVal < rightVal ? -1 : leftVal > rightVal ? 1 : 0;
        }
    }


   
}
