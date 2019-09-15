using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JdSuite.Common.Module;

namespace JdSuite.Common.TreeListView
{
    public class DataTypeChecker
    {

        public static bool IsValid(Field ff, string Value)
        {
            bool isValid = false;

            if (ff.DataType == "String")
            {
                isValid = true;
            }
            else if (ff.DataType == "Integer")
            {
                isValid = int.TryParse(Value, out var z);
            }
            else if (ff.DataType == "Float")
            {
                isValid = float.TryParse(Value, out var z);
            }
            else if (ff.DataType == "Decimal")
            {
                isValid = decimal.TryParse(Value, out var z);
            }
            else if (ff.DataType == "DateTime")
            {
                isValid = DateTime.TryParse(Value, out var z);
            }

            return isValid;
        }
    }
}
