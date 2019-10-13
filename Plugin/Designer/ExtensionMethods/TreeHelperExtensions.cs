using System.Windows;
using System.Windows.Media;

namespace Designer.ExtensionMethods
{
    public static class TreeHelperExtensions
    {
        public static T FindVisualParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            if (child == null)
                return null;

            var parent = VisualTreeHelper.GetParent(child);
            if (parent == null)
                return null;

            if (parent is T)
                return (T)parent;

            return FindVisualParent<T>(parent);
        }

        public static T FindParent<T>(DependencyObject child)
            where T: DependencyObject
        {
            if (child == null)
                return null;

            var parent = LogicalTreeHelper.GetParent(child);
            if (parent == null)
                return null;

            if (parent is T)
                return (T)parent;

            return FindParent<T>(parent);
        }
    }
}
