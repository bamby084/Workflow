using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Designer.DesignerTreeViewItems
{
    public class BlockTreeViewItem : DesignerTreeViewItem
    {
        static BlockTreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlockTreeViewItem), new FrameworkPropertyMetadata(typeof(DesignerTreeViewItem)));
        }

        public override ImageSource Image => new BitmapImage(new Uri("pack://application:,,,/Designer;component/Resources/Toolbar/block.png"));
    }
}
