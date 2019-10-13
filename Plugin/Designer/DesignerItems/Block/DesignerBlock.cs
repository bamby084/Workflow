
using System.Windows;

namespace Designer.DesignerItems
{
    public class DesignerBlock: DesignerItem
    {
        static DesignerBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerBlock), new FrameworkPropertyMetadata(typeof(DesignerBlock)));
        }

        public DesignerBlock()
        {
            
        }
    }
}
