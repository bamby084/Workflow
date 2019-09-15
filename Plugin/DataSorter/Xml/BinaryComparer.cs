using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdSuite.DataSorting
{
    public class BinaryComparer : BaseComparer<string>
    {
        static NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(FloatComparer));


        public override int Compare(string left, string right)
        {

            int result = -1;

            if (left == null)
            {
                // if lhs null, check rhs to decide on return value.
                if (right == null)
                {
                    result = 0;
                }
                else
                {
                    result = -1;
                }
            }
            else if (right == null)
            {
                result = 1;
            }
            else
            {
                
                var LBytes = Encoding.UTF8.GetBytes(left);
                var RBytes = Encoding.UTF8.GetBytes(right);

                if (LBytes.Length < RBytes.Length)
                    result = -1;
                else if (LBytes.Length > RBytes.Length)
                {
                    result = 1;
                }
                else
                {
                    for (int i = 0; i < LBytes.Length; i++)
                    {
                        result = LBytes[i].CompareTo(RBytes[i]);
                        if (result != 0)
                            break;
                    }
                }

                // return leftVal < rightVal ? -1 : leftVal > rightVal ? 1 : 0;
            }

            return result;

        }

    }
}
