using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdSuite.DataSorting
{
    public class BaseComparer<T> : IComparer<T>
    {
        public virtual int Compare(T x, T y)
        {
            throw new NotImplementedException();
        }
    }
}
