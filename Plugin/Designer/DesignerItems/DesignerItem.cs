using System.Windows;
using System.Windows.Controls;

namespace Designer.DesignerItems
{
    public abstract class DesignerItem: ContentControl, IControlPropertyProvider
    {
        public DesignerItem()
        {
            this.SetValue(DesignerCanvas.IsSelectableProperty, true);
        }

        public virtual ControlPropertiesViewModel Properties => null;
    }
}
