
using System;

namespace Designer.ExtensionMethods
{
    public static class NumberExtensions
    {
        public static T Clamp<T>(this T value, T min, T max) where T: IComparable<T>
        {
            if (value.CompareTo(min) < 0)
                return min;
            else if (value.CompareTo(max) > 0)
                return max;
            else
                return value;
        }
    }
}
