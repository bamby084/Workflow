using System;
using System.Collections;

namespace Designer.ExtensionMethods
{
    public static class TypeExtensions
    {
        public static bool IsEnumrable(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
        }
    }
}
