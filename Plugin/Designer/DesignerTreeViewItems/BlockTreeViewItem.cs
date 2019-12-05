using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Designer.DesignerItems;

namespace Designer.DesignerTreeViewItems
{
    public class BlockTreeViewItem : DesignerTreeViewItem
    {
        static BlockTreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlockTreeViewItem), new FrameworkPropertyMetadata(typeof(DesignerTreeViewItem)));
        }

        private DesignerItem _associatedItem;
        public override DesignerItem AssociatedItem {
            get => _associatedItem;
            set
            {
                _associatedItem = value;
                var binding = new Binding("Children");
                binding.Source = _associatedItem;
                SetBinding(ItemsSourceProperty, binding);
            }
        }

        public override ImageSource Image => new BitmapImage(new Uri("pack://application:,,,/Designer;component/Resources/Toolbar/block.png"));
    }
}
