using System.Windows.Controls;

namespace Designer.ExtensionMethods
{
    public static class ContextMenuExtensions
    {
        public static MenuItem FindMenuItemByName(this ContextMenu contextMenu, string name)
        {
            foreach (var item in contextMenu.Items)
            {
                if (item is MenuItem menuItem)
                {
                    var foundItem = menuItem.FindMenuItemByName(name);
                    if (foundItem != null)
                        return foundItem;
                }
            }

            return null;
        }

        private static MenuItem FindMenuItemByName(this MenuItem menuItem, string name)
        {
            if (menuItem.Name == name)
                return menuItem;

            foreach (MenuItem subMenuItem in menuItem.Items)
            {
                var foundItem = subMenuItem.FindMenuItemByName(name);
                if (foundItem != null)
                    return foundItem;
            }

            return null;
        }
    }
}
