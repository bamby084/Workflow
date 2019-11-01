using Designer.DesignerItems;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Designer.DesignerTreeViewItems
{
    public abstract class DesignerTreeViewItem: TreeViewItem
    {
        public abstract ImageSource Image { get; }
        public object AssociatedItem { get; set; }

        public event EventHandler OnDeleted;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if(e.Key == Key.Delete)
            {
                ((ISelectable) AssociatedItem).IsSelected = false;

                if (OnDeleted != null)
                    OnDeleted(this, new EventArgs());
            }
        }
    }
}
