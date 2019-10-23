using Designer.DesignerItems;
using System.Windows.Controls;
using System.Windows.Media;

namespace Designer.DesignerTreeViewItems
{
    public abstract class DesignerTreeViewItem: TreeViewItem
    {
        public abstract ImageSource Image { get; }
        public DesignerItem AssociatedItem { get; set; }
    }
}
