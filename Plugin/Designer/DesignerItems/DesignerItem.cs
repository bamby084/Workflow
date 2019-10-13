using System.Windows.Controls;

namespace Designer.DesignerItems
{
    public class DesignerItem: ContentControl
    {
        public DesignerItem()
        {
            this.SetValue(DesignerCanvas.IsSelectableProperty, true);
        }
    }
}
